using Backend;
using UnityEngine;

namespace Frontend
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CarFront : MonoBehaviour
    {
        public string identifier;
        public float acceleration;
        public float deceleration;
        public float niceDeceleration;

        public ICar car;

        private new SpriteRenderer renderer;
        private Color color;
        private bool mouseOver;

        void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            color = renderer.color;
        }

        void Start()
        {
            car.Acceleration = acceleration;
            car.Deceleration = deceleration;
            car.NiceDeceleration = niceDeceleration;
        }

        void Update()
        {
            if (car.Complete)
            {
                if (renderer.enabled == true)
                {
                    renderer.enabled = false;
                    WorldFront.ReturnCar(this);
                }
            }
            else
            {
                renderer.enabled = true;
                transform.position = car.IsNoding ? GetNodePosition() : GetRoadPosition();
            }
        }

        Vector3 GetRoadPosition()
        {
            var road = car.Path[0];
            var from = road.From.Position;
            var to = road.To.Position;
            return Vector2.Lerp(from, to, (car.Progress + World.RoadWidth) / (road.Length + 2 * World.RoadWidth)) + new Vector2(to.y - from.y, from.x - to.x).normalized * World.RealRoadWidth * 0.5F;
        }

        Vector3 GetNodePosition()
        {
            IRoad from = car.Path[0];
            IRoad to = car.Path[1];
            INode node = from.To;
            Vector2 pos = node.Position;
            int dir = ((int) node.Rotation + (int)node.GetRoadDirection(from)) % 4;
            float pro = car.Progress / node.GetLength(from, to);
            float sin = Mathf.Sin(pro * Mathf.PI * 0.5F);
            float cos = Mathf.Cos(pro * Mathf.PI * 0.5F);
            Vector2 posAdd = Vector2.zero;

            switch ((4 + node.GetRoadDirection(to) - node.GetRoadDirection(from)) % 4)
            {
                case 1: posAdd = new Vector2(1 - cos * 1.5F, 1 - sin * 1.5F) * World.RealRoadWidth;
                    break;
                case 2: posAdd = new Vector2(-0.5F, 1 - 2 * pro) * World.RealRoadWidth;
                    break;
                case 3: posAdd = new Vector2(-1 + cos * 0.5F, 1 - sin * 0.5F) * World.RealRoadWidth;
                    break;
                default: throw new System.Exception();
            }

            var pa = posAdd;

            switch (dir)
            {
                case 1:
                    posAdd.x = pa.y;
                    posAdd.y = -pa.x;
                    break;
                case 2:
                    posAdd.x = -pa.x;
                    posAdd.y = -pa.y;
                    break;
                case 3:
                    posAdd.x = -pa.y;
                    posAdd.y = pa.x;
                    break;
            }

            return pos + posAdd;
        }

        void OnMouseUpAsButton()
        {
            WorldFront.Instance.SellectedCar = this;
        }

        void OnMouseEnter()
        {
            mouseOver = true;
            UpdateColor();
        }

        void OnMouseExit()
        {
            mouseOver = false;
            UpdateColor();
        }

        public void UpdateColor()
        {
            if (mouseOver || WorldFront.Instance.SellectedCar == this)
            {
                renderer.color = Color.Lerp(color, Color.white, 0.75F);
            }
            else
            {
                renderer.color = color;
            }
        }

        void OnGUI()
        {
            if (car.Complete) return;

            var pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y;

            switch (WorldFront.Instance.carLabel)
            {
                case WorldFront.CarLabel.Name:
                    GUI.Label(new Rect(pos, Vector2.one * 32), gameObject.name);
                    break;
                case WorldFront.CarLabel.RoadID:
                    GUI.Label(new Rect(pos, Vector2.one * 32), car.Path[0].IndexOfCar(car).ToString());
                    break;
            }
        }
    }
}