using UnityEngine;

public class DrawCardEvent : EventBase
{
    EventType EventBase.GetEventType()
    {
        return EventType.DRAW_CARD;
    }


}
