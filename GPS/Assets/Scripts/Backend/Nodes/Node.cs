using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backend
{
    public class Node : INode
    {
        private readonly List<IRoad> road = new List<IRoad>();

        public Dictionary<Direction, Dictionary<Direction, List<ICar>>> cars;
        public IRoad[] RoadFrom { get; private set; }
        public IRoad[] RoadTo { get; private set; }

        public Vector2 Position { get; set; }
        public Direction Rotation { get; set; }
        public List<IRoad> Roads { get { return road; } }

        public Node(Vector2 position, Direction rotation)
        {
            Position = position;
            Rotation = rotation;
            cars = new Dictionary<Direction, Dictionary<Direction, List<ICar>>>();
            RoadFrom = new IRoad[4];
            RoadTo = new IRoad[4];

            for (int i = 0; i < 4; i++)
            {
                var side = new Dictionary<Direction, List<ICar>>();
                cars.Add((Direction)i, side);

                for (int j = 1; j < 4; j++)
                {
                    side.Add((Direction)((i + j) % 4), new List<ICar>());
                }
            }
        }

        public virtual void Update(float deltaTime)
        {

        }
        public Direction GetRoadDirection(IRoad road)
        {
            for (int dir = 0; dir < 4; dir++)
            {
                if (road == RoadFrom[dir] || road == RoadTo[dir])
                    return (Direction)dir;
            }

            throw new ArgumentException();
        }

        public virtual void AddCar(ICar car, IRoad from, IRoad to)
        {
            cars[GetRoadDirection(from)][GetRoadDirection(to)].Add(car);
        }

        public virtual void RemoveCar(ICar car, IRoad from, IRoad to)
        {
            cars[GetRoadDirection(from)][GetRoadDirection(to)].Remove(car);
        }

        public virtual float GetLength(IRoad from, IRoad to)
        {
            var dir = (4 + GetRoadDirection(to) - GetRoadDirection(from)) % 4;

            switch (dir)
            {
                case 1: return World.RoadWidth * 0.75F * Mathf.PI;
                case 2: return World.RoadWidth * 2;
                case 3: return World.RoadWidth * 0.25F * Mathf.PI;
            }

            throw new Exception();
        }

        public virtual void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown)
        {
            var dir = GetRoadDirection(from);
            var dirTo = GetRoadDirection(to);
            var dif = (4 + dirTo - dir) % 4;
            var distance = GetLength(from, to) - progress;

            if (HasSpaceFromFront(car, dirTo))
            {
                switch (dif)
                {
                    case 1:
                        if (HasSpaceFromSide(car, distance, (Direction)(((int)dir + 1) % 4))
                         && HasSpaceFromSide(car, distance, (Direction)(((int)dir + 2) % 4))
                         && HasSpaceFromSide(car, distance, (Direction)(((int)dir + 3) % 4)))
                            return;
                        break;
                    case 2:
                        if (HasSpaceFromSide(car, distance, (Direction)(((int)dir + 1) % 4))
                         && HasSpaceFromSide(car, distance, (Direction)(((int)dir + 3) % 4)))
                            return;
                        break;
                    case 3: return; //Allways allowed right
                }
            }

            if (-progress < car.SafeDistance(car.Speed))
            {
                requiredSlowdown = Mathf.Max(requiredSlowdown, car.Deceleration);
            }
            else
            {
                requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(0, -progress));
            }
        }

        protected bool HasSpaceFromSide(ICar car, float distanceLeft, Direction dir)
        {
            if (cars[dir][(Direction)(((int)dir + 2) % 4)].Count != 0)
            {
                return false;
            }

            var road = RoadTo[(int)dir];

            if (road == null)
            {
                return true;
            }

            var relevantCars = road.Cars.Where(c => c.Path.Count > 1)
                                        .Where(c => ((4 + GetRoadDirection(c.Path[1]) - dir) % 4) == 2)
                                        .ToArray();

            if (relevantCars.Length == 0)
            {
                return true;
            }

            return relevantCars[0].TimeTo(road.Length - relevantCars[0].Progress) > car.TimeTo(distanceLeft);
        }

        protected bool HasSpaceFromFront(ICar car, Direction dir)
        {
            var road = RoadFrom[(int)dir];

            if (road.CarCount == 0) return true;

            var c = road[road.CarCount - 1];

            return c.Progress - car.SafeDistance(Mathf.Min(car.Speed, c.Speed)) > World.RoadWidth;
        }

        public override string ToString()
        {
            return Position.ToString();
        }
    }
}