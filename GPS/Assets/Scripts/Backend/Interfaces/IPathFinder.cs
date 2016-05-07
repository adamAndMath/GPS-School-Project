using System.Collections.Generic;

namespace CTD_Sim.Backend
{
    public interface IPathFinder
    {
        List<IRoad> FindPath(ICar car, INode from, INode to);
    }
}