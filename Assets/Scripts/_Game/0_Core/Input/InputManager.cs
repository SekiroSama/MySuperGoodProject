using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : SingletonAutoMono<InputManager>
{

    public struct PlayerInputData
    {
        public Vector2 InputVector;
        public Vector2 InputVectorToCamera;
        public bool IsRunning;
        public bool IsLockingOn;
        public bool IsCasting;
    }

    PlayerInputData _playerInputData;
    public PlayerInputData CurrentInput => _playerInputData;

    [HideInInspector]
    public bool isReadIngPlayerInput = true;


    public void Awake()
    {
        _playerInputData = new PlayerInputData();
    }

    public void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;//CursorLockMode影响鼠标运动逻辑，Locked大概率会隐藏但在某些环境不行
        //Cursor.visible = false;//确保隐藏
    }

    public void Update()
    {
        // CheckAndSetCursorEnable();

        if (isReadIngPlayerInput)
        {
            UpdateInput();
        }
    }

    /// <summary>
    /// 检查并设置鼠标
    /// </summary>
    private void CheckAndSetCursorEnable()
    {
        //激活鼠标
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CameraManager.Instance.camFreeLook.m_XAxis.m_MaxSpeed = 0f;
            CameraManager.Instance.camFreeLook.m_YAxis.m_MaxSpeed = 0f;
            isReadIngPlayerInput = false;
            ResetInputData();
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            CameraManager.Instance.camFreeLook.m_XAxis.m_MaxSpeed = 400f;
            CameraManager.Instance.camFreeLook.m_YAxis.m_MaxSpeed = 3f;
            Cursor.lockState = CursorLockMode.Locked;//CursorLockMode影响鼠标运动逻辑，Locked大概率会隐藏但在某些环境不行
            Cursor.visible = false;//确保隐藏
            isReadIngPlayerInput = true;
        }
    }

    /// <summary>
    /// 更新输入
    /// </summary>
    private void UpdateInput()
    {
        _playerInputData.InputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _playerInputData.InputVectorToCamera= GetCameraRelativeDir(_playerInputData.InputVector);
        
        if (Input.GetKeyDown(KeyCode.LeftShift))//按下Shift 键切换跑步状态
        {
            _playerInputData.IsRunning = !_playerInputData.IsRunning;
        }
        if(Input.GetMouseButtonDown(2))//按下鼠标中键 锁定/解锁目标
        {
            _playerInputData.IsLockingOn = !_playerInputData.IsLockingOn;
            CameraManager.Instance.LockOrUnlock();
        }
        if (Input.GetMouseButtonDown(0))//按下鼠标左键 开始吟唱魔法
        {
            _playerInputData.IsCasting = true;
        }
        if (Input.GetMouseButtonUp(0))//松开鼠标左键 结束吟唱魔法
        {
            _playerInputData.IsCasting = false;
        }
    }

    /// <summary>
    /// 计算相机相对方向的移动向量
    /// </summary>
    /// <param name="input">输入方向</param>
    /// <returns></returns>
    private Vector2 GetCameraRelativeDir(Vector2 input)
    {
        if (input.sqrMagnitude <= 0.01) return Vector2.zero;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDir = camRight * input.x + camForward * input.y;
        return new Vector2(moveDir.normalized.x, moveDir.normalized.z);
    }

    /// <summary>
    /// 重置输入数据
    /// </summary>
    private void ResetInputData()
    {
        _playerInputData.InputVectorToCamera = Vector2.zero;
        _playerInputData.IsRunning = false;
    }
}
