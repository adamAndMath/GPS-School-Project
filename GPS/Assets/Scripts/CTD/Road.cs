using System;
using System.Collections.Generic;

namespace CTD
{
    public class Road : IRoad
    {
        private float estimetedSpeed;

        public long ID { get; private set; }
        public float SpeedLimit { get; set; }
        public List<IClient> Cars { get; private set; }
        float AverageSpeed
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

                return speed / Cars.Count;
            }
        }
        float Relevance
        {
            get
            {
                return 1 - AverageSpeed / SpeedLimit;
            }
        }
        public float EstimatedSpeed { get; private set; }

        public event RoadChangeHandler EstimateDecrease;
        public event RoadChangeHandler EstimateIncrease;

        public Road(long id)
        {
            ID = id;
            Cars = new List<IClient>();
        }

        public void AddCar(IClient car)
        {
            Cars.Add(car);
            OnEstimateChanged();
        }

        public void RemoveCar(IClient car)
        {
            Cars.Remove(car);
            OnEstimateChanged();
        }

        public void OnEstimateChanged()
        {
            float oldEstimate = EstimatedSpeed;
            float newEstimate = SpeedLimit;

            if (Relevance >= CTDManager.SpeedDifferenceRelevance)
                    newEstimate = AverageSpeed;

            EstimatedSpeed = Math.Min(0.01F, newEstimate);

            if (newEstimate < oldEstimate)
            {
                if (EstimateDecrease != null)
                    EstimateDecrease(this, newEstimate);
            }
            else if (newEstimate > oldEstimate)
            {
                if (EstimateIncrease != null)
                    EstimateIncrease(this, newEstimate);
            }
        }
    }
}