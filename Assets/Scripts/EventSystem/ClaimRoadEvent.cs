using UnityEngine;

public class ClaimRoadEvent : EventBase
{
    public EventType GetEventType()
    {
        return EventType.CLAIM_ROAD;
    }

    public ClaimRoadEvent(Connection p_roadToClaim)
    {
        m_roadToClaim = p_roadToClaim;
    }

    public Connection GetRoadToClaim()
    {
        return m_roadToClaim;
    }

    private Connection m_roadToClaim;
}
