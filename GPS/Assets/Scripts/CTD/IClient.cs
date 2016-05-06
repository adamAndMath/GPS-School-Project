using System.Collections;

namespace CTD
{
    public interface IClient
    {
        float Speed { get; }
        IRoad Road { get; }
    }
}