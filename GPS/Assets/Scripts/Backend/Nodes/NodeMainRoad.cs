using UnityEngine;
using System.Collections;

namespace CTD_Sim
{
    namespace Backend
    {
        public class NodeMainRoad : Node
        {
            public NodeMainRoad(Vector2 position, Direction rotation) : base(position, rotation) { }

            public override void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown)
            {
                var dir = GetRoadDirection(from);
                var dirTo = GetRoadDirection(to);
                var dif = (4 + dirTo - dir) % 4;
                var distance = GetLength(from, to) - progress;

                if (HasSpaceFromFront(car, dirTo))
                {
                    if ((int)dir % 2 == 0)
                    {
                        switch (dif)
                        {
                            case 1:
                                if (HasSpaceFromSide(car, distance, (Direction)(((int)dir + 2) % 4)))
                                    return;
                                break;
                            case 2: return; //Allways allowed straight
                            case 3: return; //Allways allowed right
                        }
                    }
                    else
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
                            case 3:
                                if (HasSpaceFromSide(car, distance, (Direction)(((int)dir + 1) % 4)))
                                    return;
                                break;
                        }
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
        }
    }
}