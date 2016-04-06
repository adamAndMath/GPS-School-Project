using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class Node : INode
    {
        private readonly List<IRoad> road = new List<IRoad>();

        public Vector2 Position { get; set; }
        public List<IRoad> Roads { get { return road; } }

        public Node(Vector2 position)
        {
            Position = position;
        }

        public virtual void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown)
        {

        }

        public override string ToString()
        {
            return Position.ToString();
        }
    }
}