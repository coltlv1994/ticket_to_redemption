using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Route is for connecting neighbors
// first two is start and end
// last three is total cost, tunnel and boat
using Route = System.Tuple<StationName, StationName, int, int, int>;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

enum CamDirection
{
    Forward,
    Backward,
    Left,
    Right,
    None
}

public class Player : NetworkBehaviour
{
    public bool BuildRoute(Route p_route)
    {
        int cost = p_route.Item3;
        return true;
    }

    public int GetHandCardCount(CardColor p_color)
    {
        return m_handDeck.GetCardCount(p_color);
    }

    public void AddHandCard(CardColor p_color, int p_numOfNewCards = 1)
    {
        m_handDeck.AddCard(p_color, p_numOfNewCards);
    }

    public bool RemoveHandCard(CardColor p_color, int p_numOfCardsToRemove = 1)
    {
        return m_handDeck.ChechAndUseCard(p_color, p_numOfCardsToRemove);
    }

    public void ResetHandDeck()
    {
        m_handDeck.ResetHandDeck();
    }

    public bool CheckEventQueue()
    {
        // false means no event at hand
        if (m_eventQueue.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddEvent(EventBase newEvent)
    {
        m_eventQueue.Enqueue(newEvent);
    }

    public void HandleEvent()
    {
        if (m_eventQueue.Count != 0)
        {
            // do some stuff like switch-case
            // need semaphore protection
            EventBase nextEvent = m_eventQueue.Dequeue();
            
            EventType eventType = nextEvent.GetEventType();
            switch (eventType)
            {
                case EventType.DRAW_CARD:
                    // do something
                    // logic needs update: player can pick card from desk or randomly from deck
                    DrawCardEvent drawCardEvent = (DrawCardEvent)nextEvent;
                    int drawCardNum = drawCardEvent.GetNumberOfCardsToDraw();
                    GameDataCollection gdc = GameDataCollection.GetInstance();
                    for (int i = 0; i < drawCardNum; i++)
                    {
                        m_handDeck.AddCard(gdc.GetRandomCard());
                    }
                    break;
                case EventType.BUILD_ROAD:
                    // do something
                    break;
                case EventType.CLAIM_ROUTE:
                    // do something
                    break;
                case EventType.DISCARD_CARD:
                    // do something
                    break;
                case EventType.DRAW_TICKET:
                    // do something
                    break;
                case EventType.END_TURN:
                    // do something
                    break;
            }
        }
    }

    public void OnUpdate()
    {
        // event handling
        HandleEvent();

        m_mainSceneUIManager.SyncCardCount(m_handDeck.GetAllCardCounts());
    }

    public bool IsRouteBuildable(Connection p_route)
    {
        BuildRoadEvent buildRouteEvent = new BuildRoadEvent(p_route);
        bool canBuildRoute = false;
        // pop out UI to ask if player want to build route here
        PlayerState ps = PlayerController.GetInstance().GetPlayerState();
        Substate ss = PlayerController.GetInstance().GetSubState();

        if (ps == PlayerState.Play && ss == Substate.Turn)
        {
            if (p_route.m_isClaimed == false)
            {
                if (m_handDeck.IsRainbowSufficient(p_route.m_boatCost))
                {
                    if (p_route.m_roadColor == CardColor.RAINBOW)
                    {
                        // find the largest number of same color cards (including RAINBOW) in player's hand
                        canBuildRoute = m_handDeck.IsAnyColorSufficient(p_route.m_totalCost);
                    }
                    else
                    {
                        canBuildRoute = m_handDeck.IsOneColorSufficient(p_route.m_roadColor, p_route.m_totalCost);
                    }
                }

            }
        }

        return canBuildRoute;

    }

    private HashSet<StationName> m_connectedStations;
    private int m_remainingCarts = 45;
    private HandDeck m_handDeck = new HandDeck();

    // Event
    private Queue<EventBase> m_eventQueue = new Queue<EventBase>();


    // main scene ui control
    [SerializeField] private MainSceneUIManager m_mainSceneUIManager;
}
