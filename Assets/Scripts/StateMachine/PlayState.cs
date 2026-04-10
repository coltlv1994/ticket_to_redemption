using UnityEngine;
using UnityEngine.SceneManagement;

public enum Substate
{
    GameStart,
    TurnStart,
    Turn,
    TurnEnd,
    WaitingForOthers,
    NOT_IN_PLAY_STATE,
}

public class PlayState : StateBase
{
    public void OnEnter()
    {
        SceneManager.LoadSceneAsync("MainScene");
        PlayerController pc = PlayerController.GetInstance();
        pc.SetPlayerState(PlayerState.Play);
        pc.SetPlayerSubstate(Substate.WaitingForOthers);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        switch (m_subState)
        {
            case Substate.GameStart:
                // preparation work
                GameDataCollection.GetInstance().DrawCardsToDesk(5);
                m_player?.AddEvent(new DrawCardEvent(4));
                // let AI or other player draw, too

                // draw ticket
                m_player?.AddEvent(new DrawTicketEvent(TicketType.NORMAL, 3));
                m_player?.AddEvent(new DrawTicketEvent(TicketType.LONG, 2));
                // let AI or other player draw, too

                // Game start, need some logic to decide who goes first
                m_subState = Substate.TurnStart;
                break;
            case Substate.TurnStart:
                // Do nothing
                break;
            case Substate.Turn:
                //m_player?
                // Do nothing
                break;
            case Substate.TurnEnd:
                // Do nothing
                break;
            case Substate.WaitingForOthers:
                // Do nothing
                break;
            default:
                break;
        }

        // player update controls the camera and etc.
        m_player?.OnUpdate();
    }

    public PlayerState GetState()
    {
        return m_state;
    }

    public Substate GetSubstate()
    {
        return m_subState;
    }

    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode)
    {
        m_gdc = GameDataCollection.GetInstance();
        m_player = GameObject.Find("PlayerPrefab").GetComponent<Player>();
    }

    private PlayerState m_state = PlayerState.Play;
    private Substate m_subState = Substate.WaitingForOthers;
    private Player m_player = null;
    private GameDataCollection m_gdc = null;
}
