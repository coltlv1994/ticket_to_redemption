using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapData
{
    // MapData will remain the same in EACH run of game

    public enum CardColor
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        BLACK,
        RAINBOW
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
        TUMNBLEWEED,
        BENEDICT,
        MERCER,
        DON_JILA,
        ARMADILLO,
        MACFARLANE,
        THIEVES,
        DAKOTA,
        CALIGA
    }

    public class Node
    {
        public Node(StationName station)
        {
            this.station = station;
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

        private StationName station;
        private Dictionary<StationName, int> neighbors;
        private Dictionary<StationName, int> builtRoads; // any connection that has been built; int is for player ID
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

        private Dictionary<CardColor, int> m_cardCounts;
    }
}