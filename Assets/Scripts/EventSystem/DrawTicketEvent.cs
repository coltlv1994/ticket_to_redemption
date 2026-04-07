using UnityEngine;

public class DrawTicketEvent : EventBase
{
    public EventType GetEventType()
    {
        return EventType.DRAW_TICKET;
    }

    public DrawTicketEvent(TicketType p_ticketType = TicketType.NORMAL, int p_numOfTicketsToDraw = 3) : base()
    {
        m_ticketType = p_ticketType;
        m_numOfTicketsToDraw = p_numOfTicketsToDraw;
    }

    public TicketType GetTicketType()
    {
        return m_ticketType;
    }

    public int GetNumberOfTicketsToDraw()
    {
        return m_numOfTicketsToDraw;
    }

    private TicketType m_ticketType;
    private int m_numOfTicketsToDraw;
}
