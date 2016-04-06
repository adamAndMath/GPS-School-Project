using Backend;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frontend
{
    public class CarPool
    {
        private static readonly Type[] CarParameterTypes = new Type[0];
        private static readonly object[] CarParameters = new object[0];

        public readonly Type carType;
        public readonly CarFront prefab;
        public readonly Stack<CarFront> cars = new Stack<CarFront>();

        private int carCount;

        public CarPool(Type type, CarFront prefab)
        {
            carType = type;
            this.prefab = prefab;
        }

        public CarFront GetCar(INode from, INode to)
        {
            CarFront car;

            if (cars.Count == 0)
            {
                car = GameObject.Instantiate(prefab.gameObject).GetComponent<CarFront>();
                car.car = (ICar) carType.GetConstructor(CarParameterTypes).Invoke(CarParameters);
                car.gameObject.name = car.identifier + (++carCount);
            }
            else
            {
                car = cars.Pop();
            }

            car.car.Init(from, to);
            return car;
        }

        public void ReturnCar(CarFront car)
        {
            cars.Push(car);
        }
    }
}