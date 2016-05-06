using System.Collections.Generic;

namespace CTD
{
    public class CTDManager
    {
        public const float SpeedDifferenceRelevance = 0.1F;

        List<IClient> clients = new List<IClient>();
        Dictionary<long, IRoad> roads = new Dictionary<long, IRoad>();

        public void AddClient(IClient client)
        {
            clients.Add(client);
        }

        public void RemoveClient(IClient client)
        {
            clients.Remove(client);
        }

        public IRoad GetRoad(long id)
        {
            return roads[id];
        }

        //Simulation Code
        public void AddRoad(IRoad road)
        {
            roads.Add(road.ID, road);
        }

        public void RemoveRoad(IRoad road)
        {
            roads.Remove(road.ID);
        }
    }
}