using CTD_Sim.Backend;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class RoadTest
{
    [Test]
    public void InitTest()
    {
        //Arrange
        var from = new Node(Vector2.zero, Direction.Up);
        var to = new Node(Vector2.zero, Direction.Up);

        //Act
        //Try to create a road
        var newRoad = new Road(725, from, to);

        //Assert
        //The id is 725
        Assert.AreEqual(725, newRoad.ID);
        //The road has the nodes as from and to
        Assert.AreEqual(from, newRoad.From);
        Assert.AreEqual(to, newRoad.To);
    }

    [Test]
    public void ToStringTest()
    {
        //Arrange
        var from = new Node(Vector2.zero, Direction.Up);
        var to = new Node(Vector2.right, Direction.Up);

        //Act
        var road = new Road(0, from, to);

        //Assert
        Assert.AreEqual(string.Format("Road from {0} to {1}", from, to), road.ToString());
    }

    [Test]
    public void LengthTest()
    {
        //Arrange
        var from = new Node(Vector2.zero, Direction.Up);
        var to = new Node(new Vector2(3, 4), Direction.Up);
        var newRoad = new Road(0, from, to);
        World.WorldScale = 2;
        World.RoadWidth = 1;

        //Act
        //None
        
        //Assert
        //The roads length is the distance between the nodes
        Assert.AreEqual(4, newRoad.RealLength);
        Assert.AreEqual(8, newRoad.Length);
    }

    [Test]
    public void SlowdownSpeedLimitTest()
    {
        //Arrange
        var from = new Node(Vector2.zero, Direction.Up);
        var to = new Node(Vector2.right, Direction.Up);
        var road = new Road(0, from, to) { SpeedLimit = 1 };
        var car = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1 };
        road.AddCar(car);
        float slowdown = 0;

        //Speed is lower than speedlimit
        road.GetSlowdown(car, 0, 0, 0, ref slowdown);
        //The car sould not slow down
        Assert.AreEqual(0F, slowdown);

        //Speed is higher than speedlimit and the car is on the road
        car.Speed = 2;
        slowdown = 0;
        road.GetSlowdown(car, 0, 0, 0, ref slowdown);
        //The car sould slow down as fast as posible
        Assert.AreEqual(2F, slowdown);

        //Speed is higher than speedlimit, but the car is not on the road yet
        slowdown = 0;
        road.GetSlowdown(car, -1, 0, 0, ref slowdown);
        //the car needs to slow down by 1.5 m/s^2 to have a speed of 1 m/s when entering the road
        Assert.AreEqual(1.5F, slowdown);
    }

    [Test]
    public void SlowdownOtherCarTest()
    {
        //Arrange
        var from = new Node(Vector2.zero, Direction.Up);
        var to = new Node(Vector2.right, Direction.Up);
        var road = new Road(0, from, to) { SpeedLimit = 100 };
        var car = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1 };
        var otherCar = new Car();
        float slowdown = 0;
        World.CarDistance = 0;

        //There is a car behind the car
        car.Progress = 0;
        otherCar.Progress = -1;
        road.AddCar(car);
        road.AddCar(otherCar);
        road.GetSlowdown(car, 0, 0, 0, ref slowdown);
        //Cars behind the car has no influence
        Assert.AreEqual(0F, slowdown);
        road.RemoveCar(otherCar);
        road.RemoveCar(car);

        //There is a car infront of the car
        slowdown = 0;
        car.Progress = 0;
        car.Speed = 1;
        otherCar.Progress = 2;
        road.AddCar(otherCar);
        road.AddCar(car);
        road.GetSlowdown(car, 0, 0, 1, ref slowdown);
        //To stop at the other cars poition the car need to decelerate by 0.25 m/s^2
        Assert.AreEqual(0.25F, slowdown);
        road.RemoveCar(otherCar);
        road.RemoveCar(car);
    }
}
