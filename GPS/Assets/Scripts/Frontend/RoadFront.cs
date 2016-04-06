using Backend;
using UnityEngine;

namespace Frontend
{
    public class RoadFront : MonoBehaviour
    {
        public NodeFront from;
        public NodeFront to;

        public float speedLimit;

        private IRoad roadForward;
        public bool backward;
        private IRoad roadBackward;

        void Start()
        {
            roadForward = new Road(from.Node, to.Node) { SpeedLimit = speedLimit};

            if (backward)
            {
                roadBackward = new Road(to.Node, from.Node) { SpeedLimit = speedLimit };
            }

            from.AddRoad(roadForward, roadBackward);
            to.AddRoad(roadBackward, roadForward);
        }

        void Update()
        {
            transform.position = (to.Node.Position + from.Node.Position) / 2;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, to.Node.Position - from.Node.Position);
            transform.localScale = new Vector3(World.RealRoadWidth * 2, roadForward.RealLength, 1);
        }
    }
}