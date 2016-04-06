using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    [ExecuteInEditMode]
    public class Road : MonoBehaviour
    {
        public Node from;
        public Node to;
        public float speedLimit;
        public List<Car> cars = new List<Car>();

        public float RealLength { get { return (to.transform.position - from.transform.position).magnitude; } }
        public float Length { get { return RealLength * World.Instance.worldScale; } }

        public void Init(Node from, Node to)
        {
            this.from = from;
            this.to = to;
            transform.position = from.transform.position;
        }

        void OnDestroy()
        {
            from.roads.Remove(this);
        }

        void Update()
        {
            transform.position = (to.transform.position + from.transform.position) / 2;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, to.transform.position - from.transform.position);
            transform.localScale = new Vector3(1, RealLength, 1);
        }

        public void GetSlowdown(Car car, float progress, int preIndex, int roadIndex, ref float requiredSlowdown)
        {
            if (car.speed > speedLimit)
            {
                if (progress > 0)
                {
                    requiredSlowdown = Mathf.Max(requiredSlowdown, (Mathf.Pow(speedLimit, 2) - Mathf.Pow(car.speed, 2)) / (2 * -progress));
                }
                else
                {
                    requiredSlowdown = car.deceleration;
                    return;
                }
            }

            for (var i = roadIndex - 1; i >= 0; i--)
            {
                if (preIndex + roadIndex - i > World.Instance.lookForward)
                    break;

                var c = cars[i];


                var dist = c.pro - progress - (preIndex + roadIndex - i) * (Mathf.Pow(car.speed, 2) / (2 * car.niceDeceleration) + World.Instance.carDistance);

                if (dist > World.Instance.viewDistance) break;

                if (dist < 0)
                {
                    requiredSlowdown = car.deceleration;
                    return;
                }

                if (c.speed >= car.speed) continue;

                requiredSlowdown = Mathf.Max(requiredSlowdown, (Mathf.Pow(car.speed, 2) - Mathf.Pow(c.speed, 2)) / (2 * dist));
            }
        }
    }
}