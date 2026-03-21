using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float Atk;
    public float PoiseAtk;

    List<int> whiteList = new List<int>();
    private Coroutine dissolveCoroutine;//设置武器溶解效果协程
    public bool hideWeapon = false;
    public Transform tipTransform;
    public Transform baseTransform;
    public bool isHit;
    private GameObject trailObj; 
    private GameObject hitObj;


    private void OnTriggerEnter(Collider other)
    {

    }
    public void WeaponTrailOn()
    {

    }
    public void WeaponTrailOff()
    {

    }

    /// <summary>
    /// 动画事件：武器碰撞开启
    /// </summary>
    public void WeaponColliderOn()
    {
        this.GetComponent<Collider>().enabled = true;
        whiteList.Clear();
    }

    /// <summary>
    /// 动画事件：武器碰撞关闭
    /// </summary>
    public void WeaponColliderOff()
    {
        this.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// 动画事件：开始显示武器
    /// </summary>
    public void WeaponRedissolveValStart()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);
        dissolveCoroutine = StartCoroutine(SetWeaponDissolveVal(0, 0.1f, 0));
    }

    /// <summary>
    /// 动画事件：开始溶解隐藏武器
    /// </summary>
    public void WeaponDissolveValStart()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);
        dissolveCoroutine = StartCoroutine(SetWeaponDissolveVal(1, 2f, 2f));
    }
 
    /// <summary>
    /// 设置武器溶解效果
    /// </summary>
    /// <param name="targetDissolveVal">目标溶解值</param>
    /// <param name="transitionDuration">溶解持续时间</param>
    /// <param name="startTime">溶解开始时间</param>
    /// <returns></returns>
    private IEnumerator SetWeaponDissolveVal(float targetDissolveVal, float transitionDuration = 0.1f, float startTime = 0f)
    {
        if (hideWeapon)
        {
            float dissolveVal = Shader.GetGlobalFloat("_DissolveVal");
            float dissolveTimer = 0f;
            while (dissolveTimer - startTime < transitionDuration)
            {
                dissolveTimer += Time.deltaTime;
                if(dissolveTimer < startTime)
                {
                    yield return null;
                }
                else
                {
                    Shader.SetGlobalFloat("_DissolveVal", Mathf.Lerp(dissolveVal, targetDissolveVal, (dissolveTimer - startTime) / transitionDuration));
                    yield return null;
                }
            }
            Shader.SetGlobalFloat("_DissolveVal", targetDissolveVal);
        }
        else
        {
            Shader.SetGlobalFloat("_DissolveVal", 0);
        }
    }
}

