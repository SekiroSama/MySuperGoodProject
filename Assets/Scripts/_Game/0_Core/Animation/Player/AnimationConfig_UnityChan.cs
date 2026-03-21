using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationConfig_UnityChan
{
    public static class Parameters
    {
        public static readonly int XSpeed = Animator.StringToHash("XSpeed");
        public static readonly int YSpeed = Animator.StringToHash("YSpeed");
    }

    public static class StateHashes
    {
        #region 移动状态

        public static readonly int HorLocomotion = Animator.StringToHash("HorLocomotion");
        public static readonly int VerLocomotion = Animator.StringToHash("VerLocomotion");
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int WalkBack = Animator.StringToHash("WalkBack");
        public static readonly int Rest = Animator.StringToHash("Rest");

        #endregion

        #region 攻击

        public static readonly int Attack01 = Animator.StringToHash("Attack01");
        public static readonly int Attack02 = Animator.StringToHash("Attack02");
        public static readonly int Attack03 = Animator.StringToHash("Attack03");
        public static readonly int ForceAttack = Animator.StringToHash("ForceAttack");
        public static readonly int DefenseAttack = Animator.StringToHash("DefenseAttack");

        #endregion

        #region 特殊状态

        public static readonly int Defense = Animator.StringToHash("Defense");
        public static readonly int Dodge = Animator.StringToHash("Dodge");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        #endregion
    }

    public static class StatePriority
    {
        public const int Idle = 0;
        public const int Run = 1;
        public const int Move = 2;
        public const int Defense = 3;
        public const int Dodge = 3;
        public const int Attack = 4;
        public const int Jump = 5;
    }

    public static readonly Dictionary<System.Type, int[]> StateToParameters = new Dictionary<System.Type, int[]>
    {
        { typeof(PlayerIdleState), new int[] { Parameters.XSpeed } },
    };

    public static class TransitionSettings
    {
        public const float NormalTransitionDuration = 0.1f;
        public const float AttackTransitionDuration = 0.05f;
        public const float AttackOverTransitionDuration = 0.5f;
        public const float SuperTransitionDuration = 1f;
    }

}
