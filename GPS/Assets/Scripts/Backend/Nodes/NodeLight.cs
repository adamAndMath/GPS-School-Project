using UnityEngine;
using System.Collections;

namespace Backend
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

        public NodeLight(Vector2 position, Direction rotation, LightSequence sequence) : base(position, rotation)
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
            if (light == (from == RoadTo[(int)Direction.Up] || from == RoadTo[(int)Direction.Down]))
            {
                if (-progress-1 < car.SafeDistance(car.Speed))
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, car.Deceleration);
                }
                else
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, car.RequiredDecceleration(0, -progress-1));
                }
            }
        }
    }
}