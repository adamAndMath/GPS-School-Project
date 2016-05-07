using CTD_Sim.Backend;
using UnityEngine;

namespace CTD_Sim.Frontend
{
    public class RoadFront : MonoBehaviour
    {
        public NodeFront from;
        public NodeFront to;

        public float speedLimit;

        private IRoad roadForward;
        private CTD.IRoad ctdForward;
        public bool backward;
        private IRoad roadBackward;
        private CTD.IRoad ctdBackward;

        void Start()
        {
            roadForward = new Road(WorldFront.Instance.NextRoadID, from.Node, to.Node) { SpeedLimit = speedLimit };
            ctdForward = new CTD.Road(roadForward.ID);
            World.CTDManager.AddRoad(ctdForward);

            if (backward)
            {
                roadBackward = new Road(WorldFront.Instance.NextRoadID, to.Node, from.Node) { SpeedLimit = speedLimit };
                ctdBackward = new CTD.Road(roadBackward.ID);
                World.CTDManager.AddRoad(ctdBackward);
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