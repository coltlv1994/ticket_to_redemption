using System;
using System.Collections.Generic;
using UnityEngine;

public enum CardColor
{
    PINK,
    WHITE,
    BLUE,
    YELLOW,
    ORANGE,
    BLACK,
    RED,
    GREEN,
    RAINBOW // it means any color when mentioned in road/Connection class
}

public enum StationName
{
    COLTER,
    ISABELLA,
    WAPITI,
    BRANDWINE,
    BUCCHUS,
    OCREAGB,
    ANNESBURG,
    PRONGHORN,
    WALLACE,
    VALENTINE,
    OIL_FIELD,
    EMERALD,
    BUTCHER,
    VAN_HORN,
    OWANJILA,
    STRAWBERRY,
    RIGGS,
    FLATNECK,
    HEARTLAND,
    LAGRAS,
    PRISON,
    BEECHER,
    BLACKWATER,
    RHODES,
    ST_DENIS,
    COUJAR,
    TUMBLEWEED,
    BENEDICT,
    MERCER,
    DON_JILA,
    ARMADILLO,
    MACFARLANE,
    THIEVES,
    DAKOTA,
    CALIGA,
    MAX_NUM_OF_STATIONS
}

public enum EventType
{
    DRAW_CARD,
    BUILD_ROAD,
    CLAIM_ROUTE,
    DISCARD_CARD,
    DRAW_TICKET,
    END_TURN
}

public enum TicketType
{
    NORMAL,
    LONG
}

public class Node
{
    public Node(StationName stationName)
    {
        this.stationName = stationName;
        neighbors = new Dictionary<StationName, int>();
        builtRoads = new Dictionary<StationName, int>();
    }

    public void AddNeighbor(StationName station, int cost, int tunnel, int boat)
    {
        if (neighbors.ContainsKey(station))
        {
            neighbors[station] = cost;
        }
        else
        {
            neighbors.Add(station, cost);
        }
    }

    public Dictionary<StationName, int> GetNeighborList()
    {
        return neighbors;
    }

    public bool BuildRoad(StationName station, int playerID)
    {
        if (neighbors.ContainsKey(station))
        {
            if (!builtRoads.ContainsKey(station))
            {
                builtRoads.Add(station, playerID);
                return true;
            }
        }
        return false;
    }

    public void ResetNode()
    {
        builtRoads.Clear();
    }

    public Vector3 GetPosition()
    {
        return positionOnMap;
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        positionOnMap = newPosition;
    }

    private StationName stationName;
    private Dictionary<StationName, int> neighbors;
    private Dictionary<StationName, int> builtRoads; // any connection that has been built; int is for player ID

    private Vector3 positionOnMap;
}

public class HandDeck
{
    // per player hand deck
    public HandDeck()
    {
        m_cardCounts = new Dictionary<CardColor, int>();
        m_cardCounts.Add(CardColor.RAINBOW, 0);
        m_cardCounts.Add(CardColor.RED, 0);
        m_cardCounts.Add(CardColor.GREEN, 0);
        m_cardCounts.Add(CardColor.BLUE, 0);
        m_cardCounts.Add(CardColor.BLACK, 0);
        m_cardCounts.Add(CardColor.YELLOW, 0);
    }

    public void ResetHandDeck()
    {
        m_cardCounts[CardColor.RAINBOW] = 0;
        m_cardCounts[CardColor.RED] = 0;
        m_cardCounts[CardColor.GREEN] = 0;
        m_cardCounts[CardColor.BLUE] = 0;
        m_cardCounts[CardColor.BLACK] = 0;
        m_cardCounts[CardColor.YELLOW] = 0;
    }

    public void AddCard(CardColor color, int numOfNewCards = 1)
    {
        m_cardCounts[color] += numOfNewCards;
    }

    public bool CheckCardSufficient(CardColor color, int count = 1)
    {
        return m_cardCounts[color] >= count;
    }

    public bool ChechAndUseCard(CardColor color, int count = 1)
    {
        // if card is sufficient, use the card and return true; otherwise return false and data is untouched
        if (CheckCardSufficient(color, count))
        {
            m_cardCounts[color] -= count;
            return true;
        }
        return false;
    }

    public int GetCardCount(CardColor color)
    {
        return m_cardCounts[color];
    }

    public bool IsOneColorSufficient(CardColor color, int count = 1)
    {
        // check if one color of card is sufficient to use, which means rainbow card can be used as any color
        if (color == CardColor.RAINBOW)
        {
            return m_cardCounts[CardColor.RAINBOW] >= count;
        }
        else
        {
            return m_cardCounts[color] + m_cardCounts[CardColor.RAINBOW] >= count;
        }
    }

    public bool IsAnyColorSufficient(int count = 1)
    {
        int rainbowCount = m_cardCounts[CardColor.RAINBOW];
        if (rainbowCount >= count)
        {
            return true;
        }

        // check if any color of card is sufficient to use, which means rainbow card can be used as any color
        foreach (KeyValuePair<CardColor, int> entry in m_cardCounts)
        {
            if (entry.Key != CardColor.RAINBOW && entry.Value + rainbowCount >= count)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsRainbowSufficient(int count = 1)
    {
        return m_cardCounts[CardColor.RAINBOW] >= count;
    }

    private Dictionary<CardColor, int> m_cardCounts;
}

public class TrainTicket
{
    public TrainTicket(StationName start, StationName end, int pointValue)
    {
        startStation = start;
        endStation = end;
        m_pointValue = pointValue;
    }

    public List<StationName> GetStations()
    {
        List<StationName> stations = new List<StationName>();
        stations.Add(startStation);
        if (intermediateStop != StationName.MAX_NUM_OF_STATIONS)
        {
            stations.Add(intermediateStop);
        }
        stations.Add(endStation);
        return stations;
    }

    private StationName startStation;
    private StationName endStation;
    private StationName intermediateStop = StationName.MAX_NUM_OF_STATIONS; // if there is an intermediate stop, otherwise it will be MAX_NUM_OF_STATIONS
    private int m_stopCount = 2;
    private int m_pointValue = 0;
}