using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEditor.U2D.ScriptablePacker;

public class MainSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_notificationWindow, m_turnActionWindow;
    [SerializeField] private GameObject m_cancelButonTAW, m_yesButtonTAW;
    [SerializeField] private TextMeshProUGUI m_playerNotification;
    [SerializeField] private TextMeshProUGUI m_titleTAW, m_contentTAW;
    [SerializeField] private TextMeshProUGUI m_greenCardCount, m_blackCardCount, m_redCardCount, m_blueCardCount, m_yellowCardCount, m_orangeCardCount, m_pinkCardCount, m_whiteCardCount, m_rainbowCardCount;
    [SerializeField] private RawImage m_1stDeskCard, m_2ndDeskCard, m_3rdDeskCard, m_4thDeskCard, m_5thDeskCard;
    [SerializeField] private RawImage m_1stCardDCW, m_2ndCardDCW, m_3rdCardDCW, m_4thCardDCW, m_5thCardDCW;
    [SerializeField] private GameObject m_cancelButtonDCW;
    [SerializeField] private GameObject m_deskCardPanel;
    [SerializeField] private Player m_player;
    [SerializeField] private GameObject m_dcwObject;
    [SerializeField] private GameObject m_tcwObject_StartGame, m_shortButton1, m_shortButton2, m_shortButton3, m_longButton1, m_longButton2, m_confirmButtonTCW;

    // main ui
    [SerializeField] private GameObject m_drawCardButton, m_drawTravelButton;

    private GameDataCollection m_gameDataCollection;
    private EventBase m_pendingEvent;
    private Dictionary<CardColor, TextMeshProUGUI> m_cardCountTextMap;
    private List<RawImage> m_deskCardImageMap;
    private Dictionary<CardColor, Texture> m_cardTextureMap;
    private List<RawImage> m_dcwCardImageMap;
    private List<GameObject> m_tcwShortButtonMap;
    private List<GameObject> m_tcwLongButtonMap;
    Texture no_card_texture;


    // input actions
    private InputAction mouseLeftClick;
    private InputAction mouseRightClick;
    private InputAction keyboardWASD;
    private InputAction mouseMove;

    // Camera control
    private bool isRightMouseHold = false;
    private Vector2 kbInput = Vector2.zero;

    // selected card
    private List<int> m_selectedCardIndex = new List<int>();

    // destination cards
    private List<int> m_selectedTravelCardIndex = new List<int>(); // short
    private List<int> m_selectedLongTravelCardIndex = new List<int>(); // long
    private List<TravelTicket> m_availableShortTravelTickets = new List<TravelTicket>();
    private List<TravelTicket> m_availableLongTravelTickets = new List<TravelTicket>();
    private int m_neededShortTravelCardNum = 0;
    private int m_neededLongTravelCardNum = 0;

    // build road window
    [SerializeField] private GameObject m_buildRoadWindow;
    [SerializeField] private GameObject m_resetButtonBRW, m_yesButtonBRW;
    [SerializeField] private TextMeshProUGUI m_contentBRW;
    [SerializeField] private TextMeshProUGUI m_colorCardNumber, m_rainbowCardNumber;
    [SerializeField] private TMP_Dropdown m_colorDropdown;

    private HandDeck m_playerHandDeck; // temporarily hold player's hand deck
    private Connection m_connection_BRW; // temporarily hold the connection player want to build road on
    private int m_selectedColorCardCount_BRW = 0;
    private int m_selectedRainbowCardCount_BRW = 0;

    void Awake()
    {
        mouseLeftClick = InputSystem.actions.FindAction("Click");
        if (mouseLeftClick != null)
        {
            mouseLeftClick.performed += OnMouseClickAction;
        }

        keyboardWASD = InputSystem.actions.FindAction("Move");
        if (keyboardWASD != null)
        {
            keyboardWASD.performed += OnKeyboardWASDAction;
            keyboardWASD.canceled += OnWASDReleased;
        }

        mouseRightClick = InputSystem.actions.FindAction("RightHold");
        if (mouseRightClick != null)
        {
            mouseRightClick.performed += OnRightMousePressed;
            mouseRightClick.canceled += OnRightMouseReleased;
        }

        mouseMove = InputSystem.actions.FindAction("Look");
        if (mouseMove != null)
        {
            mouseMove.performed += OnMouseMove;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_gameDataCollection = GameObject.Find("GameDataCollection").GetComponent<GameDataCollection>();
        if (m_gameDataCollection == null )
        {
            Debug.LogError("GameDataCollection not found in the scene.");
        }
        m_gameDataCollection.RegisterDeskCardChangeCallback(UpdateDeskCards);

        PlayerController playerController = PlayerController.GetInstance();
        playerController.UpdatePlayerRef(m_player);

        m_notificationWindow.SetActive(true);
        m_turnActionWindow.SetActive(false);
        m_dcwObject.SetActive(false);
        m_buildRoadWindow.SetActive(false);
        m_tcwObject_StartGame.SetActive(false);

        m_cardCountTextMap = new Dictionary<CardColor, TextMeshProUGUI>
        {
            { CardColor.GREEN, m_greenCardCount },
            { CardColor.BLACK, m_blackCardCount },
            { CardColor.RED, m_redCardCount },
            { CardColor.BLUE, m_blueCardCount },
            { CardColor.YELLOW, m_yellowCardCount },
            { CardColor.ORANGE, m_orangeCardCount },
            { CardColor.PINK, m_pinkCardCount },
            { CardColor.WHITE, m_whiteCardCount },
            { CardColor.RAINBOW, m_rainbowCardCount }
        };

        m_deskCardImageMap = new List<RawImage>
        {
            m_1stDeskCard,
            m_2ndDeskCard,
            m_3rdDeskCard,
            m_4thDeskCard,
            m_5thDeskCard
        };

        m_dcwCardImageMap = new List<RawImage>
        {
            m_1stCardDCW,
            m_2ndCardDCW,
            m_3rdCardDCW,
            m_4thCardDCW,
            m_5thCardDCW
        };

        m_tcwShortButtonMap = new List<GameObject>
        {
            m_shortButton1,
            m_shortButton2,
            m_shortButton3
        };

        m_tcwLongButtonMap = new List<GameObject>
        {
            m_longButton1,
            m_longButton2
        };

        m_cardTextureMap = new Dictionary<CardColor, Texture>();

        foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
        {
            string texturePath = $"CardPicture/{color.ToString().ToLower()}_card";
            Texture cardTexture = Resources.Load<Texture>(texturePath);
            if (cardTexture != null)
            {
                m_cardTextureMap[color] = cardTexture;
            }
            else
            {
                Debug.LogError($"Failed to load texture for {color} from path: {texturePath}");
            }
        }

        no_card_texture = Resources.Load<Texture>("CardPicture/no_card");
        if (no_card_texture == null)
        {
            Debug.LogError($"Failed to load texture for no_card");
        }
    }

    void Update()
    {
        // cam update
        // Camera movement
        Camera maimCam = Camera.main;

        Vector3 cameraFront = maimCam.transform.forward;
        Vector3 cameraRight = maimCam.transform.right;

        maimCam.transform.position += (cameraFront * kbInput.y + cameraRight * kbInput.x) * Time.deltaTime * 10.0f;
    }

    public void CloseButton_NotiWindow()
    {
        // ask game data collection to generate connections
        m_gameDataCollection.InstanciateUIConnectionPerfab();

        // send game start event to player to trigger the first turn
        m_player.AddEvent(new GameStartDestEvent());

        m_notificationWindow.SetActive(false);
    }

    public void YesButton_TAWindow()
    {
        m_turnActionWindow.SetActive(false);

        // send event
        m_player.AddEvent(m_pendingEvent);
        m_pendingEvent = null;

        // reset UI elements
        ResetAllUIElements();
    }

    public void CancelButton_TAWindow()
    {
        m_turnActionWindow.SetActive(false);

        // reset UI elements
        ResetAllUIElements();
    }

    //public void SetPendingEvent(EventBase p_event, bool p_canAccept)
    //{
    //    // p_canAccept means the event can be accepted by player,
    //    // which will enable the YES button.
    //    m_pendingEvent = p_event;

    //    EventType eventType = p_event.GetEventType();
    //    switch (eventType)
    //    {
    //        case EventType.BUILD_ROAD:
    //            BuildRoadEvent buildRoadEvent = (BuildRoadEvent)p_event;
    //            Connection connection = buildRoadEvent.GetRoadToBuild();
    //            m_titleTAW.text = "Build Road";
    //            m_contentTAW.text = $"{connection.m_end1} - {connection.m_end2}\n Cost: {connection.m_totalCost}\n";
    //            if (!p_canAccept)
    //            {
    //                m_yesButtonTAW.SetActive(false);
    //            }
    //            else
    //            {
    //                m_yesButtonTAW.SetActive(true);
    //            }
    //            break;

    //        default:
    //            break;
    //    }

    //    m_turnActionWindow.SetActive(true);
    //}

    public void SyncCardCount(Dictionary<CardColor, int> cardCounts)
    {
        foreach (var kvp in cardCounts)
        {
            if (m_cardCountTextMap.ContainsKey(kvp.Key))
            {
                m_cardCountTextMap[kvp.Key].text = kvp.Value.ToString();
            }
        }
    }

    private void UpdateDeskCards(List<CardColor> deskCards)
    {
        int numOfCards = deskCards.Count;

        for (int i = 0; i < deskCards.Count; i++)
        {
            m_deskCardImageMap[i].texture = m_cardTextureMap[deskCards[i]];
            m_dcwCardImageMap[i].texture = m_cardTextureMap[deskCards[i]];
        }

        for (; numOfCards < 5; numOfCards++)
        {
            m_deskCardImageMap[numOfCards].texture = no_card_texture;
            m_dcwCardImageMap[numOfCards].texture = no_card_texture;
        }
    }

    public void OnDrawCardButtonClicked()
    {
        m_deskCardPanel.SetActive(false);
        m_turnActionWindow.SetActive(false);
        m_buildRoadWindow.SetActive(false);

        m_dcwObject.SetActive(true);

        List<CardColor> availableCards = m_gameDataCollection.GetAvailableCardsOnDesk();
    }

    public void OnDCWCardButtonClicked(int index)
    {
        bool isSelected = GameDataCollection.GetInstance().ToggleCardSelection(index);
        RawImage correspondingImage = m_dcwCardImageMap[index];

        if (isSelected)
        {
            correspondingImage.color = UnityEngine.Color.grey;
            m_selectedCardIndex.Add(index);
        }
        else
        {
            correspondingImage.color = UnityEngine.Color.white;
            m_selectedCardIndex.Remove(index);
        }
    }

    public void OnDCWConfirmButtonClicked()
    {
        GameDataCollection gdc = GameDataCollection.GetInstance();
        List<CardColor> selectedCards = new List<CardColor>();

        if (gdc != null)
        {
            gdc.ResetDrawCardLimit(selectedCards);
        }

        foreach (CardColor card in selectedCards)
        {
            m_player.AddEvent(new IssueCardEvent(card));
        }

        // reset UI
        OnDCWCancelButtonClicked();
    }

    public void OnDCWCancelButtonClicked()
    {
        // reset UI
        m_1stCardDCW.color = UnityEngine.Color.white;
        m_2ndCardDCW.color = UnityEngine.Color.white;
        m_3rdCardDCW.color = UnityEngine.Color.white;
        m_4thCardDCW.color = UnityEngine.Color.white;
        m_5thCardDCW.color = UnityEngine.Color.white;

        // clear local store
        m_selectedCardIndex.Clear();
        m_dcwObject.SetActive(false);
        m_deskCardPanel.SetActive(true);
        m_drawCardButton.SetActive(true);
        m_drawTravelButton.SetActive(true);
        m_cancelButtonDCW.SetActive(true);
    }

    public void OnDCWRandomCardClicked()
    {
        // once you draw random card, you cannot draw any travel cards or cancel anything
        m_cancelButtonDCW.SetActive(false);
        m_drawTravelButton.SetActive(false);

        GameDataCollection gdc = GameDataCollection.GetInstance();

        // Get draw card upper limit
        int drawCardLimit = gdc.GetDrawCardLimit();

        // check if we should remove one from selected
        if (m_selectedCardIndex.Count == drawCardLimit)
        {
            OnDCWCardButtonClicked(m_selectedCardIndex[0]);
        }

        // decrease one upper limit from GDC
        GameDataCollection.GetInstance().ReduceOneDrawCardLimit();
        drawCardLimit -= 1;

        // issue one random card to player
        if (m_player != null)
        {
            m_player.AddEvent(new DrawCardEvent());
        }

        if (drawCardLimit <= 0)
        {
            // as if clicked confirm
            OnDCWConfirmButtonClicked();
        }
    }

    public void ResetAllUIElements()
    {
        // reset all UI elements to their default state
        m_notificationWindow.SetActive(false);
        m_turnActionWindow.SetActive(false);
        m_deskCardPanel.SetActive(true);
    }

    private void OnMouseClickAction(InputAction.CallbackContext obj)
    {
        if (m_turnActionWindow.activeSelf == true || m_dcwObject.activeSelf == true || m_tcwObject_StartGame.activeSelf == true || m_buildRoadWindow.activeSelf == true)
        {
            // if turn action window is active, ignore mouse click on the map
            return;
        }

        // do stuff
        Vector2 vector2 = Mouse.current.position.ReadValue();
        Ray rayOrigin = Camera.main.ScreenPointToRay(vector2);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, out hit))
        {
            Station hitStation = hit.collider.gameObject.GetComponent<Station>();
            if (hitStation != null)
            {
                Debug.Log("Clicked on station: " + hitStation.m_name.ToString());
                return;
            }

            UI_Connection hitConnection = hit.collider.gameObject.GetComponent<UI_Connection>();
            if (hitConnection != null)
            {
                Connection hitResult = hitConnection.m_connection;
                bool isBuildable = m_player.IsRouteBuildable(hitResult);

                // set pending event
                if (isBuildable)
                {
                    BuildRoadEvent buildRoadEvent = new BuildRoadEvent(hitResult);
                    m_pendingEvent = buildRoadEvent;
                }

                // show turn action window
                m_titleTAW.text = "Build Road";
                m_contentTAW.text = $"{hitResult.m_end1} - {hitResult.m_end2}\n Cost: {hitResult.m_totalCost}\n";
                m_yesButtonTAW.SetActive(isBuildable);

                //m_titleTAW.text = "DEBUG";
                //m_contentTAW.text = hitConnection.transform.rotation.eulerAngles.ToString();
                //m_yesButtonTAW.SetActive(false);

                m_turnActionWindow.SetActive(true); 
                
                return;
            }

        }
    }

    private void OnKeyboardWASDAction(InputAction.CallbackContext obj)
    {
        kbInput = obj.ReadValue<Vector2>();
    }

    private void OnWASDReleased(InputAction.CallbackContext obj)
    {
        kbInput = Vector2.zero;
    }

    private void OnRightMousePressed(InputAction.CallbackContext obj)
    {
        isRightMouseHold = true;
        Debug.Log("Right mouse clicked");
    }

    private void OnRightMouseReleased(InputAction.CallbackContext obj)
    {
        isRightMouseHold = false;
        Debug.Log("Right mouse released");
    }

    private void OnMouseMove(InputAction.CallbackContext obj)
    {
        if (isRightMouseHold)
        {
            // x+: right, x-: left, y+: up, y-: down
            Vector2 vector2 = obj.ReadValue<Vector2>();
            Camera.main.transform.SetPositionAndRotation(Camera.main.transform.position, Quaternion.Euler(
                Camera.main.transform.rotation.eulerAngles + new Vector3(-vector2.y, vector2.x, 0) * Time.deltaTime * 100.0f));
        }
    }

    public void OnPlayerDrawTravel(Player p_player, int p_shortTravelsNeeded, int p_longTravelsNeeded = 0)
    {
        m_tcwObject_StartGame.SetActive(true);
        m_drawCardButton.SetActive(false);

        m_availableShortTravelTickets.Clear();
        m_availableLongTravelTickets.Clear();

        m_selectedTravelCardIndex.Clear();
        m_selectedLongTravelCardIndex.Clear();

        GameDataCollection gdc = GameDataCollection.GetInstance();
        bool isSuccess = gdc.FillTravelCards(m_availableShortTravelTickets, m_availableLongTravelTickets, p_shortTravelsNeeded, p_longTravelsNeeded);
        
        if (!isSuccess)
        {
            // shortage of travel tickets, give player whatever is left
            Debug.LogWarning("Not enough travel tickets to fill the requirement. Player can only choose from the remaining tickets.");
            p_shortTravelsNeeded = m_availableShortTravelTickets.Count;
            p_longTravelsNeeded = m_availableLongTravelTickets.Count;
        }

        m_neededShortTravelCardNum = Mathf.Min(p_shortTravelsNeeded, 2);
        m_neededLongTravelCardNum = Mathf.Min(p_longTravelsNeeded, 1);

        for (int i = 0; i <= m_neededShortTravelCardNum; i++)
        {
            m_tcwShortButtonMap[i].SetActive(true);
            m_tcwShortButtonMap[i].GetComponentInChildren<TextMeshProUGUI>().text = m_availableShortTravelTickets[i].ToString();
        }

        for (int i = 0;i <= m_neededLongTravelCardNum; i++)
        {
            m_tcwLongButtonMap[i].SetActive(true);
            m_tcwLongButtonMap[i].GetComponentInChildren<TextMeshProUGUI>().text = m_availableLongTravelTickets[i].ToString();
        }

        m_confirmButtonTCW.SetActive(false);

        return;
    }

    public void OnTCWShortButtonClicked(int index)
    {
        bool isSelected = m_selectedTravelCardIndex.Contains(index);
        Image buttonImage = m_tcwShortButtonMap[index].GetComponent<Image>();

        if (isSelected)
        {
            m_selectedTravelCardIndex.Remove(index);
            buttonImage.color = UnityEngine.Color.white;
        }
        else
        {
            m_selectedTravelCardIndex.Add(index);
            buttonImage.color = UnityEngine.Color.grey;
        }

        if (m_selectedTravelCardIndex.Count >= m_neededShortTravelCardNum && m_selectedLongTravelCardIndex.Count >= m_neededLongTravelCardNum)
        {
            m_confirmButtonTCW.SetActive(true);
        }
        else
        {
            m_confirmButtonTCW.SetActive(false);
        }
    }

    public void OnTCWLongButtonClicked(int index)
    {
        if (m_selectedLongTravelCardIndex.Count > 0)
        {
            // if player has already selected some short travel cards, they cannot select long travel cards
            return;
        }

        bool isSelected = m_selectedLongTravelCardIndex.Contains(index);
        Image buttonImage = m_tcwLongButtonMap[index].GetComponent<Image>();

        if (isSelected)
        {
            m_selectedLongTravelCardIndex.Remove(index);
            buttonImage.color = UnityEngine.Color.white;
        }
        else
        {
            m_selectedLongTravelCardIndex.Add(index);
            buttonImage.color = UnityEngine.Color.grey;
        }

        if (m_selectedTravelCardIndex.Count >= m_neededShortTravelCardNum && m_selectedLongTravelCardIndex.Count >= m_neededLongTravelCardNum)
        {
            m_confirmButtonTCW.SetActive(true);
        }
        else
        {
            m_confirmButtonTCW.SetActive(false);
        }
    }

    public void OnTCWConfirmButtonClicked()
    {
        // send event to player
        foreach (int index in m_selectedTravelCardIndex)
        {
            m_player.AddEvent(new IssueTravelCardEvent(m_availableShortTravelTickets[index]));
        }

        foreach (int index in m_selectedLongTravelCardIndex)
        {
            m_player.AddEvent(new IssueTravelCardEvent(m_availableLongTravelTickets[index]));
        }

        // reset UI
        m_tcwObject_StartGame.SetActive(false);

        foreach (GameObject button in m_tcwShortButtonMap)
        {
            button.SetActive(false);
            button.GetComponent<Image>().color = UnityEngine.Color.white;
        }

        foreach (GameObject button in m_tcwLongButtonMap)
        {
            button.SetActive(false);
            button.GetComponent<Image>().color = UnityEngine.Color.white;
        }

        m_confirmButtonTCW.SetActive(false);
        m_drawCardButton.SetActive(true);

        // clear local store
        m_selectedTravelCardIndex.Clear();
        m_selectedLongTravelCardIndex.Clear();
    }

    public void OnMainDrawTravelClicked()
    {
        m_drawCardButton.SetActive(false);
        OnPlayerDrawTravel(m_player, 3, 0);
    }

    public void OnPlayerBuildRoad(Player p_player, HandDeck p_handDeck, Connection p_connection)
    {
        // let player choose which color cards to pick
        m_playerHandDeck = p_handDeck;
        m_connection_BRW = p_connection;

        m_turnActionWindow.SetActive(false);
        m_buildRoadWindow.SetActive(true);
        m_contentBRW.text = $"Need: {m_connection_BRW.m_totalCost}";

        int rainBowCount = m_playerHandDeck.GetCardCount(CardColor.RAINBOW);

        if (rainBowCount == 0 )
        {
            // dont display rainbow card

        }

        // init BRW UI
        m_selectedRainbowCardCount_BRW = 0;

        m_colorDropdown.ClearOptions();
        List<string> colorOptions = new List<string>();

        // determine available options
        if (m_connection_BRW.m_roadColor == CardColor.RAINBOW)
        {

            colorOptions = System.Enum.GetNames(typeof(CardColor)).ToList();
            // remvoe last one since it's rainbow
            colorOptions.RemoveAt(colorOptions.Count - 1);

            m_colorDropdown.AddOptions(colorOptions);
        }
        else
        {
            colorOptions.Add(m_connection_BRW.m_roadColor.ToString());
            m_colorDropdown.AddOptions(colorOptions);
        }

        m_colorDropdown.value = 0;
        m_colorDropdown.RefreshShownValue();
        int givenColorCardCount = m_playerHandDeck.GetCardCountByName(colorOptions[0]);

        if (givenColorCardCount >= m_connection_BRW.m_totalCost)
        {
            m_yesButtonBRW.SetActive(true);
            m_selectedColorCardCount_BRW = m_connection_BRW.m_totalCost;
        }
        else
        {
            m_yesButtonBRW.SetActive(false);
            m_selectedColorCardCount_BRW = givenColorCardCount;
        }

        m_colorCardNumber.text = m_selectedColorCardCount_BRW.ToString();
        m_rainbowCardNumber.text = m_selectedRainbowCardCount_BRW.ToString();
    }

    public void OnBRWDropdownValueChanged()
    {
        int dropdownValue = m_colorDropdown.value;

        Debug.Log("Dropdown value changed: " + dropdownValue);

        m_selectedRainbowCardCount_BRW = 0;

        int givenColorCardCount = m_playerHandDeck.GetCardCountByName(m_colorDropdown.options[dropdownValue].text);

        if (givenColorCardCount >= m_connection_BRW.m_totalCost)
        {
            m_yesButtonBRW.SetActive(true);
            m_selectedColorCardCount_BRW = m_connection_BRW.m_totalCost;
            m_colorCardNumber.text = m_connection_BRW.m_totalCost.ToString();
        }
        else
        {
            m_yesButtonBRW.SetActive(false);
            m_selectedColorCardCount_BRW = givenColorCardCount;
            m_colorCardNumber.text = givenColorCardCount.ToString();
        }

        m_colorCardNumber.text = m_selectedColorCardCount_BRW.ToString();
        m_rainbowCardNumber.text = m_selectedRainbowCardCount_BRW.ToString();
    }

    public void OnBRWRainbowButtonClicked()
    {
        int rainbowCardCount = m_playerHandDeck.GetCardCount(CardColor.RAINBOW);
        int totalCost = m_connection_BRW.m_totalCost;

        rainbowCardCount = Mathf.Min(rainbowCardCount, totalCost); // player can't use more rainbow cards than the total cost

        m_selectedRainbowCardCount_BRW += 1;

        if (m_selectedRainbowCardCount_BRW > rainbowCardCount)
        {
            m_selectedRainbowCardCount_BRW = 0;

            // reset color card number
            int dropdownValue = m_colorDropdown.value;
            m_selectedColorCardCount_BRW = Mathf.Min(m_playerHandDeck.GetCardCountByName(m_colorDropdown.options[dropdownValue].text), totalCost);
        }

        m_rainbowCardNumber.text = m_selectedRainbowCardCount_BRW.ToString();

        if (m_selectedColorCardCount_BRW + m_selectedRainbowCardCount_BRW > totalCost)
        {
            m_selectedColorCardCount_BRW -= 1;
            
            m_yesButtonBRW.SetActive(true);
        }
        else if (m_selectedColorCardCount_BRW + m_selectedRainbowCardCount_BRW == totalCost)
        {
            m_yesButtonBRW.SetActive(true);
        }
        else
        {
            m_yesButtonBRW.SetActive(false);
        }

        m_colorCardNumber.text = m_selectedColorCardCount_BRW.ToString();
        m_rainbowCardNumber.text = m_selectedRainbowCardCount_BRW.ToString();
    }

    public void OnBRWBuildButtonClicked()
    {
        // ask player to reduce number of color cards and m number of rainbow cards
        CardColor selectedColor = (CardColor)System.Enum.Parse(typeof(CardColor), m_colorDropdown.options[m_colorDropdown.value].text);
        m_playerHandDeck.AddCard(selectedColor, -m_selectedColorCardCount_BRW);
        m_playerHandDeck.AddCard(CardColor.RAINBOW, -m_selectedRainbowCardCount_BRW);

        // send event to player
        m_player.AddEvent(new ClaimRoadEvent(m_connection_BRW));

        // reset UI and internal state as canceled action
        OnBRWCancelButtonClicked();
    }

    public void OnBRWResetButtonClicked()
    {
        m_colorDropdown.value = 0;
        m_colorDropdown.RefreshShownValue();

        OnBRWDropdownValueChanged();
    }

    public void OnBRWCancelButtonClicked()
    {
        // reset UI and internal state
        m_buildRoadWindow.SetActive(false);
        m_colorDropdown.ClearOptions();
        m_selectedColorCardCount_BRW = 0;
        m_selectedRainbowCardCount_BRW = 0;
        m_colorCardNumber.text = m_selectedColorCardCount_BRW.ToString();
        m_rainbowCardNumber.text = m_selectedRainbowCardCount_BRW.ToString();
    }
}
    