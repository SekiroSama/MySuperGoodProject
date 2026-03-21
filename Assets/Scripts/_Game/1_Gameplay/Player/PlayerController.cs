using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController CC;
    [HideInInspector]
    public StateMachine stateMachine;

    private Animator animator;


    public float moveSpeed;
    public float rotatSpeed;


    private bool UseRootMotion;

    private void Start()
    {

        CC = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        stateMachine = new StateMachine(this);
        stateMachine.Initialize<PlayerIdleState>();
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
        Vector3 moveDir = GetCameraRelativeDir(input);

        FaceDirection(moveDir);
        if (InputManager.Instance.CurrentInput.isMoveing)
        {
            CC.Move(moveDir * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 根据输入方向让角色面向该方向
    /// </summary>
    /// <param name="input">输入方向</param>
    public void FaceInput(Vector2 input)
    {
        Vector3 moveDir = GetCameraRelativeDir(input);
        FaceDirection(moveDir);
    }

    /// <summary>
    /// 让角色面向moveDir
    /// </summary>
    /// <param name="moveDir"></param>
    private void FaceDirection(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude <= 0.01f) return;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(moveDir), rotatSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 计算相机相对方向的移动向量
    /// </summary>
    /// <param name="input">输入方向</param>
    /// <returns></returns>
    private Vector3 GetCameraRelativeDir(Vector2 input)
    {
        if (input.sqrMagnitude <= 0.01) return Vector3.zero;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDir = camRight * input.x + camForward * input.y;
        return moveDir.normalized;
    }

    /// <summary>
    /// 处理角色根运动
    /// </summary>
    private void OnAnimatorMove()
    {
        if (!UseRootMotion) return;
        CC.Move(animator.deltaPosition);
        this.transform.rotation *= animator.deltaRotation;
    }

    /// <summary>
    /// 让角色水平方向混合树动画参数更新
    /// </summary>
    /// <param name="speed"></param>
    public void UpdateHorLocomotion(float speed)
    {
        animator.SetFloat(AnimationConfig_UnityChan.Parameters.XSpeed, speed);
    }
    /// <summary>
    ///  让角色垂直方向混合树动画参数更新
    /// </summary>
    /// <param name="speed"></param>
    public void UpdateVerLocomotion(float speed)
    {
        animator.SetFloat(AnimationConfig_UnityChan.Parameters.YSpeed, speed);
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
