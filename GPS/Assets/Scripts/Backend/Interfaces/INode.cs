using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public interface INode
    {
        Vector2 Position { get; set; }
        Direction Rotation { get; set; }
        List<IRoad> Roads { get; }
        IRoad[] RoadFrom { get; }
        IRoad[] RoadTo { get; }

        void Update(float deltaTime);

        Direction GetRoadDirection(IRoad road);

        void AddCar(ICar car, IRoad from, IRoad to);

        void RemoveCar(ICar car, IRoad from, IRoad to);

        float GetLength(IRoad from, IRoad to);

        void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown);
    }
}