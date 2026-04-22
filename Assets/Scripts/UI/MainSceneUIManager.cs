using System.Collections.Generic;
using System.Drawing;
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
    [SerializeField] private GameObject m_deskCardPanel;
    [SerializeField] private Player m_player;

    private GameDataCollection m_gameDataCollection;
    private EventBase m_pendingEvent;
    private Dictionary<CardColor, TextMeshProUGUI> m_cardCountTextMap;
    private Dictionary<int, RawImage> m_deskCardImageMap;
    private Dictionary<CardColor, Texture> m_cardTextureMap;
    private Dictionary<int, RawImage> m_dcwCardImageMap;
    Texture no_card_texture;

    // input actions
    private InputAction mouseLeftClick;
    private InputAction mouseRightClick;
    private InputAction keyboardWASD;
    private InputAction mouseMove;

    // Camera control
    private bool isRightMouseHold = false;
    private Vector2 kbInput = Vector2.zero;

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

        m_deskCardImageMap = new Dictionary<int, RawImage>
        {
            { 0, m_1stDeskCard },
            { 1, m_2ndDeskCard },
            { 2, m_3rdDeskCard },
            { 3, m_4thDeskCard },
            { 4, m_5thDeskCard }
        };

        m_dcwCardImageMap = new Dictionary<int, RawImage>
        {
            { 0, m_1stCardDCW },
            { 1, m_2ndCardDCW },
            { 2, m_3rdCardDCW },
            { 3, m_4thCardDCW },
            { 4, m_5thCardDCW }
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

    public void SetPendingEvent(EventBase p_event, bool p_canAccept)
    {
        // p_canAccept means the event can be accepted by player,
        // which will enable the YES button.
        m_pendingEvent = p_event;

        EventType eventType = p_event.GetEventType();
        switch (eventType)
        {
            case EventType.BUILD_ROAD:
                BuildRoadEvent buildRoadEvent = (BuildRoadEvent)p_event;
                Connection connection = buildRoadEvent.GetRoadToBuild();
                m_titleTAW.text = "Build Road";
                m_contentTAW.text = $"{connection.m_end1} - {connection.m_end2}\n Cost: {connection.m_totalCost}\n";
                if (!p_canAccept)
                {
                    m_yesButtonTAW.SetActive(false);
                }
                else
                {
                    m_yesButtonTAW.SetActive(true);
                }
                break;

            default:
                break;
        }

        m_turnActionWindow.SetActive(true);
    }

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
        }

        for (; numOfCards < 5; numOfCards++)
        {
            m_deskCardImageMap[numOfCards].texture = no_card_texture;
        }
    }

    public void OnDrawCardButtonClicked()
    {
        m_deskCardPanel.SetActive(false);

        List<CardColor> availableCards = m_gameDataCollection.GetAvailableCardsOnDesk();


        m_turnActionWindow.SetActive(true);
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
        if (m_turnActionWindow.activeSelf == true)
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
}
