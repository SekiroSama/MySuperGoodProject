using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationConfig
{
    public static class Parameters
    {
        public static readonly int Speed = Animator.StringToHash("Speed"); 
        public static readonly int Die = Animator.StringToHash("Die");
        public static readonly int IsDowned = Animator.StringToHash("IsDowned"); 
        public static readonly int isSleeping = Animator.StringToHash("isSleeping"); 
        public static readonly int isBackAwaying = Animator.StringToHash("isBackAwaying");
        public static readonly int DragonShout = Animator.StringToHash("DragonShout");
        //public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        public static readonly int ProjectileAttack = Animator.StringToHash("ProjectileAttack");
        public static readonly int IsFireballShooting = Animator.StringToHash("IsFireballShooting");
        public static readonly int MeleeAttack = Animator.StringToHash("MeleeAttack");
        public static readonly int IsBasicAttacking = Animator.StringToHash("IsBasicAttacking");
        public static readonly int IsTailAttacking = Animator.StringToHash("IsTailAttacking");
    }

    public static class FatFatDragonSettings
    {
        public const float FatFatDragonWalkSpeedRatio = 0.33f;
        public const float FatFatDragonRunSpeedRatio = 1f;
        public static readonly List<int> FatFatDragonMeleeAttackList = new List<int>()
        {
            Parameters.IsBasicAttacking,
            Parameters.IsTailAttacking,
        };
    }
}
