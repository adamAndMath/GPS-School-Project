using UnityEngine;

namespace CTD_Sim.Frontend
{
    public class WorldBuilder : MonoBehaviour
    {
        public NodeFront node;
        public RoadFront road;

        private NodeFront roadStart;
        private Vector2 clickPos;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickPos = RealMousePos();
                roadStart = GetNode(clickPos);
            }
        
            if (Input.GetMouseButtonUp(0))
            {
                NodeFront roadEnd = GetNode(RealMousePos());

                if (roadStart == null)
                {
                    if (roadEnd == null)
                    {
                        NodeFront clone = Instantiate(node);
                        clone.transform.position = RealMousePos();
                    }
                }
                else
                {
                    Debug.Log("Yep");
                    if (roadEnd != null && roadEnd != roadStart)
                    {
                        RoadFront clone = Instantiate(road);
                        clone.from = roadStart;
                        clone.to = roadEnd;
                    }
                }

                roadStart = null;
            }
        }

        private Vector2 RealMousePos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
        }

        private NodeFront GetNode(Vector2 pos)
        {
            NodeFront node = null;

            foreach (var n in NodeFront.Nodes)
            {
                if (n.GetComponent<Collider2D>().OverlapPoint(pos) && (node == null || ((Vector2)node.transform.position - clickPos).sqrMagnitude > ((Vector2)n.transform.position - pos).sqrMagnitude))
                    node = n;
            }

            return node;
        }
    }
}
