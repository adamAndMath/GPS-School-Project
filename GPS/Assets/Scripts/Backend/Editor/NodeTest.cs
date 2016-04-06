using Backend;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class NodeTest
{
    [Test]
    public void InitTest()
    {
        //Arrange
        //None

        //Act
        //Try to create a node
        var position = new Vector2(4, 6.25F);
        var newNode = new Node(position);

        //Assert
        //The node has a the position
        Assert.AreEqual(position, newNode.Position);
    }

    [Test]
    public void ToStringTest()
    {
        //Arrange
        var pos = new Vector2(3.5F, 7);

        //Act
        var node = new Node(pos);

        //Assert
        Assert.AreEqual(pos.ToString(), node.ToString());
    }

    [Test]
    public void SlowdownTestTCross()
    {
        //Arrange
        World.CarDistance = 1;
        World.WorldScale = 1;

        var node = new NodeMainRoad(new Vector2(0, 0));
        var nodeUp = new Node(new Vector2(0, 1));
        var nodeDown = new Node(new Vector2(0, -1));
        var nodeRight = new Node(new Vector2(1, 0));

        var roadUpTo = new Road(nodeUp, node);
        var roadUpFrom = new Road(node, nodeUp);
        var roadDownTo = new Road(nodeDown, node);
        var roadDownFrom = new Road(node, nodeDown);
        var roadRightTo = new Road(nodeRight, node);
        var roadRightFrom = new Road(node, nodeRight);

        node.Roads.Add(roadUpFrom);
        nodeUp.Roads.Add(roadUpTo);
        node.Roads.Add(roadDownFrom);
        nodeDown.Roads.Add(roadDownTo);
        node.Roads.Add(roadRightFrom);
        nodeRight.Roads.Add(roadRightTo);

        node.upTo = roadUpTo;
        node.upFrom = roadUpFrom;
        node.downTo = roadDownTo;
        node.downFrom = roadDownFrom;
        node.rightTo = roadRightTo;
        node.rightFrom = roadRightFrom;

        var car = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1 };
        var carOther = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1 };

        roadUpTo.AddCar(car);
        roadDownTo.AddCar(carOther);
        car.Progress = roadUpTo.Length;
        carOther.Progress = roadDownTo.Length;

        //Act
        float requiredSlowdown = 0;
        node.GetSlowdown(car, roadUpTo, roadRightFrom, 0, 0, ref requiredSlowdown);

        //Assert
        Assert.AreEqual(2, requiredSlowdown);
    }
}
