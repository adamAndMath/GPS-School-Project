using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

            public OpenPath(OpenPath parent, ICar car, IRoad road, float timeTo, INode goal) : this(car, road.To, goal)
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

            for (var i = 0; i < open.Count; i++)
            {
                if (open[i].node == road.To)
                {
                    if (open[i].timeTo > timeTo)
                    {
                        open[i].timeTo = timeTo;
                        open[i].parent = parent;
                        open[i].road = road;
                        var o = open[i];

                        if (i != 0 && open[i - 1].T >= o.T) return;
                        
                        open.Remove(o);

                        for (--i; i >= 0; i--)
                        {
                            if (open[i].T > o.T)
                            {
                                open.Insert(i + 1, o);
                                return;
                            }
                        }
                    }

                    return;
                }
            }

            var op = new OpenPath(parent, car, road, timeTo, goal);

            for (var i = 0; i < open.Count; i++)
            {
                if (open[i].T > op.T)
                {
                    open.Insert(i, op);

                    return;
                }
            }

            open.Add(op);
        }
    }
}