using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    public class Node : MonoBehaviour
    {
        public NodeType nodeType;
        public Road roadPrefab;
        public List<Road> roads;
        public List<Road> controlled;

        public enum NodeType
        {
            ToTheRight, Unconditional, Light, Roundabout
        }

        public bool GetBottleneck(Road from, Road to, float distance, out float speed)
        {
            speed = to.speedLimit;
            switch (nodeType)
            {
                case NodeType.Unconditional:
                    break;
                case NodeType.Light:
                    break;
                case NodeType.Roundabout:
                    break;
            }

            return false;
        }
    }
}