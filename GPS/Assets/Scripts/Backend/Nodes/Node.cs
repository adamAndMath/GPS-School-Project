using System;
using System.Collections.Generic;
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
        public List<IRoad> Roads { get { return road; } }

        public Node(Vector2 position)
        {
            Position = position;
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

        public virtual void AddCar(ICar car, IRoad from, IRoad to)
        {
            cars[(Direction)Array.IndexOf(RoadTo, from)][(Direction)Array.IndexOf(RoadFrom, to)].Add(car);
        }

        public virtual void RemoveCar(ICar car, IRoad from, IRoad to)
        {
            cars[(Direction)Array.IndexOf(RoadTo, from)][(Direction)Array.IndexOf(RoadFrom, to)].Remove(car);
        }

        public virtual float GetLength(IRoad from, IRoad to)
        {
            var dir = (4 + Array.IndexOf(RoadFrom, to) - Array.IndexOf(RoadTo, from)) % 4;

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

        }

        public override string ToString()
        {
            return Position.ToString();
        }
    }
}