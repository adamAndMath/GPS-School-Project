using CTD_Sim.Backend;
using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class PathFinderTest
{
    [Test]
    public void SimplePath()
    {
        //Arrange
        var pathfinder = new PathFinder();
        var car = new Car();
        var node0 = new Node(Vector2.zero, Direction.Up);
        var node1 = new Node(Vector2.right, Direction.Up);
        var road0 = new Road(0, node0, node1);
        node0.Roads.Add(road0);

        //Act
        //Try to find a path
        var path = pathfinder.FindPath(car, node0, node1);

        //Assert
        //The path is the road
        CollectionAssert.AreEqual(new IRoad[] { road0 }, path);
    }

    [Test]
    public void Simple5StepPath()
    {
        //Arrange
        var pathfinder = new PathFinder();
        var car = new Car();
        var node0 = new Node(Vector2.zero, Direction.Up);
        var node1 = new Node(Vector2.right, Direction.Up);
        var node2 = new Node(Vector2.right * 2, Direction.Up);
        var node3 = new Node(Vector2.right * 3, Direction.Up);
        var node4 = new Node(Vector2.right * 4, Direction.Up);
        var node5 = new Node(Vector2.right * 5, Direction.Up);
        var road0 = new Road(0, node0, node1);
        var road1 = new Road(1, node1, node2);
        var road2 = new Road(2, node2, node3);
        var road3 = new Road(3, node3, node4);
        var road4 = new Road(4, node4, node5);
        node0.Roads.Add(road0);
        node1.Roads.Add(road1);
        node2.Roads.Add(road2);
        node3.Roads.Add(road3);
        node4.Roads.Add(road4);

        //Act
        //Try to find a path
        var path = pathfinder.FindPath(car, node0, node5);

        //Assert
        //The path is the road
        CollectionAssert.AreEqual(new IRoad[] { road0, road1, road2, road3, road4 }, path);
    }

    [Test]
    public void FailLoopPath()
    {
        //Arrange
        var pathfinder = new PathFinder();
        var car = new Car();
        var node0 = new Node(Vector2.zero, Direction.Up);
        var node1 = new Node(Vector2.right, Direction.Up);
        var node2 = new Node(Vector2.up, Direction.Up);
        var road0 = new Road(0, node0, node1);
        var road1 = new Road(1, node1, node0);
        node0.Roads.Add(road0);
        node1.Roads.Add(road1);

        //Act
        //Assert
        //Try to find path but fail
        Assert.Throws<Exception>(() => pathfinder.FindPath(car, node0, node2));
    }

    [Test]
    public void TwowayPathTest()
    {
        //Arrange
        var pathfinder = new PathFinder();
        var car = new Car();
        var node0 = new Node(new Vector2(0, 0), Direction.Up);
        var node1 = new Node(new Vector2(2, 0), Direction.Up);
        var node2 = new Node(new Vector2(0, 1), Direction.Up);
        var node3 = new Node(new Vector2(1, 1), Direction.Up);

        var road0 = new Road(0, node0, node1);
        var road1 = new Road(1, node0, node2);
        var road2 = new Road(2, node1, node3);
        var road3 = new Road(3, node2, node3);

        node0.Roads.Add(road0);
        node0.Roads.Add(road1);
        node1.Roads.Add(road2);
        node2.Roads.Add(road3);

        //Act
        var path = pathfinder.FindPath(car, node0, node3);

        //Assert
        CollectionAssert.AreEqual(new IRoad[] { road1, road3 }, path);
    }

    [Test]
    public void SomeTest()
    {
        var pathfinder = new PathFinder();
        var car = new Car();

        var node0 = new Node(new Vector2(0, 0), Direction.Up);
        var node1 = new Node(new Vector2(0, 4), Direction.Up);
        var node2 = new Node(new Vector2(-4, 4), Direction.Up);
        var node3 = new Node(new Vector2(-4, 0), Direction.Up);
        var node4 = new Node(new Vector2(-4, 8), Direction.Up);
        var node5 = new Node(new Vector2(4, 0), Direction.Up);
        var node6 = new Node(new Vector2(4, -4), Direction.Up);
        var node7 = new Node(new Vector2(8, -4), Direction.Up);
        var node8 = new Node(new Vector2(8, 8), Direction.Up);
        var node9 = new Node(new Vector2(-8, 8), Direction.Up);
        var node10 = new Node(new Vector2(-8, -4), Direction.Up);
        var node11 = new Node(new Vector2(0, -4), Direction.Up);

        var road0 = new Road(0, node0, node1);
        var road0B = new Road(1, node1, node0);
        node0.Roads.Add(road0);
        node1.Roads.Add(road0B);
        var road1 = new Road(2, node1, node2);
        var road1B = new Road(3, node2, node1);
        node1.Roads.Add(road1);
        node2.Roads.Add(road1B);
        var road2 = new Road(4, node2, node3);
        var road2B = new Road(5, node3, node2);
        node2.Roads.Add(road2);
        node3.Roads.Add(road2B);
        var road3 = new Road(6, node2, node4);
        var road3B = new Road(7, node4, node2);
        node2.Roads.Add(road3);
        node4.Roads.Add(road3B);
        var road4 = new Road(8, node0, node5);
        var road4B = new Road(9, node5, node0);
        node0.Roads.Add(road4);
        node5.Roads.Add(road4B);
        var road5 = new Road(10, node5, node6);
        var road5B = new Road(11, node6, node5);
        node5.Roads.Add(road5);
        node6.Roads.Add(road5B);
        var road6 = new Road(12, node6, node7);
        var road6B = new Road(13, node7, node6);
        node6.Roads.Add(road6);
        node7.Roads.Add(road6B);
        var road7 = new Road(14, node7, node8);
        var road7B = new Road(15, node8, node7);
        node7.Roads.Add(road7);
        node8.Roads.Add(road7B);
        var road8 = new Road(16, node8, node4);
        var road8B = new Road(17, node4, node8);
        node8.Roads.Add(road8);
        node4.Roads.Add(road8B);
        var road9 = new Road(18, node4, node9);
        var road9B = new Road(19, node9, node4);
        node4.Roads.Add(road9);
        node9.Roads.Add(road9B);
        var road10 = new Road(20, node9, node10);
        var road10B = new Road(21, node10, node9);
        node9.Roads.Add(road10);
        node10.Roads.Add(road10B);
        var road11 = new Road(22, node10, node11);
        var road11B = new Road(23, node11, node10);
        node10.Roads.Add(road11);
        node11.Roads.Add(road11B);
        var road12 = new Road(24, node11, node0);
        var road12B = new Road(25, node0, node11);
        node11.Roads.Add(road12);
        node0.Roads.Add(road12B);

        //Arrange
        var path0 = pathfinder.FindPath(car, node5, node9);
        var path1 = pathfinder.FindPath(car, node6, node9);

        //Assert
        Assert.AreEqual(new IRoad[] { road4B, road0, road1, road3, road9 }, path0);
        Assert.AreEqual(new IRoad[] { road5B, road4B, road0, road1, road3, road9 }, path1);
    }
}
