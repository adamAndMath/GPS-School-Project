﻿using CTD;
using System;
using UnityEngine;

namespace CTD_Sim.Backend
{
    public class CTDClient : IClient, IDisposable
    {
        private readonly CTDManager Manager;

        public float Speed { get; private set; }
        public CTD.IRoad Road { get; private set; }

        public event RoadChangeHandler RoadEstimationDecrease;
        public event RoadChangeHandler RoadEstimationIncrease;

        public CTDClient(CTDManager manager)
        {
            Manager = manager;
            Manager.AddClient(this);
        }

        public void ListenToRoadDecrease(long roadID)
        {
            Manager.GetRoad(roadID).EstimateDecrease += OnRoadEstimationDecrease;
        }

        public void MuteRoadDecrease(long roadID)
        {
            Manager.GetRoad(roadID).EstimateDecrease -= OnRoadEstimationDecrease;
        }

        public void ListenToRoadIncrease(long roadID)
        {
            Manager.GetRoad(roadID).EstimateIncrease += OnRoadEstimationIncrease;
        }

        public void MuteRoadIncrease(long roadID)
        {
            Manager.GetRoad(roadID).EstimateIncrease -= OnRoadEstimationIncrease;
        }

        public float GetEstimatedSpeed(long roadID)
        {
            return Manager.GetRoad(roadID).EstimatedSpeed;
        }

        void OnRoadEstimationDecrease(long roadID, float estimate)
        {
            if (RoadEstimationDecrease != null)
                RoadEstimationDecrease(roadID, estimate);
        }

        void OnRoadEstimationIncrease(long roadID, float estimate)
        {
            if (RoadEstimationIncrease != null)
                RoadEstimationIncrease(roadID, estimate);
        }

        public void SendSpeedChange(float speed)
        {
            Speed = speed;

            if (Road != null)
                Road.OnEstimateChanged(Time.time);
        }

        public void SendRoadChange(long roadID, float speed)
        {
            Speed = speed;

            if (Road != null)
                Road.RemoveCar(this, Time.time);

            Road = Manager.GetRoad(roadID);
            Road.AddCar(this, Time.time);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Manager.RemoveClient(this);
            Road.RemoveCar(this, Time.time);
        }
    }
}