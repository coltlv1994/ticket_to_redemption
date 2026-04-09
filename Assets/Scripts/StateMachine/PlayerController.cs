using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    Intro,
    Play,
    Pause,
    GameOver
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController GetInstance()
    {
        return m_instance;
    }
    
    public void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(m_instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(StateBase newState)
    {
        m_isChangingState = true;

        if (m_currentState != null)
        {
            m_currentState.OnExit();
        }

        m_nextState = newState;
        m_nextState.OnEnter();
    }   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_currentState = new IntroState();
    }

    // Update is called once per frame
    void Update()
    {
        m_currentState.OnUpdate();
    }

    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode)
    {
        m_currentState = m_nextState;
        m_currentState?.OnSceneLoaded(p_scene, p_loadMode);
        m_nextState = null;

        if (p_loadMode == LoadSceneMode.Additive)
        {
            SceneManager.SetActiveScene(p_scene);
        }

        m_isChangingState = false;
    }

    public PlayerState GetPlayerState()
    {         
        return m_playerState;
    }

    public Substate GetSubState()
    {
        return m_substate;
    }

    public void UpdatePlayerRef(Player p_player)
    {
        m_player = p_player;
    }

    public Player GetPlayerRef()
    {
        return m_player;
    }

    public void SetPlayerState(PlayerState state)
    {
        m_playerState = state;
    }

    public void SetPlayerSubstate(Substate substate) 
    {
        m_substate = substate;
    }

    private static PlayerController m_instance;
    private Player m_player = null;

    private StateBase m_currentState;
    private StateBase m_nextState;
    private bool m_isChangingState;

    private PlayerState m_playerState;
    private Substate m_substate;
}
