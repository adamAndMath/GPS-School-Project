using Backend;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class CarTest
{
    [Test]
    public void SetAccelerationTest()
    {
        //Arrange
        var car = new Car();

        //Act
        //Try to set acceleration and deceleration
        var acceleration = 1.5F;
        var deceleration = 6F;
        var niceDeceleration = 3.5F;
        car.Acceleration = acceleration;
        car.Deceleration = deceleration;
        car.NiceDeceleration = niceDeceleration;

        //Assert
        //The object has a new acceleration and a new deceleration
        Assert.AreEqual(acceleration, car.Acceleration);
        Assert.AreEqual(deceleration, car.Deceleration);
        Assert.AreEqual(niceDeceleration, car.NiceDeceleration);
    }

    [Test]
    public void PointEstimationMapTest()
    {
        //Arrange
        var car = new Car();
        World.WorldScale = 3;
        var node0 = new Node(new Vector2(0, 0));
        var node1 = new Node(new Vector2(4, 3));
        var node2 = new Node(new Vector2(7, 7));

        var road0 = new Road(node0, node1);
        var road1 = new Road(node1, node2);

        node0.Roads.Add(road0);
        node1.Roads.Add(road1);

        //Act
        var time = car.TimePoints(road0);
        var estimate = car.TimeEstimate(node1, node2);

        //Assert
        Assert.AreEqual(15, time);
        Assert.AreEqual(15, estimate);
    }

    [Test]
    public void PointEstimationGPSTest()
    {
        //Arrange
        var car = new CarGPS();
        World.WorldScale = 3;
        World.EstimationSpeedLimit = 5;
        var node0 = new Node(new Vector2(0, 0));
        var node1 = new Node(new Vector2(4, 3));
        var node2 = new Node(new Vector2(7, 7));

        var road0 = new Road(node0, node1) { SpeedLimit = 3 };
        var road1 = new Road(node1, node2) { SpeedLimit = 3 };

        node0.Roads.Add(road0);
        node1.Roads.Add(road1);

        //Act
        var time = car.TimePoints(road0);
        var estimate = car.TimeEstimate(node1, node2);

        //Assert
        Assert.AreEqual(5, time);
        Assert.AreEqual(3, estimate);
    }
}
