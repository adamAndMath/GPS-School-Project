using System.Collections;

namespace CTD
{
    public interface IClient
    {
        float Speed { get; }
        IRoad Road { get; }

        event RoadChangeHandler RoadEstimationDecrease;
        event RoadChangeHandler RoadEstimationIncrease;

        void ListenToRoadDecrease(long roadID);
        void MuteRoadDecrease(long roadID);
        void ListenToRoadIncrease(long roadID);
        void MuteRoadIncrease(long roadID);
        float GetEstimatedSpeed(long roadID);
    }
}