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
                m_subState = SubState.TurnStart;
                break;
            case SubState.TurnStart:
                // Do nothing
                break;
            case SubState.Turn:
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
}
