﻿using System.Collections.Generic;

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
            client.RoadEstimationDecrease += OnSpeedEstimateDecrease;
        }

        public override void Return()
        {
            client.Dispose();
        }

        public override void OnRoadChanged()
        {
            client.MuteRoadDecrease(Path[0].ID);
            client.SendRoadChange(Path[0].ID, Speed);
        }

        public void OnPathChanged()
        {
            foreach (IRoad path in Path)
            {
                client.ListenToRoadDecrease(path.ID);
            }

            client.MuteRoadDecrease(Path[0].ID);
        }

        private void OnSpeedEstimateDecrease(long roadID, float estimate)
        {
            foreach (IRoad path in Path)
            {
                client.MuteRoadDecrease(path.ID);
            }

            List<IRoad> newPath = World.Pathfinder.FindPath(this, Path[0].To, To);
            newPath.Insert(0, Path[0]);
            Path = newPath;
            OnPathChanged();
        }

        public override float TimePoints(IRoad road)
        {
            return road.Length / client.GetEstimatedSpeed(road.ID);
        }

        public override float TimeEstimate(INode from, INode to)
        {
            return base.TimeEstimate(from, to) / World.EstimationSpeedLimit;
        }
    }
}