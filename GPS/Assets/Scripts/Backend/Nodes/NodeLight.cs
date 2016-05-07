using UnityEngine;
using System.Collections;

namespace CTD_Sim.Backend
{
    public class NodeLight : Node
    {
        private readonly LightSequence sequence;
        private bool light;
        private float timer;

        public bool Light { get { return light; } }

        [System.Serializable]
        public struct LightSequence
        {
            public float timeHorizontal;
            public float timeVertical;

            public LightSequence(float timeHorizontal, float timeVertical)
            {
                this.timeHorizontal = timeHorizontal;
                this.timeVertical = timeVertical;
            }
        }

        public NodeLight(Vector2 position, Direction rotation, LightSequence sequence)
            : base(position, rotation)
        {
            this.sequence = sequence;
            timer = light ? sequence.timeHorizontal : sequence.timeVertical;
        }

        public override void Update(float deltaTime)
        {
            timer -= deltaTime;

            if (timer <= 0)
            {
                light = !light;
                timer += light ? sequence.timeHorizontal : sequence.timeVertical;
            }
        }

        public override void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown)
        {
            var dir = GetRoadDirection(from);

            if ((!car.IsNoding || progress < 0) && light == (((int)dir % 2) == 0))
            {
                if (-progress - 1 < car.SafeDistance(car.Speed))
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, car.Deceleration);
                }
                else
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(0, -progress - 1));
                }
            }
            else
            {
                var dirTo = GetRoadDirection(to);
                var dif = (4 + dirTo - dir) % 4;
                var distance = GetLength(from, to) - progress;

                if (HasSpaceFromFront(car, dirTo))
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