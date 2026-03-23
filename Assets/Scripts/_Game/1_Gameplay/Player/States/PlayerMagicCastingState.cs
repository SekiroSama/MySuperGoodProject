using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using static AnimationConfig_Player;

public class PlayerMagicReleaseState : StateBase
{
    private Tweener tweener_SetLayerWeight;//层权重tween
    private Tweener tweener_PushPower;//推力tween

    public override void OnEnter()
    {
        base.OnEnter();
        //释放给自身一个向后推力
        float pushPower = 10f;
        tweener_PushPower?.Kill();
        tweener_PushPower = DOTween.To(() => pushPower, x => pushPower = x, 0f, 0.5f)
            .OnUpdate(() => {
                owner.CC.Move(-owner.transform.forward * pushPower * Time.deltaTime);
            });
    }

    public override void OnUpdate()
    {
        owner.stateMachine.ChangeState<PlayerMovementState>();
        owner.PlayAnimation(StateHashes.Movement);

    }

    public override void OnExit()
    {
        tweener_SetLayerWeight?.Kill();
        tweener_SetLayerWeight = DOTween.To(() => owner.animator.GetLayerWeight(1), 
                                        x => owner.animator.SetLayerWeight(1, x), 
                                        0f, 0.2f);
    }
}
