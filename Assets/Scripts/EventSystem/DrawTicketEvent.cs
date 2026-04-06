using UnityEngine;

public class DrawTicketEvent : EventBase
{
    public EventType GetEventType()
    {
        return EventType.DRAW_TICKET;
    }
}
