using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroState : StateBase
{
    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode)
    {

    }

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
