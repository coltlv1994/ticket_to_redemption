using System.Collections.Generic;
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

    public void AddUIPart(UI_Connection p_uiPart)
    {
        m_uiList.Add(p_uiPart);
    }

    public void BuiltRoad()
    {
        m_isClaimed = true;
        foreach (UI_Connection ui in m_uiList)
        {
            ui.transform.localScale = new Vector3(ui.transform.localScale.x, 1.6f, ui.transform.localScale.z);
        }
    }

    public StationName m_end1, m_end2;

    public int m_totalCost;

    public int m_tunnelCost;

    public int m_boatCost;

    public CardColor m_roadColor;

    public bool m_isClaimed = false;

    public List<UI_Connection> m_uiList = new List<UI_Connection>();
}
