using CTD;
using System;

namespace CTD_Sim
{
    namespace Backend
    {
        public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3 }

        public class World
        {
            private static World instance;

            public static World Instance
            {
                get
                {
                    if (instance == null)
                        instance = new World();

                    return instance;
                }
            }

            private readonly IPathFinder pathfinder = new PathFinder();
            private readonly CTDManager manager = new CTDManager();
            private float worldScale = 1;
            private float timeScale = 1;
            private float roadWidth = 4;
            private float estimationSpeedLimit = 1;
            private int viewDistance = 100;
            private float carDistance = 2;
            private int lookForward = 3;

            public static IPathFinder Pathfinder { get { return Instance.pathfinder; } }
            public static CTDManager CTDManager { get { return Instance.manager; } }
            public static float WorldScale { get { return Instance.worldScale; } set { Instance.worldScale = value; } }
            public static float TimeScale { get { return Instance.timeScale; } set { Instance.timeScale = value; } }
            public static float RoadWidth { get { return Instance.roadWidth; } set { Instance.roadWidth = value; } }
            public static float RealRoadWidth { get { return RoadWidth / WorldScale; } }
            public static float EstimationSpeedLimit { get { return Instance.estimationSpeedLimit; } set { Instance.estimationSpeedLimit = value; } }
            public static int ViewDistance { get { return Instance.viewDistance; } set { Instance.viewDistance = value; } }
            public static float CarDistance { get { return Instance.carDistance; } set { Instance.carDistance = value; } }
            public static int LookForward { get { return Instance.lookForward; } set { Instance.lookForward = value; } }
        }
    }
}