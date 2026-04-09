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
        PlayerController pc = PlayerController.GetInstance();
        pc.SetPlayerState(PlayerState.Intro);
        pc.SetPlayerSubstate(Substate.NOT_IN_PLAY_STATE);
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

    public Substate GetSubstate()
    {
        return Substate.NOT_IN_PLAY_STATE;
    }

    private PlayerState m_state = PlayerState.Intro;
}
