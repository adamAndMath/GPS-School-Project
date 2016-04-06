using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    public class World : MonoBehaviour
    {
        public static World Instance { get; private set; }

        public float worldScale = 1;
        public float timeScale = 1;
        public int viewDistance = 100;
        public float carDistance = 2;
        public int lookForward = 3;
        public Car[] carPrefabs;
        public float[] carRatio;

        private List<CarPool> carPools = new List<CarPool>();

        void OnEnable()
        {
            Instance = this;
            carPools.Clear();

            foreach (var car in carPrefabs)
            {
                carPools.Add(new CarPool(car));
            }
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(16, 16, Screen.width / 4 - 20, Screen.height - 32));
            GUILayout.Label("World Scale: " + worldScale);
            worldScale = GUILayout.HorizontalSlider(worldScale, 1, 100);
            GUILayout.Label("Time Scale: " + timeScale);
            timeScale = GUILayout.HorizontalSlider(timeScale, 0, 10);
            GUILayout.Label("Car Ratio");

            for (var i = 0; i < carRatio.Length; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Width(32);
                GUILayout.Label(carPrefabs[i].identifier);
                carRatio[i] = GUILayout.HorizontalSlider(carRatio[i], 0, 1);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
        }

        public static Car GetCar(Node from, Node to)
        {
            var r = Random.Range(0, Instance.carRatio.Sum());

            for (var i = 0; i < Instance.carRatio.Length; i++)
            {
                r -= Instance.carRatio[i];

                if (r < 0)
                {
                    return Instance.carPools[i].GetCar(from, to);
                }
            }

            throw new System.Exception();
        }

        public void ReturnCar(Car car)
        {
            carPools.Find(pool => pool.prefab.GetType() == car.GetType()).ReturnCar(car);
        }
    }
}