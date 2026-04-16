using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.CoreUtils;

// Route is for connecting neighbors
// first two is start and end and road color
// last three is total cost, tunnel and boat
using Route = System.Tuple<StationName, StationName, CardColor, int, int, int>;

public class GameDataCollection : MonoBehaviour
{
    // make a singleton for game data collection
    #region singletonRealization
    public static GameDataCollection GetInstance()
    {
        return m_instance;
    }

    public void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;

            // This class will NOT call Update()
            enabled = false;

            routes = new List<Route>();
            mapData = new Dictionary<StationName, Node>();
            GenerateCardDecks();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region PublicMethods
    public Node GetNodeByName(StationName stationName)
    {
        if (!mapData.ContainsKey(stationName))
        {
            mapData[stationName] = new Node(stationName);
        }
        return mapData[stationName];
    }

    public CardColor GetRandomCard()
    {
        // this will send cards to player's handdeck
        List<CardColor> availableColors = new List<CardColor>();
        List<int> colorCardCounts = new List<int>();

        int totalNumOfCards = 0;

        foreach (var kvp in cardDeck)
        {
            if (kvp.Value > 0)
            {
                totalNumOfCards += kvp.Value;

                colorCardCounts.Add(totalNumOfCards);
                availableColors.Add(kvp.Key);
            }
        }

        if (totalNumOfCards == 0)
        {

            // re-shuffle the deserted cards back into the card deck
            cardDeck = new Dictionary<CardColor, int>(desertedCardDeck);
            desertedCardDeck = new Dictionary<CardColor, int>(emptyCardDict);

            availableColors.Clear();
            colorCardCounts.Clear();

            foreach (var kvp in cardDeck)
            {
                if (kvp.Value > 0)
                {
                    totalNumOfCards += kvp.Value;

                    colorCardCounts.Add(totalNumOfCards);
                    availableColors.Add(kvp.Key);
                }
            }

            if (availableColors.Count == 0)
            {
                Debug.LogError("No cards available in the deck or deserted deck!");
                return CardColor.PINK; // Return a default value to avoid errors
            }
        }

        int randomIndex = Random.Range(0, totalNumOfCards);
        CardColor randomCard = CardColor.PINK; // Default initialization
        for (int i = 0; i < colorCardCounts.Count; i++)
        {
            if (randomIndex < colorCardCounts[i])
            {
                randomCard = availableColors[i];
                break;
            }
        }
        cardDeck[randomCard]--;
        return randomCard;
    }

    public void DrawCardsToDesk(int numCards)
    {
        // check if there are still possible to darw cards to the desk
        int totalPossibleCards = 0;
        foreach (var kvp in cardDeck)
        {
            totalPossibleCards += kvp.Value;
        }

        foreach (var kvp in desertedCardDeck)
        {
            totalPossibleCards += kvp.Value;
        }

        if (totalPossibleCards < numCards)
        {
            // if there are not enough cards to draw, we will draw all the possible cards to the desk
            numCards = totalPossibleCards;
        }

        bool disableRainbowCardCheck = false;
        if (totalPossibleCards - cardDeck[CardColor.RAINBOW] - desertedCardDeck[CardColor.RAINBOW] < 3)
        {
            // no enough non-rainbow cards to draw, so we will disable the rainbow card check to avoid infinite loop
            disableRainbowCardCheck = true;
        }

        for (int i = 0; i < numCards; i++)
        {
            // this will send cards to the desk
            CardColor drawnCard = GetRandomCard();
            availableCardsOnDesk.Add(drawnCard);
        }

        if (!disableRainbowCardCheck)
        {
            while (IsRainbowCardTooMany())
            {
                // if there are too many rainbow cards, redraw the cards on the desk
                foreach (CardColor card in availableCardsOnDesk)
                {
                    desertedCardDeck[card]++;
                }
                availableCardsOnDesk.Clear();
                for (int i = 0; i < numCards; i++)
                {
                    CardColor drawnCard = GetRandomCard();
                    availableCardsOnDesk.Add(drawnCard);
                }
            }
        }

        onDeskCardChange?.Invoke(availableCardsOnDesk);
    }

