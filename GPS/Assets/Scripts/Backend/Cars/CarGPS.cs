using UnityEngine;
using System.Collections;

namespace CTD_Sim
{
    namespace Backend
    {
        public class CarGPS : Car
        {
            public override float TimePoints(IRoad road)
            {
                return road.Length / road.SpeedLimit;
            }

            public override float TimeEstimate(INode from, INode to)
            {
                return base.TimeEstimate(from, to) / World.EstimationSpeedLimit;
            }
        }
    }
}