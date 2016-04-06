using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Old
{
    public class Car : MonoBehaviour
    {
        public string identifier;
        public float acceleration;
        public float deceleration;
        public float niceDeceleration;

        public Node from;
        public Node to;

        public float speed;
        public float pro;
        public List<Road> path;

        private class OpenPath
        {
            public OpenPath parent;
            public Road road;
            public Node node;
            public float timeTo;
            public float timeEstimate;

            public float T { get { return timeTo + timeEstimate; } }

            public OpenPath(Node node, Node goal)
            {
                this.node = node;
                timeEstimate = (goal.transform.position - node.transform.position).magnitude;
            }

            public OpenPath(OpenPath parent, Road road, float timeTo, Node goal)
                : this(road.to, goal)
            {
                this.parent = parent;
                this.road = road;
                this.timeTo = timeTo;
            }
        }

        void OnEnable()
        {
            if (from == null) return;
            path = FindPath(from, to);
            transform.position = from.transform.position;
            path[0].cars.Add(this);
        }

        void Update()
        {
            if (path.Count == 0) return;

            UpdateSpeedAndProgress();

            var fromPos = path[0].from.transform.position;
            var toPos = path[0].to.transform.position;
            transform.position = Vector3.Lerp(fromPos, toPos, pro / path[0].Length) + new Vector3(toPos.y - fromPos.y, fromPos.x - toPos.x) / (path[0].RealLength * 4);

            if (pro > path[0].Length)
            {
                pro = 0;
                path[0].cars.Remove(this);
                path.RemoveAt(0);

                if (path.Count == 0)
                    World.Instance.ReturnCar(this);
                else
                    path[0].cars.Add(this);
            }
        }

        private void UpdateSpeedAndProgress()
        {
            var roadIndex = path[0].cars.IndexOf(this);
            var deltaTime = Time.deltaTime * World.Instance.timeScale;
            var speedLimit = path[0].speedLimit;

            float requiredSlowdown = 0;
            float progress = pro;

            path[0].GetSlowdown(this, progress, 0, roadIndex, ref requiredSlowdown);

            for (var i = 1; i < path.Count; i++)
            {
                progress -= path[i - 1].Length;
                path[i].GetSlowdown(this, progress, roadIndex, path[i].cars.Count, ref requiredSlowdown);
                roadIndex += path[i].cars.Count;
            }

            if (requiredSlowdown > niceDeceleration)
            {
                requiredSlowdown = Mathf.Min(requiredSlowdown, deceleration);

                var timeToZero = speed / requiredSlowdown;

                if (deltaTime < timeToZero)
                {
                    pro += speed * deltaTime - requiredSlowdown * Mathf.Pow(deltaTime, 2);
                    speed -= deltaTime * requiredSlowdown;
                }
                else
                {
                    pro += speed * timeToZero - requiredSlowdown * Mathf.Pow(timeToZero, 2);
                    speed = 0;
                }
            }
            else
            {
                var timeToLimit = (speedLimit - speed) / acceleration;

                if (deltaTime < timeToLimit)
                {
                    pro += speed * deltaTime + acceleration * Mathf.Pow(deltaTime, 2);
                    speed += deltaTime * acceleration;
                }
                else
                {
                    pro += speed * timeToLimit + acceleration * Mathf.Pow(timeToLimit, 2) + speedLimit * (deltaTime - timeToLimit);
                    speed = speedLimit;
                }
            }
        }

        protected virtual float TimePoints(Road road)
        {
            return road.RealLength;
        }

        private List<Road> FindPath(Node from, Node to)
        {
            var open = new List<OpenPath>();
            var closed = new List<Node>();
            OpenPath current;

            open.Add(new OpenPath(from, to));

            do
            {
                current = Closed(open, closed, to);
            }
            while (current.node != to && open.Count > 0);

            var path = new List<Road>();

            while (current != null && current.parent != null)
            {
                path.Add(current.road);
                current = current.parent;
            }

            path.Reverse();

            return path;
        }

        private OpenPath Closed(List<OpenPath> open, List<Node> closed, Node goal)
        {
            var close = open[0];
            open.Remove(close);

            foreach (var road in close.node.roads)
            {
                Open(open, closed, goal, close, road, close.timeTo + TimePoints(road));
            }

            return close;
        }

        private void Open(List<OpenPath> open, List<Node> closed, Node goal, OpenPath parent, Road road, float timeTo)
        {
            if (closed.Contains(road.to)) return;

            for (var i = 0; i < open.Count; i++)
            {
                if (open[i].node == road.to)
                {
                    if (open[i].timeTo > timeTo)
                    {
                        open[i].timeTo = timeTo;
                        open[i].parent = parent;
                        open[i].road = road;
                        var o = open[i];
                        open.Remove(o);

                        if (i == open.Count && open[i - 1].T < o.T)
                        {
                            open.Add(o);
                            return;
                        }

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

            var op = new OpenPath(parent, road, timeTo, goal);

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

        void OnGUI()
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y;

            GUI.Label(new Rect(pos, Vector2.one * 32), gameObject.name);
            //GUI.Label(new Rect(pos, Vector2.one * 32), path[0].cars.IndexOf(this).ToString());
        }
    }
}