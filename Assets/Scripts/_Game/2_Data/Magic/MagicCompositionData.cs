using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicComposition", menuName = "SO/Magic/MagicComposition")]
public class MagicCompositionData : ScriptableObject
{
    [Header("基础数据")]
    public string magicName;

    [Header("交互数据")]
    public ComboInteracionConfig ComboInteracionConfig;

    [Header("攻击检测数据")]
    public AttackDetectConfig AttackDetectConfig;
}

[Serializable]
public class ComboInteracionConfig
{
    
}


[Serializable]
public class AttackDetectConfig
{
    
}
 