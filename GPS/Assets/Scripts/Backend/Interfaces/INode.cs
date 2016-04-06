using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public interface INode
    {
        Vector2 Position { get; set; }

        List<IRoad> Roads { get; }

        void GetSlowdown(ICar car, IRoad from, IRoad to, float progress, int index, ref float requiredSlowdown);
    }
}