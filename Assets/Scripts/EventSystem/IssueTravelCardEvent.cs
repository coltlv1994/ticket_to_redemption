using UnityEngine;

public class IssueTravelCardEvent : EventBase
{
    public EventType GetEventType()
    {
        return EventType.ISSUE_TRAVEL;
    }

    public IssueTravelCardEvent(TravelTicket p_travelTicket) : base()
    {
        m_travelTicket = p_travelTicket;
    }

    public TravelTicket GetTravelTicket()
    {
        return m_travelTicket;
    }

    private TravelTicket m_travelTicket;
}
