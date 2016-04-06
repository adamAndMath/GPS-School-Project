using Backend;
using UnityEngine;
using System.Collections.Generic;

namespace Frontend
{
    public class NodeFront : MonoBehaviour
    {
        public static readonly List<NodeFront> Nodes = new List<NodeFront>();

        public NodeType nodeType;
        public float spawnRate;

        private float timer;
        private INode node;
        public INode Node { get { return node; } }

        public enum NodeType { Turn, MainRoad }

        void OnEnable()
        {
            Nodes.Add(this);
        }

        void OnDisable()
        {
            Nodes.Remove(this);
        }

        void Start()
        {
            switch (nodeType)
            {
                case NodeType.Turn:
                    node = new Node(transform.position);
                    break;
                case NodeType.MainRoad:
                    node = new NodeMainRoad(transform.position);
                    break;
            }
        }

        public void AddRoad(IRoad from, IRoad to)
        {
            if (from != null)
                node.Roads.Add(from);

            var nodeFrom = from == null ? to.To.Position : from.From.Position;
            var nodeTo = from == null ? to.From.Position : from.To.Position;
            var v = nodeFrom - nodeTo;
            var dir = Mathf.FloorToInt(((360 + 45 + Mathf.Atan2(v.x, -v.y) * Mathf.Rad2Deg - transform.rotation.z) / 4) % 4);

            switch (nodeType)
            {
                case NodeType.MainRoad:
                    switch (dir)
                    {
                        case 0:
                            ((NodeMainRoad)node).upFrom = from;
                            ((NodeMainRoad)node).upTo = to;
                            break;
                        case 1:
                            ((NodeMainRoad)node).rightFrom = from;
                            ((NodeMainRoad)node).rightTo = to;
                            break;
                        case 2:
                            ((NodeMainRoad)node).downFrom = from;
                            ((NodeMainRoad)node).downTo = to;
                            break;
                        case 3:
                            ((NodeMainRoad)node).leftFrom = from;
                            ((NodeMainRoad)node).leftTo = to;
                            break;
                    }
                    break;
            }
        }

        void Update()
        {
            if (node.Position != (Vector2) transform.position)
                node.Position = transform.position;

            if (spawnRate > 0)
            {
                timer -= Time.deltaTime * World.TimeScale;

                if (timer <= 0)
                {
                    timer = spawnRate;
                    WorldFront.GetCar(Node, Nodes[(Nodes.IndexOf(this) + Random.Range(1, Nodes.Count - 1)) % Nodes.Count].Node);
                }
            }
        }
    }
}