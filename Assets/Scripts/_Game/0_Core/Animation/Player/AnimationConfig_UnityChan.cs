using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationConfig_UnityChan
{
    /// <summary>
    /// 动画参数的HashID
    /// </summary>
    public static class Parameters
    {
        public static readonly int XSpeed = Animator.StringToHash("horizontal");

        public static readonly int ZSpeed = Animator.StringToHash("vertical");
    }

    public static class StateHashes
    {
        
    }

    /// <summary>
    /// 状态参数系数
    /// </summary>
    public static class StateParametersMul
    {
        public const int Movement_Idle = 0;
        public const float Movement_Walk = 1.482197f;
        public const float Movement_Run = 3.412789f;
    }

    public static readonly Dictionary<System.Type, int[]> StateToParameters = new Dictionary<System.Type, int[]>
    {
        { typeof(PlayerMovementState), new int[] { Parameters.ZSpeed } },
    };

    public static class TransitionSettings
    {
        public const float NormalTransitionDuration = 0.1f;
        public const float AttackTransitionDuration = 0.05f;
        public const float AttackOverTransitionDuration = 0.5f;
        public const float SuperTransitionDuration = 1f;
    }

}
