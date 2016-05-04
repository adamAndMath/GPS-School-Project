using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CTD_Sim
{
    namespace Backend
    {
        public class PathFinder : IPathFinder
        {
            private class OpenPath
            {
                public OpenPath parent;
                public IRoad road;
                public INode node;
                public float timeTo;
                public float timeEstimate;

                public float T { get { return timeTo + timeEstimate; } }

                public OpenPath(ICar car, INode node, INode goal)
                {
                    this.node = node;
                    timeEstimate = car.TimeEstimate(node, goal);
                }

                public OpenPath(OpenPath parent, ICar car, IRoad road, float timeTo, INode goal)
                    : this(car, road.To, goal)
                {
                    this.parent = parent;
                    this.road = road;
                    this.timeTo = timeTo;
                }
            }

            public List<IRoad> FindPath(ICar car, INode from, INode to)
            {
                var open = new List<OpenPath>();
                var closed = new List<INode>();
                OpenPath current;

                open.Add(new OpenPath(car, from, to));

                do
                {
                    if (open.Count == 0)
                        throw new Exception(string.Format("No Path from {0} to {1}", from.Position, to.Position));

                    current = Closed(car, open, closed, to);
                }
                while (current.node != to);

                var path = new List<IRoad>();

                while (current != null && current.parent != null)
                {
                    path.Add(current.road);
                    current = current.parent;
                }

                path.Reverse();

                return path;
            }

            private OpenPath Closed(ICar car, List<OpenPath> open, List<INode> closed, INode goal)
            {
                var close = open[0];
                open.Remove(close);
                closed.Add(close.node);

                foreach (var road in close.node.Roads)
                {
                    Open(car, open, closed, goal, close, road, close.timeTo + car.TimePoints(road));
                }

                return close;
            }

            private void Open(ICar car, List<OpenPath> open, List<INode> closed, INode goal, OpenPath parent, IRoad road, float timeTo)
            {
                if (closed.Contains(road.To)) return;

                var old = open.FirstOrDefault(o => o.node == road.To);

                if (old != null)
                {
                    if (old.timeTo > timeTo)
                    {
                        var oldIndex = open.IndexOf(old);
                        old.timeTo = timeTo;
                        old.parent = parent;
                        old.road = road;

                        if (oldIndex != 0 && open[oldIndex - 1].T >= old.T) return;

                        open.RemoveAt(oldIndex);

                        int newIndex = open.FindIndex(o => o.T > old.T);

                        if (newIndex == -1)
                            newIndex = 0;

                        open.Insert(newIndex, old);
                    }

                    return;
                }

                var op = new OpenPath(parent, car, road, timeTo, goal);
                var index = open.FindIndex(o => o.T > op.T);

                if (index == -1)
                    open.Add(op);
                else
                    open.Insert(index, op);
            }
        }
    }
}