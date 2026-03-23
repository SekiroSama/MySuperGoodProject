using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonAutoMono<CameraManager>
{
    public CinemachineFreeLook camFreeLook;
    public CinemachineVirtualCamera Lock_VirtualCamera;
    public CinemachineCollider camCollider;
    private Material FX_RadialBlur_FullScreen_Material;//径向模糊材质球

    private bool isLockingOnTriggered = false;//是否锁定

    public void Init(CinemachineCollider camCollider, CinemachineFreeLook camFreeLook, Material FX_RadialBlur_FullScreen_Material)
    {
        this.camCollider = camCollider;
        this.FX_RadialBlur_FullScreen_Material = FX_RadialBlur_FullScreen_Material;
    }

    void Awake()
    {
        camFreeLook = GameObject.Find("FreeLook Camera").GetComponent<CinemachineFreeLook>();
        Lock_VirtualCamera = GameObject.Find("Lock Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        camCollider = camFreeLook.GetComponent<CinemachineCollider>();
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    public void LockOrUnlock()
    {
        if(isLockingOnTriggered) { //锁定了则解锁
            Lock_VirtualCamera.Priority = 9;//切换到自由摄像机
            isLockingOnTriggered = false;
        }
        else // 没锁定则锁定
        {
            Lock_VirtualCamera.Priority = 11;//切换到锁定摄像机
            isLockingOnTriggered = true;
        }
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
