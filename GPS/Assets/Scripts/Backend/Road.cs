using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class Road : IRoad
    {
        private List<ICar> cars = new List<ICar>();
        
        public INode From { get; private set; }
        public INode To { get; private set; }
        public float SpeedLimit { get; set; }
        public float RealLength { get { return (To.Position - From.Position).magnitude - 2*World.RealRoadWidth; } }
        public float Length { get { return RealLength * World.WorldScale; } }

        public int CarCount { get { return cars.Count; } }

        public ICar this[int i] { get { return cars[i]; } }

        public Road(INode from, INode to)
        {
            From = from;
            To = to;
        }

        public void AddCar(ICar car)
        {
            cars.Add(car);
        }

        public void RemoveCar(ICar car)
        {
            cars.Remove(car);
        }

        public int IndexOfCar(ICar car)
        {
            return cars.IndexOf(car);
        }

        public void GetSlowdown(ICar car, float progress, int preIndex, int roadIndex, ref float requiredSlowdown)
        {
            if (car.Speed > SpeedLimit)
            {
                if (progress < 0)
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(SpeedLimit, -progress));
                }
                else
                {
                    requiredSlowdown = car.Deceleration;
                    return;
                }
            }

            for (var i = roadIndex - 1; i >= 0; i--)
            {
                if (preIndex + roadIndex - i > World.LookForward)
                    break;

                var c = cars[i];
                
                var dist = c.Progress - progress - (preIndex + roadIndex - i) * car.SafeDistance(Mathf.Min(car.Speed, c.Speed));

                if (dist > World.ViewDistance) break;

                if (dist < 0)
                {
                    requiredSlowdown = car.Deceleration;
                    return;
                }

                if (c.Speed >= car.Speed) continue;

                requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(c.Speed, dist));
            }
        }

        public override string ToString()
        {
            return string.Format("Road from {0} to {1}", From, To);
        }
    }
}