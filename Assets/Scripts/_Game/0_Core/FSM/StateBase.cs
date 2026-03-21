using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase
{
    protected PlayerController owner;
    protected StateMachine stateMachine;

    public void Initialize(PlayerController owner, StateMachine machine)
    {
        this.owner = owner;
        this.stateMachine = machine;
    }

    #region 状态生命周期
    public virtual void OnEnter()
    {

    }
    public virtual void OnUpdate()
    {

    }
    public virtual void OnExit()
    {

    }
    #endregion
}
