using System.Collections.Generic;
using UnityEngine;

namespace CTD_Sim.Backend
{
    public class Car : ICar
    {
        public bool Complete { get { return Path == null || Path.Count == 0; } }
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }
        public float NiceDeceleration { get; set; }
        public List<IRoad> Path { get; protected set; }
        public float Progress { get; set; }
        public bool IsNoding { get; set; }
        public virtual float Speed { get; set; }
        public INode From { get; private set; }
        public INode To { get; private set; }

        public virtual void Init(INode from, INode to)
        {
            From = from;
            To = to;
            Speed = 0;
            Path = World.Pathfinder.FindPath(this, from, to);
            Path[0].AddCar(this);
        }

        public virtual void Return()
        {

        }

        public virtual void OnRoadChanged()
        {

        }

        public void UpdateSpeedAndProgress(float deltaTime)
        {
            var roadIndex = Path[0].IndexOfCar(this);
            var speedLimit = Path[0].SpeedLimit;

            float requiredSlowdown = 0;
            float progress = Progress;

            if (!IsNoding)
            {
                Path[0].GetSlowdown(this, progress, 0, roadIndex, ref requiredSlowdown);
                progress -= Path[0].Length;
            }

            for (var i = 1; i < Path.Count; i++)
            {
                Path[i].From.GetSlowdown(this, Path[i - 1], Path[i], progress, roadIndex, ref requiredSlowdown);
                progress -= Path[i].From.GetLength(Path[i - 1], Path[i]);
                Path[i].GetSlowdown(this, progress, roadIndex, Path[i].CarCount, ref requiredSlowdown);
                progress -= Path[i].Length;
                roadIndex += Path[i].CarCount;
            }

            if (requiredSlowdown > NiceDeceleration)
            {
                requiredSlowdown = Mathf.Min(requiredSlowdown, Deceleration);

                var timeToZero = Speed / requiredSlowdown;

                if (deltaTime < timeToZero)
                {
                    Progress += Speed * deltaTime - requiredSlowdown * Mathf.Pow(deltaTime, 2) / 2;
                    Speed -= deltaTime * requiredSlowdown;
                }
                else
                {
                    Progress += Speed * timeToZero - requiredSlowdown * Mathf.Pow(timeToZero, 2) / 2;
                    Speed = 0;
                }
            }
            else
            {
                var timeToLimit = (speedLimit - Speed) / Acceleration;

                if (deltaTime < timeToLimit)
                {
                    Progress += Speed * deltaTime + Acceleration * Mathf.Pow(deltaTime, 2) / 2;
                    Speed += deltaTime * Acceleration;
                }
                else
                {
                    Progress += Speed * timeToLimit + Acceleration * Mathf.Pow(timeToLimit, 2) / 2 + speedLimit * (deltaTime - timeToLimit);
                    Speed = speedLimit;
                }
            }

            if (IsNoding)
            {
                if (Progress > Path[0].To.GetLength(Path[0], Path[1]))
                {
                    Progress -= Path[0].To.GetLength(Path[0], Path[1]);
                    IsNoding = false;
                    Path[0].To.RemoveCar(this, Path[0], Path[1]);
                    Path.RemoveAt(0);
                    Path[0].AddCar(this);
                    OnRoadChanged();
                }
            }
            else if (Progress > Path[0].Length)
            {
                Progress -= Path[0].Length;
                Path[0].RemoveCar(this);

                if (Path.Count == 1)
                {
                    Path.Clear();
                }
                else
                {
                    IsNoding = true;
                    Path[0].To.AddCar(this, Path[0], Path[1]);
                }
            }
        }

        public float RequiredDecceleration(float speedTo, float distance)
        {
            return (Mathf.Pow(Speed, 2) - Mathf.Pow(speedTo, 2)) / (2 * distance);
        }

        public float SafeDistance(float speed)
        {
            return Mathf.Pow(speed, 2) / (2 * NiceDeceleration) + World.CarDistance;
        }

        public float TimeTo(float distance)
        {
            return Mathf.Sqrt(2 * distance / Acceleration + Mathf.Pow(Speed / Acceleration, 2)) - Speed / Acceleration;
        }

        public virtual float TimePoints(IRoad road)
        {
            return road.Length;
        }

        public virtual float TimeEstimate(INode from, INode to)
        {
            return (to.Position - from.Position).magnitude * World.WorldScale;
        }
    }
}