using System.Collections.Generic;

namespace Backend
{
    public interface IPathFinder
    {
        List<IRoad> FindPath(ICar car, INode from, INode to);
    }
}