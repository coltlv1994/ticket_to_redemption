using System.Security.Cryptography;
using UnityEngine;

public class StateMachine
{
    public void Init()
    {

    }

    public void ChangeState(StateBase newState)
    {
        //m_isChangingState = true;

        if (m_currentState != null)
        {
            m_currentState.OnExit();
        }

        m_currentState = newState;
        m_currentState.OnEnter();
    }

    public void Update()
    {
        //if (m_isChangingState)
        //{
        //    return;
        //}
        
        m_currentState?.OnUpdate();
    }

    public PlayerState GetCurrentState()
    {
        return m_currentState?.GetState() ?? PlayerState.Intro;
    }

    private StateBase m_currentState;
    private StateBase m_nextState;
    //private bool m_isChangingState = false;
}
