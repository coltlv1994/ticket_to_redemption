using UnityEngine;

// Route is for connecting neighbors
// first two is start and end and road color
// last three is total cost, tunnel and boat
using Route = System.Tuple<StationName, StationName, CardColor, int, int, int>;

public class Connection
{
    public Connection(Route p_route)
    {
        m_end1 = p_route.Item1;
        m_end2 = p_route.Item2;
        m_roadColor = p_route.Item3;
        m_totalCost = p_route.Item4;
        m_tunnelCost = p_route.Item5;
        m_tunnelCost = p_route.Item6;
    }

    public StationName m_end1, m_end2;

    public int m_totalCost;

    public int m_tunnelCost;

    public int m_boatCost;

    public CardColor m_roadColor;

    public bool m_isClaimed = false;
}
