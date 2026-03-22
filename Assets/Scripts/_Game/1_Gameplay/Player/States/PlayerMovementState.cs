using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static AnimationConfig_UnityChan;

public class PlayerMovementState : StateBase
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Vector2 input = InputManager.Instance.CurrentInput.MoveVector;


        if(input.sqrMagnitude < 0.01f)//如果输入很小就当做没有输入，直接切换到Idle状态
        {
            owner.animator.SetFloat(Parameters.XSpeed, 0);
            owner.animator.SetFloat(Parameters.ZSpeed, 0);
        }
        else
        {
            //只要在前进，就要根据镜头方向修正角色旋转
            if(input.y > 0.01f)
            {
                owner.FaceInput(new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z));
            }

            if(InputManager.Instance.CurrentInput.IsRunning) //如果按下了Shift 就切换到跑步状态
            {
                float target = input.x * StateParametersMul.Movement_Run;
                DOTween.To(() => owner.animator.GetFloat(Parameters.XSpeed), 
                            x => owner.animator.SetFloat(Parameters.XSpeed, x), 
                            target, 0.2f);

                target = input.y * StateParametersMul.Movement_Run;
                DOTween.To(() => owner.animator.GetFloat(Parameters.ZSpeed), 
                            x => owner.animator.SetFloat(Parameters.ZSpeed, x), 
                            target, 0.2f);
            }
            else //否则就是走路状态
            {
                float target = input.x * StateParametersMul.Movement_Walk;
                DOTween.To(() => owner.animator.GetFloat(Parameters.XSpeed), 
                            x => owner.animator.SetFloat(Parameters.XSpeed, x), 
                            target, 0.2f);

                target = input.y * StateParametersMul.Movement_Walk;
                DOTween.To(() => owner.animator.GetFloat(Parameters.ZSpeed), 
                            x => owner.animator.SetFloat(Parameters.ZSpeed, x), 
                            target, 0.2f);
            }
        }

    }
}
