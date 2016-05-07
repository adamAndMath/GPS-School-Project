using CTD_Sim.Backend;
using System.Collections.Generic;
using UnityEngine;

namespace CTD_Sim.Frontend
{
    public class NodeFront : MonoBehaviour
    {
        public static readonly List<NodeFront> Nodes = new List<NodeFront>();

        public new SpriteRenderer renderer;
        public Lights lights;
        public NodeSprites sprites;
        public NodeType nodeType;
        public float spawnRate;
        public Color colorStart;
        public Color colorStop;
        public NodeLight.LightSequence lightSequence;

        private float timer;
        private INode node;
        private int roadCount;
        private Vector2 direction;

        public INode Node { get { return node; } }

        public enum NodeType { Turn, MainRoad, Light }

        [System.Serializable]
        public struct NodeSprites
        {
            public Sprite straight;
            public Sprite turn;
            public Sprite tCross;
            public Sprite cross;
        }

        [System.Serializable]
        public struct Lights
        {
            public SpriteRenderer up;
            public SpriteRenderer left;
            public SpriteRenderer down;
            public SpriteRenderer right;
        }

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
            Direction dir = (Direction)Mathf.RoundToInt(transform.rotation.eulerAngles.z / 90);

            switch (nodeType)
            {
                case NodeType.Turn:
                    node = new Node(transform.position, dir);
                    break;
                case NodeType.MainRoad:
                    node = new NodeMainRoad(transform.position, dir);
                    break;
                case NodeType.Light:
                    node = new NodeLight(transform.position, dir, lightSequence);
                    break;
            }
        }

        public void AddRoad(IRoad from, IRoad to)
        {
            roadCount++;

            if (from == null)
            {
                direction += (to.From.Position - to.To.Position).normalized;
            }
            else
            {
                node.Roads.Add(from);
                direction += (from.To.Position - from.From.Position).normalized;
            }

            var nodeFrom = from == null ? to.To.Position : from.From.Position;
            var nodeTo = from == null ? to.From.Position : from.To.Position;
            var v = nodeFrom - nodeTo;
            var dir = Mathf.FloorToInt((180 + 45 + Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg - transform.rotation.eulerAngles.z) / 90) % 4;

            node.RoadFrom[dir] = from;
            node.RoadTo[dir] = to;
        }

        void Update()
        {
            if (node.Position != (Vector2)transform.position)
                node.Position = transform.position;

            transform.localScale = new Vector3(World.RealRoadWidth * 2, World.RealRoadWidth * 2, 1);

            switch (roadCount)
            {
                case 1:
                    renderer.sprite = sprites.straight;
                    break;
                case 2:
                    if (direction == Vector2.zero)
                    {
                        renderer.sprite = sprites.straight;
                    }
                    else
                    {
                        renderer.sprite = sprites.turn;
                        renderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0, 0, -45);
                    }
                    break;
                case 3:
                    renderer.sprite = sprites.tCross;
                    renderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                    break;
                default:
                    renderer.sprite = sprites.cross;
                    break;
            }

            if (nodeType == NodeType.Light)
            {
                NodeLight nodeLight = (NodeLight)Node;
                lights.up.enabled = nodeLight.RoadFrom[(int)Direction.Up] != null || nodeLight.RoadTo[(int)Direction.Up] != null;
                lights.left.enabled = nodeLight.RoadFrom[(int)Direction.Left] != null || nodeLight.RoadTo[(int)Direction.Left] != null;
                lights.down.enabled = nodeLight.RoadFrom[(int)Direction.Down] != null || nodeLight.RoadTo[(int)Direction.Down] != null;
                lights.right.enabled = nodeLight.RoadFrom[(int)Direction.Right] != null || nodeLight.RoadTo[(int)Direction.Right] != null;

                lights.up.color = lights.down.color = nodeLight.Light ? colorStop : colorStart;
                lights.left.color = lights.right.color = nodeLight.Light ? colorStart : colorStop;
            }

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