using UnityEngine;

public class DrawCardEvent : EventBase
{
    EventType EventBase.GetEventType()
    {
        return EventType.DRAW_CARD;
    }

    public DrawCardEvent(int p_numOfCardsToDraw = 1) : base()
    {
        m_numOfCardsToDraw = p_numOfCardsToDraw;
    }

    public int GetNumberOfCardsToDraw()
    {
        return m_numOfCardsToDraw;
    }

    private int m_numOfCardsToDraw;
}
