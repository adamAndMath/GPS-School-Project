using UnityEngine;
using System.Collections;

namespace Backend
{
    public class NodeMainRoad : Node
    {
        public NodeMainRoad(Vector2 position) : base(position) { }

        public override void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown)
        {
            if (to == null) return;

            if (from == RoadTo[(int)Direction.Down])
            {
                if (to != RoadFrom[(int)Direction.Left] || (HasSpaceFromSide(car, progress - World.RoadWidth / 2, RoadTo[(int)Direction.Up]) && HasSpaceFromFront(car, progress, to)))
                    return;
            }
            else if (from == RoadTo[(int)Direction.Up])
            {
                if (to != RoadFrom[(int)Direction.Right] || (HasSpaceFromSide(car, progress - World.RoadWidth / 2, RoadTo[(int)Direction.Down]) && HasSpaceFromFront(car, progress, to)))
                    return;
            }
            else if (from == RoadTo[(int)Direction.Right])
            {
                if (to == RoadFrom[(int)Direction.Up])
                {
                    if (HasSpaceFromSide(car, progress, RoadTo[(int)Direction.Down]) && HasSpaceFromFront(car, progress, to))
                        return;
                }
                else if (HasSpaceFromSide(car, progress - World.RoadWidth / 2, RoadTo[(int)Direction.Up]) && HasSpaceFromSide(car, progress - World.RoadWidth / 2, RoadTo[(int)Direction.Down]) && HasSpaceFromFront(car, progress, to))
                    return;
            }
            else if (from == RoadTo[(int)Direction.Left])
            {
                if (to == RoadFrom[(int)Direction.Down])
                {
                    if (HasSpaceFromSide(car, progress, RoadTo[(int)Direction.Up]) && HasSpaceFromFront(car, progress, to))
                        return;
                }
                else if (HasSpaceFromSide(car, progress - World.RoadWidth / 2, RoadTo[(int)Direction.Down]) && HasSpaceFromSide(car, progress, RoadTo[(int)Direction.Up]) && HasSpaceFromFront(car, progress - World.RoadWidth / 2, to))
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