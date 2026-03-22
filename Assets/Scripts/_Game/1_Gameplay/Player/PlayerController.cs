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

    [HideInInspector]
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
    /// 让角色面向moveDir
    /// </summary>
    /// <param name="faceDir"></param>
    public void FaceDirection(Vector3 faceDir)
    {
        if (faceDir.sqrMagnitude <= 0.01f) return;

        rotateTween?.Kill();
        rotateTween = transform.DORotate(Quaternion.LookRotation(faceDir).eulerAngles, 0.2f).SetEase(Ease.OutQuad);
    }



    /// <summary>
    /// 处理角色根运动
    /// </summary>
    // private void OnAnimatorMove()
    // {
    //     CC.Move(animator.deltaPosition);
    //     this.transform.rotation *= animator.deltaRotation;
    // }

    #region 角色动画状态

    /// <summary>
    /// 让角色更新动画
    /// </summary>
    /// <param name="animHash"></param>
    /// <param name="fadeTime">过渡时间</param>
    public void PlayAnimation(int animHash, float fadeTime = 0.1f)
    {
        animator.CrossFadeInFixedTime(animHash, fadeTime);
    }

    #endregion
}
