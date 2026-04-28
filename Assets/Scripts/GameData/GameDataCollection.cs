using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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

    public bool ToggleCardSelection(int cardIndex)
    {
        if (m_pickedCardIndex.Contains(cardIndex))
        {
            m_pickedCardIndex.Remove(cardIndex);
            return false; // card is now unselected
        }
        else
        {
            if (m_pickedCardIndex.Count < m_allowedCardPickFromDesk)
            {
                m_pickedCardIndex.Add(cardIndex);
                return true; // card is now selected
            }
            else
            {
                Debug.LogWarning("You can only select up to " + m_allowedCardPickFromDesk + " cards from the desk.");
                return false; // selection limit reached, card remains unselected
            }
        }
    }

    public void ReduceOneDrawCardLimit()
    {
        m_allowedCardPickFromDesk -= 1;

        if (m_allowedCardPickFromDesk < 0)
        {
            m_allowedCardPickFromDesk = 0;
        }
    }

    public void ResetDrawCardLimit(List<CardColor> p_pickedCards)
    {
        // this function will be called when confirm button is clicked.

        // clear the object
        p_pickedCards.Clear();

        // prepare return value
        foreach (int pickedIndex in m_pickedCardIndex)
        {
            CardColor pickedCard = availableCardsOnDesk[pickedIndex];
            p_pickedCards.Add(pickedCard);
        }

        // remove from desk
        switch (m_pickedCardIndex.Count)
        {
            case 1:
                // one card is picked, move it from desk to player's hand
                availableCardsOnDesk.RemoveAt(m_pickedCardIndex[0]);
                break;
            case 2:
                if (m_pickedCardIndex[0] > m_pickedCardIndex[1])
                {
                    // if the first picked card index is greater than the second one, we need to remove the first one first to avoid index shift
                    availableCardsOnDesk.RemoveAt(m_pickedCardIndex[0]);
                    availableCardsOnDesk.RemoveAt(m_pickedCardIndex[1]);
                }
                else
                {
                    availableCardsOnDesk.RemoveAt(m_pickedCardIndex[1]);
                    availableCardsOnDesk.RemoveAt(m_pickedCardIndex[0]);
                }
                break;
            default:
                break;
        }

        // draw cards to desk if necessary
        if (availableCardsOnDesk.Count < 5)
        {
            DrawCardsToDesk(5 - availableCardsOnDesk.Count);
        }

        // reset draw card limit and index list
        m_allowedCardPickFromDesk = 2;
        m_pickedCardIndex.Clear();
    }

    public int GetDrawCardLimit()
    {
        return m_allowedCardPickFromDesk;
    }

    public void GameStartCardPick(List<TravelTicket> p_shortTravels, List<TravelTicket> p_longTravels)
    {
        int shortTravelCardsToPick = 4;
        int longTravelCardsToPick = 2;

        p_shortTravels.Clear();
        p_longTravels.Clear();

        System.Random rng = new System.Random();

    }

    #endregion

    #region PrivateMethods
    private void GenerateMapRoute()
    {
        routes.Add(new Route(StationName.BUCCHUS, StationName.DAKOTA, CardColor.BLACK, 4, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.STRAWBERRY, CardColor.BLACK, 1, 0, 0));
        routes.Add(new Route(StationName.RIGGS, StationName.BLACKWATER, CardColor.BLACK, 2, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.VALENTINE, CardColor.BLACK, 3, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.EMERALD, CardColor.BLACK, 2, 0, 0));
        routes.Add(new Route(StationName.PRISON, StationName.ST_DENIS, CardColor.BLACK, 3, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.LAGRAS, CardColor.BLACK, 4, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.FLATNECK, CardColor.BLUE, 2, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.HEARTLAND, CardColor.BLUE, 2, 0, 0));
        routes.Add(new Route(StationName.RHODES, StationName.CALIGA, CardColor.BLUE, 2, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.DAKOTA, CardColor.BLUE, 3, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.LAGRAS, CardColor.BLUE, 3, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.BENEDICT, CardColor.BLUE, 3, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.BUTCHER, CardColor.BLUE, 3, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.BENEDICT, CardColor.GREEN, 4, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.DAKOTA, CardColor.GREEN, 3, 0, 0));
        routes.Add(new Route(StationName.HEARTLAND, StationName.RHODES, CardColor.GREEN, 4, 0, 0));
        routes.Add(new Route(StationName.STRAWBERRY, StationName.RIGGS, CardColor.GREEN, 2, 0, 0));
        routes.Add(new Route(StationName.MACFARLANE, StationName.THIEVES, CardColor.GREEN, 2, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.EMERALD, CardColor.GREEN, 3, 0, 0));
        routes.Add(new Route(StationName.COLTER, StationName.DAKOTA, CardColor.ORANGE, 4, 0, 0));
        routes.Add(new Route(StationName.BUTCHER, StationName.VAN_HORN, CardColor.ORANGE, 1, 0, 0));
        routes.Add(new Route(StationName.FLATNECK, StationName.HEARTLAND, CardColor.ORANGE, 2, 0, 0));
        routes.Add(new Route(StationName.TUMBLEWEED, StationName.BENEDICT, CardColor.ORANGE, 3, 0, 0));
        routes.Add(new Route(StationName.ARMADILLO, StationName.MACFARLANE, CardColor.ORANGE, 3, 0, 0));
        routes.Add(new Route(StationName.BUCCHUS, StationName.OCREAGB, CardColor.ORANGE, 3, 0, 0));
        routes.Add(new Route(StationName.STRAWBERRY, StationName.BEECHER, CardColor.ORANGE, 2, 0, 0));
        routes.Add(new Route(StationName.TUMBLEWEED, StationName.MERCER, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.BEECHER, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.BUCCHUS, CardColor.PINK, 1, 0, 0));
        routes.Add(new Route(StationName.BRANDWINE, StationName.ANNESBURG, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.RIGGS, CardColor.PINK, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.HEARTLAND, CardColor.PINK, 3, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.OIL_FIELD, CardColor.PINK, 4, 0, 0));
        routes.Add(new Route(StationName.LAGRAS, StationName.CALIGA, CardColor.RAINBOW, 1, 0, 0));
        routes.Add(new Route(StationName.ANNESBURG, StationName.VAN_HORN, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.DAKOTA, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.OIL_FIELD, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.RIGGS, StationName.FLATNECK, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.COUJAR, StationName.TUMBLEWEED, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.COUJAR, StationName.BENEDICT, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.DON_JILA, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.ARMADILLO, CardColor.RAINBOW, 2, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.WALLACE, CardColor.RAINBOW, 3, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.BUTCHER, CardColor.RAINBOW, 3, 0, 0));
        routes.Add(new Route(StationName.BEECHER, StationName.MACFARLANE, CardColor.RAINBOW, 3, 0, 0));
        routes.Add(new Route(StationName.BLACKWATER, StationName.THIEVES, CardColor.RAINBOW, 3, 0, 0));
        routes.Add(new Route(StationName.COLTER, StationName.WAPITI, CardColor.RAINBOW, 4, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.PRONGHORN, CardColor.RAINBOW, 4, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.CALIGA, CardColor.RAINBOW, 4, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.MACFARLANE, CardColor.RAINBOW, 4, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.DAKOTA, CardColor.RAINBOW, 5, 0, 0));
        routes.Add(new Route(StationName.BUTCHER, StationName.LAGRAS, CardColor.RAINBOW, 5, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.ST_DENIS, CardColor.RAINBOW, 5, 0, 0));
        routes.Add(new Route(StationName.FLATNECK, StationName.RHODES, CardColor.RAINBOW, 5, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.BRANDWINE, CardColor.RAINBOW, 6, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.RHODES, CardColor.RAINBOW, 6, 0, 0));
        routes.Add(new Route(StationName.BLACKWATER, StationName.RHODES, CardColor.RAINBOW, 6, 0, 0));
        routes.Add(new Route(StationName.ST_DENIS, StationName.THIEVES, CardColor.RAINBOW, 8, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.HEARTLAND, CardColor.RED, 3, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.PRISON, CardColor.RED, 4, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.OWANJILA, CardColor.RED, 2, 0, 0));
        routes.Add(new Route(StationName.ST_DENIS, StationName.CALIGA, CardColor.RED, 2, 0, 0));
        routes.Add(new Route(StationName.BRANDWINE, StationName.OCREAGB, CardColor.RED, 3, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.MACFARLANE, CardColor.RED, 3, 0, 0));
        routes.Add(new Route(StationName.COLTER, StationName.ISABELLA, CardColor.WHITE, 2, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.STRAWBERRY, CardColor.WHITE, 2, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.DAKOTA, CardColor.WHITE, 2, 0, 0));
        routes.Add(new Route(StationName.LAGRAS, StationName.ST_DENIS, CardColor.WHITE, 3, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.ANNESBURG, CardColor.WHITE, 4, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.THIEVES, CardColor.WHITE, 4, 0, 0));
        routes.Add(new Route(StationName.BEECHER, StationName.BLACKWATER, CardColor.WHITE, 1, 0, 0));
        routes.Add(new Route(StationName.RHODES, StationName.ST_DENIS, CardColor.YELLOW, 4, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.ARMADILLO, CardColor.YELLOW, 2, 0, 0));
        routes.Add(new Route(StationName.ANNESBURG, StationName.BUTCHER, CardColor.YELLOW, 1, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.STRAWBERRY, CardColor.YELLOW, 2, 0, 0));
        routes.Add(new Route(StationName.BUCCHUS, StationName.OIL_FIELD, CardColor.YELLOW, 3, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.RIGGS, CardColor.YELLOW, 3, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.WALLACE, CardColor.YELLOW, 4, 0, 0));

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
        GenerateTickets();

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
        // short travels
        shortTravels.Add(new TravelTicket(StationName.BUCCHUS, StationName.VALENTINE, 5));
        shortTravels.Add(new TravelTicket(StationName.PRONGHORN, StationName.BLACKWATER, 5));
        shortTravels.Add(new TravelTicket(StationName.STRAWBERRY, StationName.DAKOTA, 5));
        shortTravels.Add(new TravelTicket(StationName.TUMBLEWEED, StationName.DON_JILA, 5));
        shortTravels.Add(new TravelTicket(StationName.MERCER, StationName.MACFARLANE, 5));
        shortTravels.Add(new TravelTicket(StationName.ISABELLA, StationName.STRAWBERRY, 6));
        shortTravels.Add(new TravelTicket(StationName.PRONGHORN, StationName.FLATNECK, 6));
        shortTravels.Add(new TravelTicket(StationName.EMERALD, StationName.ST_DENIS, 6));
        shortTravels.Add(new TravelTicket(StationName.STRAWBERRY, StationName.HEARTLAND, 6));
        shortTravels.Add(new TravelTicket(StationName.BEECHER, StationName.DON_JILA, 6));
        shortTravels.Add(new TravelTicket(StationName.ISABELLA, StationName.FLATNECK, 7));
        shortTravels.Add(new TravelTicket(StationName.OCREAGB, StationName.FLATNECK, 7));
        shortTravels.Add(new TravelTicket(StationName.VALENTINE, StationName.RHODES, 7));
        shortTravels.Add(new TravelTicket(StationName.VAN_HORN, StationName.RHODES, 7));
        shortTravels.Add(new TravelTicket(StationName.BEECHER, StationName.DAKOTA, 7));
        shortTravels.Add(new TravelTicket(StationName.COLTER, StationName.OWANJILA, 8));
        shortTravels.Add(new TravelTicket(StationName.ISABELLA, StationName.BLACKWATER, 8));
        shortTravels.Add(new TravelTicket(StationName.BRANDWINE, StationName.HEARTLAND, 8));
        shortTravels.Add(new TravelTicket(StationName.OCREAGB, StationName.PRISON, 8));
        shortTravels.Add(new TravelTicket(StationName.PRONGHORN, StationName.OIL_FIELD, 8));
        shortTravels.Add(new TravelTicket(StationName.VALENTINE, StationName.BUTCHER, 8));
        shortTravels.Add(new TravelTicket(StationName.BUTCHER, StationName.FLATNECK, 8));
        shortTravels.Add(new TravelTicket(StationName.STRAWBERRY, StationName.DON_JILA, 8));
        shortTravels.Add(new TravelTicket(StationName.BEECHER, StationName.MERCER, 8));
        shortTravels.Add(new TravelTicket(StationName.BENEDICT, StationName.THIEVES, 8));
        shortTravels.Add(new TravelTicket(StationName.WAPITI, StationName.LAGRAS, 9));
        shortTravels.Add(new TravelTicket(StationName.WALLACE, StationName.RHODES, 9));
        shortTravels.Add(new TravelTicket(StationName.EMERALD, StationName.BEECHER, 10));
        shortTravels.Add(new TravelTicket(StationName.THIEVES, StationName.CALIGA, 10));
        shortTravels.Add(new TravelTicket(StationName.VALENTINE, StationName.ST_DENIS, 11));
        shortTravels.Add(new TravelTicket(StationName.RIGGS, StationName.MERCER, 11));
        shortTravels.Add(new TravelTicket(StationName.BEECHER, StationName.COUJAR, 12));
        shortTravels.Add(new TravelTicket(StationName.PRISON, StationName.BLACKWATER, 13));

        // long travels
        longTravels.Add(new TravelTicket(StationName.COLTER, StationName.BENEDICT, 19));
        longTravels.Add(new TravelTicket(StationName.ANNESBURG, StationName.ARMADILLO, 20));
        longTravels.Add(new TravelTicket(StationName.BRANDWINE, StationName.DON_JILA, 21));
        longTravels.Add(new TravelTicket(StationName.WAPITI, StationName.TUMBLEWEED, 23));
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
    private List<TravelTicket> shortTravels = new List<TravelTicket>();
    private List<TravelTicket> longTravels = new List<TravelTicket>();

    private Dictionary<StationName, Node> mapData;
    private Dictionary<CardColor, int> cardDeck; // this is the main card deck, cards will be drawn from here to player's hand and the desk
    private Dictionary<CardColor, int> desertedCardDeck; // deserted deck
    private List<CardColor> availableCardsOnDesk = new List<CardColor>(); // card that can be immediately drawn by player

    public delegate void DeskCardChange_UI(List<CardColor> updatedDeskCards);
    private DeskCardChange_UI onDeskCardChange;

    private int m_allowedCardPickFromDesk = 2;
    private List<int> m_pickedCardIndex = new List<int>();

    private Dictionary<CardColor, int> emptyCardDict = new Dictionary<CardColor, int>();

    private static GameDataCollection m_instance;

    [SerializeField] public GameObject m_uiConnectionPrefab;

}