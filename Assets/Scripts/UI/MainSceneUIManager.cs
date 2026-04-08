using TMPro;
using UnityEngine;

public class MainSceneUIManager : MonoBehaviour
{

    [SerializeField] private GameObject m_player, m_notificationWindow;
    [SerializeField] private TextMeshProUGUI m_playerNotification;
    private GameDataCollection m_gameDataCollection;

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
    }

    public void CloseButton_NotiWindow()
    {
        m_notificationWindow.SetActive(false);
    }
}
