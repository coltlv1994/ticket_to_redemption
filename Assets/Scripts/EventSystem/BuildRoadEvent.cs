using UnityEngine;

public class BuildRoadEvent : EventBase
{
    public EventType GetEventType()
    {
        return EventType.BUILD_ROAD;
    }

    public Connection GetRoadToBuild()
    {
        return m_roadToBuild;
    }

    public BuildRoadEvent(Connection p_roadToBuild)
    {
        m_roadToBuild = p_roadToBuild;
    }

    private Connection m_roadToBuild;
}
