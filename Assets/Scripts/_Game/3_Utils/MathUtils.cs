using UnityEngine;

/// <summary>
/// 数学工具类，静态类不能被实例化体现唯一性
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Catmull-Rom插值算法计算插值点
    /// </summary>
    /// 最终坐标 = 0.5 * (A项 + B项 + C项 + D项)。
    /// 四项分解逻辑：
    /// A项（常数项）：只与起点 P1 有关。
    /// 逻辑：2 * P1
    /// B项（一次项）：与 t 成正比，受 P0 和 P2 影响（决定切线）。
    /// 逻辑：(P2 - P0) * t
    /// C项（二次项）：与 tt 成正比，受所有点影响。
    /// 逻辑：(2 * P0 - 5 * P1 + 4 * P2 - P3) * tt
    /// D项（三次项）：与 ttt 成正比，受所有点影响。
    /// 逻辑：(-P0 + 3 * P1 - 3 * P2 + P3) * ttt
    /// <param name="prevPos">前一个辅助点</param>
    /// <param name="startPos">当前线段起点</param>
    /// <param name="endPos">当前线段终点</param>
    /// <param name="nextPos">后一个辅助点</param>
    /// <param name="t">插值进度</param>
    /// <returns>插值位置</returns>
    public static Vector3 Catmull_Rom(Vector3 prevPos, Vector3 startPos, Vector3 endPos, Vector3 nextPos, float t)
    {
        float tt = t * t;
        float ttt = tt * t;
        
        Vector3 partA = 2f * startPos;
        Vector3 partB = (endPos - prevPos) * t;
        Vector3 partC = (2f * prevPos - 5f * startPos + 4f * endPos - nextPos) * tt;
        Vector3 partD = (-prevPos + 3f * startPos - 3f * endPos + nextPos) * ttt;

        return (partA + partB + partC + partD) * 0.5f;
    }
}
