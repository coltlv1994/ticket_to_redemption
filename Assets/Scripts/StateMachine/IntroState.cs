using UnityEngine;


public class IntroState : StateBase
{
    public void OnEnter()
    {
        // Read file
    }

    public void OnExit()
    {
        // Do nothing
        return;
    }

    public void OnUpdate()
    {
        // Do nothing
        return;
    }

    public PlayerState GetState()
    {
        return m_state;
    }

    private PlayerState m_state = PlayerState.Intro;
}
