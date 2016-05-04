using System.Collections.Generic;

namespace CTD_Sim
{
    namespace Backend
    {
        public interface IRoad
        {
            INode From { get; }
            INode To { get; }
            float SpeedLimit { get; }
            float RealLength { get; }
            float Length { get; }

            IEnumerable<ICar> Cars { get; }
            int CarCount { get; }
            ICar this[int i] { get; }

            void AddCar(ICar car);
            void RemoveCar(ICar car);
            int IndexOfCar(ICar car);

            void GetSlowdown(ICar car, float progress, int preIndex, int roadIndex, ref float requiredSlowdown);
        }
    }
}