    public List<CardColor> GetAvailableCardsOnDesk()
    {
        // this will return the cards that can be immediately drawn by player
        return availableCardsOnDesk;
    }

    public Dictionary<CardColor, int> GetEmptyDict_Color_int()
    {
        return emptyCardDict;
    }

    public void RegisterDeskCardChangeCallback(DeskCardChange_UI callback)
    {
        onDeskCardChange += callback;
    }

    #endregion

    #region PrivateMethods
    private void GenerateMapRoute()
    {
        // DEBUG PART
        routes.Add(new Route(StationName.COLTER, StationName.ISABELLA, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.COLTER, StationName.DAKOTA, CardColor.WHITE, 4, 0, 0));
        routes.Add(new Route(StationName.COLTER, StationName.WAPITI, CardColor.RED, 4, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.DAKOTA, CardColor.BLUE, 3, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.WALLACE, CardColor.GREEN, 4, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.PRONGHORN, CardColor.BLACK, 4, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.DAKOTA, CardColor.ORANGE, 5, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.BUCCHUS, CardColor.YELLOW, 1, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.BRANDWINE, CardColor.RAINBOW, 6, 0, 0));

        // OTHERS
        routes.Add(new Route(StationName.BUCCHUS, StationName.DAKOTA, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.BUCCHUS, StationName.OIL_FIELD, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.BUCCHUS, StationName.OCREAGB, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.BRANDWINE, StationName.OCREAGB, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.BRANDWINE, StationName.ANNESBURG, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.OIL_FIELD, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.ANNESBURG, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.BUTCHER, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.EMERALD, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.ANNESBURG, StationName.VAN_HORN, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.ANNESBURG, StationName.BUTCHER, CardColor.PINK, 1, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.WALLACE, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.OWANJILA, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.STRAWBERRY, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.DAKOTA, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.VALENTINE, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.RIGGS, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.STRAWBERRY, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.DAKOTA, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.OIL_FIELD, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.HEARTLAND, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.FLATNECK, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.RIGGS, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.EMERALD, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.HEARTLAND, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.DAKOTA, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.HEARTLAND, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.CALIGA, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.LAGRAS, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.BUTCHER, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.RHODES, CardColor.PINK, 6, 0, 0));
        routes.Add(new Route(StationName.BUTCHER, StationName.VAN_HORN, CardColor.PINK, 1, 0, 0));
        routes.Add(new Route(StationName.BUTCHER, StationName.LAGRAS, CardColor.PINK, 5, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.PRISON, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.ST_DENIS, CardColor.PINK, 5, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.LAGRAS, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.STRAWBERRY, CardColor.PINK, 1, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.BEECHER, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.MACFARLANE, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.STRAWBERRY, StationName.RIGGS, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.STRAWBERRY, StationName.BEECHER, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.RIGGS, StationName.FLATNECK, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.RIGGS, StationName.BLACKWATER, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.FLATNECK, StationName.HEARTLAND, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.FLATNECK, StationName.RHODES, CardColor.PINK, 5, 0, 0));
        routes.Add(new Route(StationName.HEARTLAND, StationName.RHODES, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.LAGRAS, StationName.CALIGA, CardColor.PINK, 1, 0, 0));
        routes.Add(new Route(StationName.LAGRAS, StationName.ST_DENIS, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.PRISON, StationName.ST_DENIS, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.BEECHER, StationName.BLACKWATER, CardColor.PINK, 1, 0, 0));
        routes.Add(new Route(StationName.BEECHER, StationName.MACFARLANE, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.BLACKWATER, StationName.RHODES, CardColor.PINK, 6, 0, 0));
        routes.Add(new Route(StationName.BLACKWATER, StationName.THIEVES, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.RHODES, StationName.ST_DENIS, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.RHODES, StationName.CALIGA, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.ST_DENIS, StationName.CALIGA, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.ST_DENIS, StationName.THIEVES, CardColor.PINK, 8, 0, 0));
        routes.Add(new Route(StationName.COUJAR, StationName.TUMBLEWEED, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.COUJAR, StationName.BENEDICT, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.TUMBLEWEED, StationName.BENEDICT, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.TUMBLEWEED, StationName.MERCER, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.DON_JILA, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.ARMADILLO, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.BENEDICT, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.ARMADILLO, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.MACFARLANE, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.THIEVES, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.BENEDICT, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.ARMADILLO, StationName.MACFARLANE, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.MACFARLANE, StationName.THIEVES, CardColor.PINK, 2, 0, 0));

        // read into map data
        foreach (Route route in routes)
        {
            mapData[route.Item1].AddNeighbor(route.Item2, route.Item4, route.Item5, route.Item6);
            mapData[route.Item2].AddNeighbor(route.Item1, route.Item4, route.Item5, route.Item6);
        }
    }

