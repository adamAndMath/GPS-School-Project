using System.Collections.Generic;

namespace CTD
{
    public delegate void RoadChangeHandler(IRoad sender, float estimate);

    public interface IRoad
    {
        long ID { get; }
        float SpeedLimit { get; }
        List<IClient> Cars { get; }
        float EstimatedSpeed { get; }

        event RoadChangeHandler EstimateDecrease;
        event RoadChangeHandler EstimateIncrease;

        void AddCar(IClient car);
        void RemoveCar(IClient car);

        void OnEstimateChanged();
    }
}