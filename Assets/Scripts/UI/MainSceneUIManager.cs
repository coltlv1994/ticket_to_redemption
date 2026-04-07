using TMPro;
using UnityEngine;

public class MainSceneUIManager : MonoBehaviour
{

    [SerializeField] private GameObject Player;
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
        playerController.UpdatePlayerRef(Player.GetComponent<Player>());
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController playerController = PlayerController.GetInstance();

        m_playerNotification.text = playerController.GetCurrentState().ToString();
    }
}
