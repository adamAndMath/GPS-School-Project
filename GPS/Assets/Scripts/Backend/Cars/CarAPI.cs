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
                client.SendRoadChange(Path[0].ID, Speed);
            }

            public override float TimePoints(IRoad road)
            {
                return World.CTDManager.GetRoad(road.ID).EstimatedSpeed;
            }

            public override float TimeEstimate(INode from, INode to)
            {
                return base.TimeEstimate(from, to) / World.EstimationSpeedLimit;
            }
        }
    }
}