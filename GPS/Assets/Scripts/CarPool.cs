using System;
using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    public class CarPool
    {
        public readonly Car prefab;
        private readonly Stack<Car> cars = new Stack<Car>();

        private int carCount;

        public CarPool(Car car)
        {
            prefab = car;
        }

        public Car GetCar(Node from, Node to)
        {
            Car car;

            if (cars.Count == 0)
            {
                car = GameObject.Instantiate(prefab.gameObject).GetComponent<Car>();
                car.gameObject.SetActive(false);
                car.gameObject.name = car.identifier + (++carCount);
            }
            else
            {
                car = cars.Pop();
            }

            car.from = from;
            car.to = to;
            car.speed = 0;
            car.gameObject.SetActive(true);
            return car;
        }

        public void ReturnCar(Car car)
        {
            cars.Push(car);
            car.gameObject.SetActive(false);
        }
    }
}