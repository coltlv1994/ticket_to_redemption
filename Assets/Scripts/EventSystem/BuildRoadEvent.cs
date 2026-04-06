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

    private Connection m_roadToBuild;
}
