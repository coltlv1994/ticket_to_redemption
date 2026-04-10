using UnityEngine;

//[ExecuteInEditMode]
public class Connection : MonoBehaviour
{
    public void SetRoadColor(CardColor color)
    {
        m_roadColor = color;
    }

    [SerializeField]
    public StationName m_end1, m_end2;

    [SerializeField]
    public int m_totalCost;

    [SerializeField]
    public int m_tunnelCost;

    [SerializeField]
    public int m_boatCost;

    [SerializeField]
    public CardColor m_roadColor;

    public bool m_isClaimed = false;
}
