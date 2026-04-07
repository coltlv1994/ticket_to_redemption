using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayState : StateBase
{
    public enum SubState
    {
        GameStart,
        TurnStart,
        Turn,
        TurnEnd,
        WaitingForOthers
    }

    public void OnEnter()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        GameDataCollection.GetInstance().DrawCardsToDesk(5);

        switch (m_subState)
        {
            case SubState.GameStart:
                // preparation work
                GameDataCollection.GetInstance().DrawCardsToDesk(5);
                m_player?.AddEvent(new DrawCardEvent(4));
                // let AI or other player draw, too

                // draw ticket
                m_player?.AddEvent(new DrawTicketEvent(TicketType.NORMAL, 3));
                m_player?.AddEvent(new DrawTicketEvent(TicketType.LONG, 2));
                // let AI or other player draw, too

                // Game start, need some logic to decide who goes first
                m_subState = SubState.TurnStart;
                break;
            case SubState.TurnStart:
                // Do nothing
                break;
            case SubState.Turn:
                //m_player?
                // Do nothing
                break;
            case SubState.TurnEnd:
                // Do nothing
                break;
            case SubState.WaitingForOthers:
                // Do nothing
                break;
        }
    }

    public PlayerState GetState()
    {
        return m_state;
    }

    private PlayerState m_state = PlayerState.Play;
    private SubState m_subState = SubState.GameStart;
    private Player m_player = null;
}
