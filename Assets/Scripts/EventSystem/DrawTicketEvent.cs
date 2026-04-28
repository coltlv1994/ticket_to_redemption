using UnityEngine;

public class DrawTicketEvent : EventBase
{
    public EventType GetEventType()
    {
        return EventType.DRAW_TICKET;
    }

    public DrawTicketEvent(int p_numOfTicketsToDraw = 1, int p_minimumNumberToKeep = 0) : base()
    {
        m_numOfTicketsToDraw = p_numOfTicketsToDraw;
        m_minimumNumberToKeep = p_minimumNumberToKeep;
    }


    public int GetNumberOfTicketsToDraw()
    {
        return m_numOfTicketsToDraw;
    }

    public int GetMinimumNumberToKeep()
    {
        return m_minimumNumberToKeep;
    }

    private int m_numOfTicketsToDraw;
    private int m_minimumNumberToKeep;
}
