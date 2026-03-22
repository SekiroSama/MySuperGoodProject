using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static AnimationConfig_UnityChan;

public class PlayerMovementState : StateBase
{
    private Tweener rotateTweenVertical;//旋转tween
    private Tweener rotateTweenHorizontal;//旋转tween
    private float target;//tween目标值
    private float target2;//tween目标值

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Vector2 inputVectorToCamera = InputManager.Instance.CurrentInput.InputVectorToCamera;
        Vector2 inputVector = InputManager.Instance.CurrentInput.InputVector;


        if(inputVectorToCamera.sqrMagnitude < 0.01f)//如果输入很小就当做没有输入，直接播放Idle
        {
            target = 0;
            rotateTweenHorizontal?.Kill();
            rotateTweenHorizontal = DOTween.To(() => owner.animator.GetFloat(Parameters.Horizontal), 
                                            x => owner.animator.SetFloat(Parameters.Horizontal, x), 
                                            target, 0.2f);
            rotateTweenVertical?.Kill();
            rotateTweenVertical = DOTween.To(() => owner.animator.GetFloat(Parameters.Vertical), 
                                            x => owner.animator.SetFloat(Parameters.Vertical, x), 
                                            target, 0.2f);
        }
        else//有输入
        {
            if(InputManager.Instance.CurrentInput.IsLockingOn) //锁定时x控制左右，y控制前后
            {
                Vector3 faceDir = LevelManager.Instance.BossLookPos.transform.position - owner.transform.position;
                owner.FaceDirection(new Vector3(faceDir.x, 0, faceDir.z));//面向目标

                if(InputManager.Instance.CurrentInput.IsRunning) //如果按下了Shift 就播放跑步
                {
                    target = inputVector.x * StateParametersMul.Movement_Run;
                    rotateTweenHorizontal?.Kill();
                    rotateTweenHorizontal = DOTween.To(() => owner.animator.GetFloat(Parameters.Horizontal), 
                                x => owner.animator.SetFloat(Parameters.Horizontal, x), 
                                target, 0.2f);

                    target2 = inputVector.y * StateParametersMul.Movement_Run;
                    rotateTweenVertical?.Kill();
                    rotateTweenVertical = DOTween.To(() => owner.animator.GetFloat(Parameters.Vertical), 
                                x => owner.animator.SetFloat(Parameters.Vertical, x), 
                                target2, 0.2f);
                }
                else //否则就是走路状态
                {
                    target = inputVector.x * StateParametersMul.Movement_Walk;
                    rotateTweenHorizontal?.Kill();
                    rotateTweenHorizontal = DOTween.To(() => owner.animator.GetFloat(Parameters.Horizontal), 
                                x => owner.animator.SetFloat(Parameters.Horizontal, x), 
                                target, 0.2f);

                    target = inputVector.y * StateParametersMul.Movement_Walk;
                    rotateTweenVertical?.Kill();
                    rotateTweenVertical = DOTween.To(() => owner.animator.GetFloat(Parameters.Vertical), 
                                x => owner.animator.SetFloat(Parameters.Vertical, x), 
                                target, 0.2f);
                }
            }
            else//没锁定xy的模控制vertical， horizontal不受输入影响
            {
                owner.FaceDirection(new Vector3(inputVectorToCamera.x, 0, inputVectorToCamera.y));

                if(InputManager.Instance.CurrentInput.IsRunning) //如果按下了Shift 就切换到跑步状态
                {
                    target = inputVectorToCamera.magnitude * StateParametersMul.Movement_Run;
                    rotateTweenVertical?.Kill();
                    rotateTweenVertical = DOTween.To(() => owner.animator.GetFloat(Parameters.Vertical), 
                                x => owner.animator.SetFloat(Parameters.Vertical, x), 
                                target, 0.2f);
                }
                else //否则就是走路状态
                {
                    target = inputVectorToCamera.magnitude * StateParametersMul.Movement_Walk;
                    rotateTweenVertical?.Kill();
                    rotateTweenVertical = DOTween.To(() => owner.animator.GetFloat(Parameters.Vertical), 
                                x => owner.animator.SetFloat(Parameters.Vertical, x), 
                                target, 0.2f);
                }
            }
        }
    }
}