    public void InstanciateUIConnectionPerfab()
    {
        GenerateMapRoute();

        foreach (Route route in routes)
        {
            Connection connection = new Connection(route);

            Vector3 startPos = mapData[route.Item1].GetPosition();
            Vector3 endPos = mapData[route.Item2].GetPosition();
            Vector3 intermediateVector = endPos - startPos;
            int totalSections = route.Item4;
            float sectionLength = Vector3.Distance(startPos, endPos) / (float)totalSections;
            Vector3 halfSectionVector = intermediateVector / 2.0f / (float)totalSections;

            Vector3 sectionScale = new Vector3(sectionLength * 0.9f, 0.2f, 1f);
            Quaternion sectionrotation = Quaternion.FromToRotation(Vector3.right, intermediateVector);

            for (int i = 1; i <= totalSections; i++)
            {
                Vector3 sectionMiddlePoint = startPos + (intermediateVector * ((float)i / totalSections)) - halfSectionVector;
                GameObject connectionObj = Instantiate(m_uiConnectionPrefab, sectionMiddlePoint, Quaternion.identity);
                connectionObj.transform.localScale = sectionScale; // Adjust the width and height as needed
                connectionObj.transform.rotation = sectionrotation;
                connectionObj.GetComponent<MeshRenderer>().material.color = route.Item3 switch
                {
                    CardColor.PINK => Color.hotPink,
                    CardColor.RED => Color.red,
                    CardColor.GREEN => Color.green,
                    CardColor.BLUE => Color.blue,
                    CardColor.YELLOW => Color.yellow,
                    CardColor.BLACK => Color.black,
                    CardColor.WHITE => Color.white,
                    CardColor.ORANGE => Color.orangeRed,
                    CardColor.RAINBOW => Color.gray, // grey for rainbow
                    _ => Color.gray, // Default color for undefined card colors
                }
                ;
                UI_Connection uiConnection = connectionObj.GetComponent<UI_Connection>();
                uiConnection.m_connection = connection;
                connection.AddUIPart(uiConnection);
            }
        }
    }

    private void GenerateCardDecks()
    {
        foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
        {
            emptyCardDict.Add(color, 0);
        }

        desertedCardDeck = new Dictionary<CardColor, int>(emptyCardDict);
        cardDeck = new Dictionary<CardColor, int>(emptyCardDict);

        foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
        {
            cardDeck[color] = 12;
        }
        cardDeck[CardColor.RAINBOW] = 14;
    }

    private void GenerateTickets()
    {
        // all destination tickets
    }

    private bool IsRainbowCardTooMany(int p_upperLimit = 3)
    {
        List<CardColor> rainbowCard = availableCardsOnDesk.FindAll(card => card == CardColor.RAINBOW);
        if (rainbowCard.Count < p_upperLimit)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    private List<Route> routes;
    private Dictionary<StationName, Node> mapData;
    private Dictionary<CardColor, int> cardDeck; // this is the main card deck, cards will be drawn from here to player's hand and the desk
    private Dictionary<CardColor, int> desertedCardDeck; // deserted deck
    private List<CardColor> availableCardsOnDesk = new List<CardColor>(); // card that can be immediately drawn by player

    public delegate void DeskCardChange_UI(List<CardColor> updatedDeskCards);
    private DeskCardChange_UI onDeskCardChange;

    private Dictionary<CardColor, int> emptyCardDict = new Dictionary<CardColor, int>();

    private static GameDataCollection m_instance;

    [SerializeField] public GameObject m_uiConnectionPrefab;

}