using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    public class House : MonoBehaviour
    {
        private static readonly List<House> houses = new List<House>();

        public Node node;
        public float rate;

        private float timer;

        void OnEnable()
        {
            houses.Add(this);
        }

        void Update()
        {
            timer -= Time.deltaTime * World.Instance.timeScale;

            if (timer < 0)
            {
                World.GetCar(node, houses[(Random.Range(1, houses.Count) + houses.IndexOf(this)) % houses.Count].node);

                timer = rate;
            }
        }
    }
}