using Backend;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

namespace Frontend
{
    public class WorldFront : MonoBehaviour
    {
        public static WorldFront Instance { get; private set; }
        private static System.Type[] carTypes = new[] { typeof(Car), typeof(CarGPS) };
        
        [System.NonSerialized]
        public readonly List<CarFront> activeCars = new List<CarFront>();

        public Grapher graph;
        public Transform sellectedFrom;
        public Transform sellectedTo;
        public CarLabel carLabel;
        public CarGraphType graphType;
        public float worldScale = 1;
        public float timeScale = 1;
        public float roadWidth = 4;
        public float estimationSpeedLimit= 1;
        public int viewDistance = 100;
        public float carDistance = 2;
        public int lookForward = 3;
        public CarFront[] carPrefabs;
        public float[] carRatio;
        public CarPool[] carPools;

        private CarFront sellectedCar;
        private FixedQueue<Vector3> carData;

        public enum CarLabel { None = 0, Name = 1, RoadID = 2 }
        public enum CarGraphType { SpeedOverTime = 0, PosOverTime = 1, SpeedOverPos = 2 }

        public CarFront SellectedCar
        {
            get { return sellectedCar; }
            set
            {
                var pre = sellectedCar;
                sellectedCar = value;
                carData.Clear();
                sellectedFrom.gameObject.SetActive(value);
                sellectedTo.gameObject.SetActive(value);

                if (pre) pre.UpdateColor();
                if (value) value.UpdateColor();
            }
        }

        void Awake()
        {
            Instance = this;
            World.WorldScale = worldScale;
            World.TimeScale = timeScale;
            World.RoadWidth = roadWidth;
            World.EstimationSpeedLimit = estimationSpeedLimit;
            World.ViewDistance = viewDistance;
            World.CarDistance = carDistance;
            World.LookForward = lookForward;

            carPools = new CarPool[carPrefabs.Length];

            for (var i = 0; i < carPools.Length; i++)
            {
                carPools[i] = new CarPool(carTypes[i], carPrefabs[i]);
            }

            carData = new FixedQueue<Vector3>(300);
        }

        void Update()
        {
            float deltaTime = Time.deltaTime * World.TimeScale;
            float pro = 0;
            IRoad road = null;

            switch (graphType)
            {
                case CarGraphType.SpeedOverTime:
                    graph.Size = new Vector2(1, 30);
                    graph.SplitPer = new Vector2(1, 10);
                    break;
                case CarGraphType.PosOverTime:
                    graph.Size = new Vector2(1, 20);
                    graph.SplitPer = new Vector2(1, 5);
                    break;
                case CarGraphType.SpeedOverPos:
                    graph.Size = new Vector2(20, 30);
                    graph.SplitPer = new Vector2(5, 10);
                    break;
            }

            if (SellectedCar)
            {
                pro = SellectedCar.car.Progress;
                road = SellectedCar.car.Path[0];
            }

            foreach (var car in activeCars)
            {
                car.car.UpdateSpeedAndProgress(deltaTime);
            }

            foreach (var node in NodeFront.Nodes)
            {
                node.Node.Update(deltaTime);
            }

            if (SellectedCar)
            {
                if (SellectedCar.car.Complete)
                {
                    SellectedCar = null;
                    carData.Clear();
                    graph.points.Clear();
                }
                else
                {
                    pro = (carData.Count == 0 ? 0 : carData.Last().z) + SellectedCar.car.Progress - pro + (road == SellectedCar.car.Path[0] ? 0 : road.Length);
                    carData.Enqueue(new Vector3(deltaTime, SellectedCar.car.Speed, pro));
                    graph.points.Clear();

                    var preTime = 1 - carData.Sum(v => v.x);
                    var posMax = 20 - carData.Last().z;

                    foreach (var v in carData)
                    {
                        preTime += v.x;

                        switch (graphType)
                        {
                            case CarGraphType.SpeedOverTime:
                                graph.points.Add(new Vector2(preTime, v.y / 30));
                                break;
                            case CarGraphType.PosOverTime:
                                graph.points.Add(new Vector2(preTime, (posMax + v.z) / 20));
                                break;
                            case CarGraphType.SpeedOverPos:
                                graph.points.Add(new Vector2((posMax + v.z) / 20, v.y / 30));
                                break;
                        }
                    }

                    sellectedFrom.position = SellectedCar.car.From.Position;
                    sellectedTo.position = SellectedCar.car.To.Position;
                    sellectedFrom.localScale = sellectedTo.localScale = Vector3.one * (2 + Mathf.Sin(Time.timeSinceLevelLoad * 2) * 0.4F) * World.RealRoadWidth;
                }
            }
        }

        public static CarFront GetCar(INode from, INode to)
        {
            var r = Random.Range(0, Instance.carRatio.Sum());

            for (var i = 0; i < Instance.carRatio.Length; i++)
            {
                r -= Instance.carRatio[i];

                if (r < 0)
                {
                    var car = Instance.carPools[i].GetCar(from, to);
                    Instance.activeCars.Add(car);
                    return car;
                }
            }

            throw new System.Exception();
        }

        public static void ReturnCar(CarFront car)
        {
            Instance.activeCars.Remove(car);
            System.Array.Find(Instance.carPools, pool => pool.carType == car.car.GetType()).ReturnCar(car);
        }
    }
}