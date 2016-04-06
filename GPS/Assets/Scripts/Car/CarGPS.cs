using UnityEngine;
using System.Collections;

namespace Old
{
    public class CarGPS : Car
    {
        protected override float TimePoints(Road road)
        {
            return road.RealLength / road.speedLimit;
        }
    }
}