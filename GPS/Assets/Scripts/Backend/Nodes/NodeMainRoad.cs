using UnityEngine;
using System.Collections;

namespace Backend
{
    public class NodeMainRoad : Node
    {
        public IRoad downFrom;
        public IRoad downTo;
        public IRoad upFrom;
        public IRoad upTo;
        public IRoad rightFrom;
        public IRoad rightTo;
        public IRoad leftFrom;
        public IRoad leftTo;

        public NodeMainRoad(Vector2 position) : base(position) { }

        public override void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown)
        {
            if (to == null) return;

            if (from == downFrom)
            {
                if (to == leftTo)
                {
                    if (HasSpaceFromSide(car, progress, upFrom) && HasSpaceFromFront(car, progress, to))
                    {
                        requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(0, -progress + World.CarDistance));
                    }
                }
            }
            else if (from == upFrom)
            {
                if (to == rightTo)
                {
                    if (HasSpaceFromSide(car, progress, downFrom) && HasSpaceFromFront(car, progress, to))
                    {
                        requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(0, -progress + World.CarDistance));
                    }
                }
            }
            else if (from == rightFrom)
            {
                if ((to == upTo || HasSpaceFromSide(car, progress, upFrom)) && HasSpaceFromSide(car, progress, downFrom) && HasSpaceFromFront(car, progress, to))
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(0, -progress + World.CarDistance));
                }
            }
            else if (from == leftFrom)
            {
                if ((to == downTo || HasSpaceFromSide(car, progress, downFrom)) && HasSpaceFromSide(car, progress, upFrom) && HasSpaceFromFront(car, progress, to))
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(0, -progress + World.CarDistance));
                }
            }
        }

        private bool HasSpaceFromSide(ICar car, float progress, IRoad check)
        {
            if (check == null || check.CarCount == 0) return true;
            
            var c = check[0];

            return c.TimeTo(check.Length - c.Progress) < car.TimeTo(-progress + World.RoadWidth);
        }

        private bool HasSpaceFromFront(ICar car, float progress, IRoad check)
        {
            if (check.CarCount == 0) return true;
            
            var c = check[check.CarCount - 1];

            return car.SafeDistance(Mathf.Min(car.Speed, c.Speed)) - c.Progress > World.RoadWidth;
        }
    }
}