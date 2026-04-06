using UnityEngine;

public interface StateBase
{
    public void OnEnter();

    public void OnUpdate();

    public void OnExit();

    public PlayerState GetState();
}
