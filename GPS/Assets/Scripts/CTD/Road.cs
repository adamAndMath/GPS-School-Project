using System;
using System.Collections.Generic;

namespace CTD
{
    public class Road : IRoad
    {
        private float lastUpdate;
        private SpeedData sumSpeed;
        private List<SpeedData> speedData = new List<SpeedData>();

        public long ID { get; private set; }
        public float SpeedLimit { get; set; }
        public List<IClient> Cars { get; private set; }
        float SumSpeed
        {
            get
            {
                if (Cars.Count == 0)
                    return SpeedLimit;

                float speed = 0;

                foreach (IClient car in Cars)
                {
                    speed += car.Speed;
                }

                return speed;
            }
        }
        float Relevance
        {
            get
            {
                return 1 - sumSpeed.Estimate / SpeedLimit;
            }
        }
        public float EstimatedSpeed
        {
            get
            {
                if (sumSpeed.weight == 0)
                    return SpeedLimit;

                return Math.Max(0.01F, Relevance > CTDManager.SpeedDifferenceRelevance ? sumSpeed.Estimate : SpeedLimit);
            }
        }

        public event RoadChangeHandler EstimateDecrease;
        public event RoadChangeHandler EstimateIncrease;

        public struct SpeedData
        {
            public float speed;
            public int weight;
            public float deltaTime;

            public float Estimate { get { return speed / weight; } }

            public static SpeedData operator +(SpeedData a, SpeedData b)
            {
                return new SpeedData { speed = a.speed + b.speed, weight = a.weight + b.weight, deltaTime = a.deltaTime + b.deltaTime };
            }

            public static SpeedData operator -(SpeedData a, SpeedData b)
            {
                return new SpeedData { speed = a.speed - b.speed, weight = a.weight - b.weight, deltaTime = a.deltaTime - b.deltaTime };
            }
        }

        public Road(long id)
        {
            ID = id;
            Cars = new List<IClient>();
        }

        public void AddCar(IClient car, float time)
        {
            Cars.Add(car);
            OnEstimateChanged(time);
        }

        public void RemoveCar(IClient car, float time)
        {
            Cars.Remove(car);
            OnEstimateChanged(time);
        }

        public void OnEstimateChanged(float time)
        {
            float oldEstimate = EstimatedSpeed;

            if (speedData.Count != 0)
            {
                var lastData = speedData[speedData.Count - 1];
                sumSpeed.deltaTime -= lastData.deltaTime;
                lastData.deltaTime = time - lastUpdate;
                sumSpeed.deltaTime += lastData.deltaTime;
                speedData[speedData.Count - 1] = lastData;
            }

            SpeedData newData = new SpeedData { speed = SumSpeed, weight = Cars.Count, deltaTime = 0 };
            speedData.Add(newData);
            sumSpeed += newData;
            lastUpdate = time;

            while (sumSpeed.deltaTime > CTDManager.TimeMemory)
            {
                sumSpeed -= speedData[0];
                speedData.RemoveAt(0);
            }

            float newEstimate = EstimatedSpeed;

            if (newEstimate < oldEstimate)
            {
                if (EstimateDecrease != null)
                    EstimateDecrease(ID, newEstimate);
            }
            else if (newEstimate > oldEstimate)
            {
                if (EstimateIncrease != null)
                    EstimateIncrease(ID, newEstimate);
            }
        }
    }
}