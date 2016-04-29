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
                transform.position = GetRoadPosition();
            }
        }

        Vector3 GetRoadPosition()
        {
            var road = car.Path[0];
            var from = road.From.Position;
            var to = road.To.Position;
            return Vector2.Lerp(from, to, (car.Progress + World.RoadWidth) / (road.Length + 2 * World.RoadWidth)) + new Vector2(to.y - from.y, from.x - to.x).normalized * World.RealRoadWidth;
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