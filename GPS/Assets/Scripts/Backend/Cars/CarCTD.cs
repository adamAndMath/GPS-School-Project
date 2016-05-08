using System.Collections.Generic;

namespace CTD_Sim.Backend
{
    public class CarCTD : Car
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
            World.GetCTDRoad(Path[0]).EstimateDecrease -= OnSpeedEstimateDecrease;
            client.SendRoadChange(Path[0].ID, Speed);
        }

        public void OnPathChanged()
        {
            foreach (IRoad road in Path)
            {
                World.GetCTDRoad(road).EstimateDecrease += OnSpeedEstimateDecrease;
            }

            World.GetCTDRoad(Path[0]).EstimateDecrease -= OnSpeedEstimateDecrease;
        }

        private void OnSpeedEstimateDecrease(CTD.IRoad road, float estimate)
        {
            foreach (IRoad path in Path)
            {
                World.GetCTDRoad(path).EstimateDecrease -= OnSpeedEstimateDecrease;
            }

            List<IRoad> newPath = World.Pathfinder.FindPath(this, Path[0].To, To);
            newPath.Insert(0, Path[0]);
            Path = newPath;
            OnPathChanged();
        }

        public override float TimePoints(IRoad road)
        {
            CTD.IRoad ctdRoad = World.GetCTDRoad(road);
            return road.Length / ctdRoad.EstimatedSpeed;
        }

        public override float TimeEstimate(INode from, INode to)
        {
            return base.TimeEstimate(from, to) / World.EstimationSpeedLimit;
        }
    }
}