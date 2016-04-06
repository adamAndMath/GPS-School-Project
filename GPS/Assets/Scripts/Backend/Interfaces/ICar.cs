﻿using System.Collections.Generic;

namespace Backend
{
    public interface ICar
    {
        bool Complete { get; }
        float Acceleration { get; set; }
        float Deceleration { get; set; }
        float NiceDeceleration { get; set; }
        List<IRoad> Path { get; }
        float Progress { get; set; }
        float Speed { get; set; }
        INode From { get; }
        INode To { get; }

        void Init(INode from, INode to);
        void UpdateSpeedAndProgress(float deltaTime);
        float RequiredDecceleration(float speedTo, float distance);
        float SafeDistance(float speed);
        float TimeTo(float distance);
        float TimePoints(IRoad road);
        float TimeEstimate(INode from, INode to);
    }
}