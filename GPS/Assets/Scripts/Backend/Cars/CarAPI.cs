using System.Collections.Generic;

namespace CTD_Sim
{
    namespace Backend
    {
        public class CarAPI : Car
        {
            private CTD.CTDClient client;

            public override float Speed
            {
                get { return base.Speed; }
                set
                {
                    base.Speed = value;

                    if (client != null)
                        client.SendSpeedChange(value);
                }
            }

            public override void Init(INode from, INode to)
            {
                base.Init(from, to);
                client = new CTD.CTDClient(World.CTDManager, Path[0].ID, Speed);
            }

            public override void Return()
            {
                client.Dispose();
            }

            public override void OnRoadChanged()
            {
                World.CTDManager.GetRoad(Path[0].ID).EstimateDecrease -= OnSpeedEstimateDecrease;
                client.SendRoadChange(Path[0].ID, Speed);
            }

            public void OnPathChanged()
            {
                foreach (IRoad road in Path)
                {
                    World.CTDManager.GetRoad(road.ID).EstimateDecrease += OnSpeedEstimateDecrease;
                }

                World.CTDManager.GetRoad(Path[0].ID).EstimateDecrease -= OnSpeedEstimateDecrease;
            }

            private void OnSpeedEstimateDecrease(CTD.IRoad road, float estimate)
            {
                foreach (IRoad r in Path)
                {
                    World.CTDManager.GetRoad(r.ID).EstimateDecrease -= OnSpeedEstimateDecrease;
                }

                List<IRoad> newPath = World.Pathfinder.FindPath(this, Path[0].To, To);
                newPath.Insert(0, Path[0]);
                Path = newPath;
                OnPathChanged();
            }

            public override float TimePoints(IRoad road)
            {
                return road.Length / World.CTDManager.GetRoad(road.ID).EstimatedSpeed;
            }

            public override float TimeEstimate(INode from, INode to)
            {
                return base.TimeEstimate(from, to) / World.EstimationSpeedLimit;
            }
        }
    }
}