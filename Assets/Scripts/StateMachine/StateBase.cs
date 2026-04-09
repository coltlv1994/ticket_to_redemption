using UnityEngine;
using UnityEngine.SceneManagement;

public interface StateBase
{
    public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_loadMode);

    public void OnEnter();

    public void OnUpdate();

    public void OnExit();

    public PlayerState GetState();

    public Substate GetSubstate();
}
