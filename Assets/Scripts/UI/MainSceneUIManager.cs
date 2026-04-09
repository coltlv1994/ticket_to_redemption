using TMPro;
using UnityEngine;

public class MainSceneUIManager : MonoBehaviour
{

    [SerializeField] private GameObject m_player, m_notificationWindow, m_turnActionWindow;
    [SerializeField] private TextMeshProUGUI m_playerNotification;
    private GameDataCollection m_gameDataCollection;
    private EventBase m_pendingEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_gameDataCollection = GameObject.Find("GameDataCollection").GetComponent<GameDataCollection>();
        if (m_gameDataCollection == null )
        {
            Debug.LogError("GameDataCollection not found in the scene.");
        }

        PlayerController playerController = PlayerController.GetInstance();
        playerController.UpdatePlayerRef(m_player.GetComponent<Player>());

        m_notificationWindow.SetActive(true);
        m_turnActionWindow.SetActive(false);
    }

    public void CloseButton_NotiWindow()
    {
        m_notificationWindow.SetActive(false);
    }

    public void YesButton_TAWindow()
    {
        m_notificationWindow.SetActive(false);

        // send event
    }

    public void CancelButton_TAWindow()
    {
        m_notificationWindow.SetActive(false);
    }

    public void SetPendingEvent(EventBase p_event, bool p_canAccept)
    {
        // p_canAccept means the event can be accepted by player,
        // which will enable the YES button.
        m_pendingEvent = p_event;
    }
}
