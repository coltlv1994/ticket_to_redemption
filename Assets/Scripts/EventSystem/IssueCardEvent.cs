using UnityEngine;

public class IssueCardEvent : EventBase
{
    public EventType GetEventType()
    {
        return EventType.ISSUE_CARD;
    }

    public IssueCardEvent(CardColor p_color, int p_number = 1) : base()
    {
        m_color = p_color;
        m_number = p_number;
    }

    public CardColor GetCardColor()
    {
        return m_color;
    }

    public int GetCardNumber()
    {
        return m_number;
    }

    private CardColor m_color;
    private int m_number;
}
