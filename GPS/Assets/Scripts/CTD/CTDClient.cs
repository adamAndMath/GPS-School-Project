using System;

namespace CTD
{
    public class CTDClient : IClient, IDisposable
    {
        private readonly CTDManager Manager;

        public float Speed { get; private set; }
        public IRoad Road { get; private set; }

        public CTDClient(CTDManager manager, long roadID, float speed)
        {
            Manager = manager;
            Manager.AddClient(this);
            Road = Manager.GetRoad(roadID);
        }

        public void SendSpeedChange(float speed)
        {
            Speed = speed;
            Road.OnEstimateChanged();
        }

        public void SendRoadChange(long roadID, float speed)
        {
            Speed = speed;
            Road.RemoveCar(this);
            Road = Manager.GetRoad(roadID);
            Road.AddCar(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Manager.RemoveClient(this);
            Road.RemoveCar(this);
        }
    }
}