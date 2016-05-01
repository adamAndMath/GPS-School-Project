using Backend;
using System;
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
        var newNode = new Node(position, Direction.Up);

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
        var node = new Node(pos, Direction.Up);

        //Assert
        Assert.AreEqual(pos.ToString(), node.ToString());
    }

    [Test]
    public void SlowdownTestTurn()
    {
        //Arrange
        World.CarDistance = 1;
        World.WorldScale = 1;
        World.RoadWidth = 1;

        INode nodeCenter = new Node(Vector2.zero, Direction.Up);
        INode nodeLeft = new Node(Vector2.left, Direction.Up);
        INode nodeDown = new Node(Vector2.down, Direction.Up);

        IRoad roadLeftFrom = new Road(nodeCenter, nodeLeft);
        IRoad roadLeftTo = new Road(nodeLeft, nodeCenter);
        IRoad roadDownFrom = new Road(nodeCenter, nodeDown);
        IRoad roadDownTo = new Road(nodeDown, nodeCenter);

        ICar carLeft = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1, Speed = 1 };
        ICar carDown = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1, Speed = 1 };

        nodeCenter.Roads.Add(roadLeftFrom);
        nodeCenter.Roads.Add(roadDownFrom);
        nodeLeft.Roads.Add(roadLeftTo);
        nodeDown.Roads.Add(roadDownTo);

        nodeCenter.RoadTo[(int)Direction.Left] = roadLeftTo;
        nodeCenter.RoadFrom[(int)Direction.Left] = roadLeftFrom;
        nodeCenter.RoadTo[(int)Direction.Down] = roadDownTo;
        nodeCenter.RoadFrom[(int)Direction.Down] = roadDownFrom;

        roadLeftTo.AddCar(carLeft);
        roadDownTo.AddCar(carDown);

        carLeft.Init(nodeLeft, nodeDown);

        //Act
        float requiredSlowdown = 0;
        nodeCenter.GetSlowdown(carDown, roadDownTo, roadLeftFrom, carDown.Progress, 0, ref requiredSlowdown);

        //Assert
        Assert.AreEqual(0, requiredSlowdown);
    }

    [Test]
    public void SlowdownTestTCross()
    {
        //Arrange
        World.CarDistance = 1;
        World.WorldScale = 1;
        World.RoadWidth = 1;

        var node = new NodeMainRoad(new Vector2(0, 0), Direction.Up);
        var nodeUp = new Node(new Vector2(0, 1), Direction.Up);
        var nodeDown = new Node(new Vector2(0, -1), Direction.Up);
        var nodeRight = new Node(new Vector2(1, 0), Direction.Up);

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

        node.RoadTo[(int)Direction.Up] = roadUpTo;
        node.RoadFrom[(int)Direction.Up] = roadUpFrom;
        node.RoadTo[(int)Direction.Down] = roadDownTo;
        node.RoadFrom[(int)Direction.Down] = roadDownFrom;
        node.RoadTo[(int)Direction.Right] = roadRightTo;
        node.RoadFrom[(int)Direction.Right] = roadRightFrom;

        var car = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1 };
        var carOther = new Car() { Acceleration = 1, Deceleration = 2, NiceDeceleration = 1 };

        roadUpTo.AddCar(car);
        roadDownTo.AddCar(carOther);
        car.Progress = roadUpTo.Length;
        carOther.Progress = roadDownTo.Length;
        carOther.Init(nodeDown, nodeUp);

        //Act
        float requiredSlowdown = 0;
        node.GetSlowdown(car, roadUpTo, roadRightFrom, 0, 0, ref requiredSlowdown);

        //Assert
        Assert.AreEqual(2, requiredSlowdown);
    }
}
