using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonAutoMono<CameraManager>
{
    public CinemachineFreeLook camFreeLook;
    public CinemachineCollider camCollider;
    private Material FX_RadialBlur_FullScreen_Material;//径向模糊材质球


    public void Init(CinemachineCollider camCollider, CinemachineFreeLook camFreeLook, Material FX_RadialBlur_FullScreen_Material)
    {
        this.camCollider = camCollider;
        this.camFreeLook = camFreeLook;
        this.FX_RadialBlur_FullScreen_Material = FX_RadialBlur_FullScreen_Material;
    }


    public void OnStart()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    int lastTimerId = -1;
    /// <summary>
    /// 开启径向模糊
    /// </summary>
    /// <param name="durTime">持续时间ms</param>
    /// <param name="BlurStrength">模糊强度 0.05左右</param>
    public void RadialBlurStart(int durTime, float BlurStrength = 0.05f)
    {
        FX_RadialBlur_FullScreen_Material.SetFloat("_BlurStrength", BlurStrength);
        if(lastTimerId != -1)
        {
            TimerMgr.Instance.RemoveTimer(lastTimerId);
        }
        lastTimerId = TimerMgr.Instance.CreateTimer(false, durTime, RadialBlurEnd);
    }

    /// <summary>
    /// 关闭径向模糊
    /// </summary>
    private void RadialBlurEnd()
    {
        FX_RadialBlur_FullScreen_Material.SetFloat("_BlurStrength", 0);
    }

}
