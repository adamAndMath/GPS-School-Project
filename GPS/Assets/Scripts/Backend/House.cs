using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CTD_Sim.Frontend
{
    public class House : MonoBehaviour
    {
        public static readonly List<House> Houses = new List<House>();

        public RoadFront road;
        public float min;
        public float max;
        public bool side;
        public float spawnRate;
        public int sections;

        private float timer;

        void OnEnable()
        {
            Houses.Add(this);
        }

        void OnDisable()
        {
            Houses.Remove(this);
        }

        void Start()
        {
            transform.SetParent(road.transform, false);
        }

        void Update()
        {
            transform.localPosition = side ? new Vector2(0.5F, -0.5F + (min + max) * 0.5F) : new Vector2(-0.5F, 0.5F - (min + max) * 0.5F);
            transform.localScale = new Vector3(1, max - min, 1);

            for (timer -= Time.deltaTime*WorldFront.Instance.timeScale; timer <= spawnRate; timer += spawnRate)
            {
                SpawnCar();
            }
        }

        private void SpawnCar()
        {
            House house = GetHouse();
            WorldFront.GetCar(side ? road.from.Node : road.to.Node, house.side ? house.road.from.Node : house.road.to.Node);
        }

        private House GetHouse()
        {
            var r = Random.Range(0, Houses.Sum(n => n.spawnRate) - spawnRate);

            foreach (var house in Houses)
            {
                if (house == this)
                    continue;

                r -= house.spawnRate;

                if (r < 0)
                {
                    return house;
                }
            }

            throw new Exception();
        }
    }
}
