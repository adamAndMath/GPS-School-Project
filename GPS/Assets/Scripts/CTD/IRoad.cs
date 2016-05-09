using System.Collections.Generic;

namespace CTD
{
    public delegate void RoadChangeHandler(long roadID, float estimate);

    public interface IRoad
    {
        long ID { get; }
        float SpeedLimit { get; }
        List<IClient> Cars { get; }
        float EstimatedSpeed { get; }

        event RoadChangeHandler EstimateDecrease;
        event RoadChangeHandler EstimateIncrease;

        void AddCar(IClient car, float time);
        void RemoveCar(IClient car, float time);

        void OnEstimateChanged(float time);
    }
}