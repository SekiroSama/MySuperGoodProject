using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    
    private void Awake()
    {
        this.Init();
    }

    private void Init()
    {
        _instance = this;
    }



    private void Start()
    {
    }

    void Update()
    {
    }

    /// <summary>
    /// 设置应用目标帧率
    /// </summary>
    /// <param name="frameRate"></param>
    public void SetApplicationTargetFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
    }

}
