using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public StateBase CurrentState;
    public StateBase PreviousState;
    public PlayerController owner; 
    Dictionary<System.Type, StateBase> States = new Dictionary<System.Type, StateBase>();

    public StateMachine(PlayerController owner)
    {
        this.owner = owner;
    }

    public void Initialize<T>() where T : StateBase, new()
    {
        ChangeState<T>();
    }

    public void OnUpdate()
    {
        CurrentState.OnUpdate();
    }

    /// <summary>
    /// 状态切换
    /// </summary>
    /// <typeparam name="T">状态类</typeparam>
    public void ChangeState<T>() where T: StateBase, new()
    {
        if(typeof(T) == CurrentState?.GetType())
        {
            return;
        }

        if (!States.ContainsKey(typeof(T)))
        {
            StateBase newstate = new T();
            newstate.Initialize(owner, this);
            States.Add(typeof(T), newstate);
        }

        CurrentState?.OnExit();
        PreviousState = CurrentState;
        CurrentState = States[typeof(T)];
        CurrentState.OnEnter();
    }
}
