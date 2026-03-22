using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController CC;
    [HideInInspector]
    public StateMachine stateMachine;

    public Animator animator;


    public float moveSpeed;
    public float rotatSpeed;
    private Tweener rotateTween;//旋转tween


    private void Start()
    {
        CC = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        stateMachine = new StateMachine(this);
        stateMachine.Initialize<PlayerMovementState>();
    }

    private void Update()
    {
        //状态帧更新
        stateMachine.OnUpdate();
        //处理角色重力
        HandGravity();
    }

    /// <summary>
    /// 处理角色重力
    /// </summary>
    private void HandGravity()
    {
        CC.Move(Physics.gravity * Time.deltaTime);
    }

    /// <summary>
    /// 让角色旋转移动
    /// </summary>
    /// <param name="input">输入方向</param>
    public void Move(Vector2 input)
    {

    }

    /// <summary>
    /// 根据镜头方向修正角色旋转
    /// </summary>
    /// <param name="cameraDir">镜头方向</param>
    public void FaceInput(Vector3 cameraDir)
    {
        FaceDirection(cameraDir);
    }

    /// <summary>
    /// 让角色面向moveDir
    /// </summary>
    /// <param name="faceDir"></param>
    private void FaceDirection(Vector3 faceDir)
    {
        if (faceDir.sqrMagnitude <= 0.01f) return;

        rotateTween?.Kill();
        rotateTween = transform.DORotate(Quaternion.LookRotation(faceDir).eulerAngles, 0.2f).SetEase(Ease.OutQuad);

        // this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(faceDir), rotatSpeed * Time.deltaTime);
    }



    /// <summary>
    /// 处理角色根运动
    /// </summary>
    // private void OnAnimatorMove()
    // {
    //     CC.Move(animator.deltaPosition);
    //     this.transform.rotation *= animator.deltaRotation;
    // }

    /// <summary>
    /// 让角色水平方向混合树动画参数更新
    /// </summary>
    /// <param name="speed"></param>
    public void UpdateHorLocomotion(float speed)
    {
        animator.SetFloat(AnimationConfig_UnityChan.Parameters.ZSpeed, speed);
    }
    /// <summary>
    ///  让角色垂直方向混合树动画参数更新
    /// </summary>
    /// <param name="speed"></param>
    public void UpdateVerLocomotion(float speed)
    {
        animator.SetFloat(AnimationConfig_UnityChan.Parameters.XSpeed, speed);
    }

    #region 角色动画状态

    /// <summary>
    /// 让角色更新动画
    /// </summary>
    /// <param name="animHash"></param>
    /// <param name="fadeTime"></param>
    public void PlayAnimation(int animHash, float fadeTime = 0.1f)
    {
        animator.CrossFadeInFixedTime(animHash, fadeTime);// 参数2：过渡时间，0.1秒通常是 ARPG 的黄金标准
    }

    #endregion
}
