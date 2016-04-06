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
    public void SlowdownTest()
    {
        Assert.Fail("TODO");
    }
}
