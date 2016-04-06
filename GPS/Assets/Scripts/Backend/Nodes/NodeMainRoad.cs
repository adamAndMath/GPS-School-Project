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

            if (from == downTo)
            {
                if (to != leftFrom || (HasSpaceFromSide(car, progress - World.RoadWidth / 2, upTo) && HasSpaceFromFront(car, progress, to)))
                    return;
            }
            else if (from == upTo)
            {
                if (to != rightFrom || (HasSpaceFromSide(car, progress - World.RoadWidth / 2, downTo) && HasSpaceFromFront(car, progress, to)))
                    return;
            }
            else if (from == rightTo)
            {
                if (to == upFrom)
                {
                    if (HasSpaceFromSide(car, progress, downTo) && HasSpaceFromFront(car, progress, to))
                        return;
                }
                else if (HasSpaceFromSide(car, progress - World.RoadWidth / 2, upTo) && HasSpaceFromSide(car, progress - World.RoadWidth / 2, downTo) && HasSpaceFromFront(car, progress, to))
                    return;
            }
            else if (from == leftTo)
            {
                if (to == downFrom)
                {
                    if (HasSpaceFromSide(car, progress, upTo) && HasSpaceFromFront(car, progress, to))
                        return;
                }
                else if (HasSpaceFromSide(car, progress - World.RoadWidth / 2, downTo) && HasSpaceFromSide(car, progress, upTo) && HasSpaceFromFront(car, progress - World.RoadWidth / 2, to))
                    return;
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

        private bool HasSpaceFromSide(ICar car, float progress, IRoad check)
        {
            if (check == null || check.CarCount == 0) return true;
            
            var c = check[0];

            return c.TimeTo(check.Length - c.Progress) > car.TimeTo(-progress);
        }

        private bool HasSpaceFromFront(ICar car, float progress, IRoad check)
        {
            if (check.CarCount == 0) return true;
            
            var c = check[check.CarCount - 1];

            return c.Progress - car.SafeDistance(Mathf.Min(car.Speed, c.Speed)) > World.RoadWidth;
        }
    }
}