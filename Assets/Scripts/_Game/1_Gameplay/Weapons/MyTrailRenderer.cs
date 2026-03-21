using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyTrailRenderer : MonoBehaviour
{
    private Transform tipTransform;
    private Transform baseTransform;

    public float minVertexDistance = 0.1f;
    private float minVertexDistanceSqr;
    public float trailLifeTime = 0.2f;
    public int subdivisions = 5;// 每段之间插值的点数
    //public bool isEmitting = false;//是否清空网格
    public bool isPlaying = true;//是否添加顶点

    private struct TrailSnapshot
    {
        public Vector3 tipPos;
        public Vector3 basePos;
        public float timeStamp;
    }

    List<TrailSnapshot> snapshotList = new List<TrailSnapshot>();// 原始轨迹快照列表
    List<TrailSnapshot> smoothedList = new List<TrailSnapshot>();// 平滑生成的轨迹快照列表
    private Mesh trailMesh;
    private MeshFilter meshFilter;


    /// <summary>
    /// 实例化刀光
    /// </summary>
    /// <param name="tipTransform">刀光的两端</param>
    /// <param name="baseTransform"></param>
    public void InitMyTrailRenderer(Transform tipTransform, Transform baseTransform)
    {
        this.tipTransform = tipTransform;
        this.baseTransform = baseTransform;
    }

    public Mesh GetTrailMesh()
    {
        return trailMesh;
    }

    private void OnEnable()
    {
        snapshotList.Clear();
        smoothedList.Clear();
        trailMesh.Clear();
    }

    private void OnDisable()
    {
        snapshotList.Clear();
        smoothedList.Clear();
        trailMesh.Clear();
    }

    private void Awake()
    {
        minVertexDistanceSqr = minVertexDistance * minVertexDistance;
        trailMesh = new Mesh();
        meshFilter = this.GetComponent<MeshFilter>();
        meshFilter.mesh = trailMesh;
    }

    void LateUpdate()
    {
        //if (!isEmitting)
        //{
        //    snapshotList.Clear();
        //    smoothedList.Clear();
        //    trailMesh.Clear();
        //    return;
        //}

        if (isPlaying)
        {
            AddNewTrailSnapshot();
        }

        RemoveOldTrailSnapshot();

        SmoothSnapshotList();

        CreateMesh();
    }

    /// <summary>
    /// 添加轨迹快照
    /// </summary>
    private void AddNewTrailSnapshot()
    {
        TrailSnapshot trailSnapshot = new TrailSnapshot() { tipPos = tipTransform.position, basePos = baseTransform.position, timeStamp = Time.time };  
        if (snapshotList.Count == 0 || (trailSnapshot.tipPos - snapshotList[snapshotList.Count - 1].tipPos).sqrMagnitude > minVertexDistanceSqr)
        {
            snapshotList.Add(trailSnapshot);
        }
    }

    /// <summary>
    /// 移除过期轨迹快照
    /// </summary>
    private void RemoveOldTrailSnapshot()
    {
        while (snapshotList.Count > 0 && Time.time - snapshotList[0].timeStamp > trailLifeTime)
        {
            snapshotList.RemoveAt(0);
        }
    }

    /// <summary>
    /// 平滑生成轨迹快照
    /// </summary>
    private void SmoothSnapshotList()
    {
        smoothedList.Clear();
        if (snapshotList.Count < 2) return;

        smoothedList.Clear();
        for (int i = 0; i < snapshotList.Count - 1; i++)
        {
            smoothedList.Add(snapshotList[i]);
            for (float j = 0; j < subdivisions; j++)
            {
                Vector3 newTipPos = MathUtils.Catmull_Rom(snapshotList[i - 1 < 0 ? 0 : i - 1].tipPos, snapshotList[i].tipPos, snapshotList[i + 1].tipPos, snapshotList[i + 2 >= snapshotList.Count ? i + 1 : i + 2].tipPos, j / subdivisions);
                Vector3 newBasePos = MathUtils.Catmull_Rom(snapshotList[i - 1 < 0 ? 0 : i - 1].basePos, snapshotList[i].basePos, snapshotList[i + 1].basePos, snapshotList[i + 2 >= snapshotList.Count ? i + 1 : i + 2].basePos, j / subdivisions);
                smoothedList.Add(new TrailSnapshot() { tipPos = newTipPos, basePos = newBasePos, timeStamp = snapshotList[i].timeStamp });
            }
        }
        smoothedList.Add(snapshotList[snapshotList.Count - 1]);
    }

    /// <summary>
    /// 生成网格
    /// </summary>
    private void CreateMesh()
    {
        trailMesh.Clear();
        if (smoothedList.Count < 2)
        {
            return;
        }

        Vector3[] meshPoints = new Vector3[smoothedList.Count * 2];
        for (int i = 0; i < meshPoints.Length; i++)
        {
            meshPoints[i] = i % 2 == 0 ? smoothedList[i / 2].tipPos : smoothedList[i / 2].basePos;// 偶数顶点是tip，奇数顶点是base
        }
        trailMesh.vertices = meshPoints;// 设置顶点

        int[] triangles = new int[(smoothedList.Count - 1) * 6];//mesh.triangles[i] = ... 会触发一次昂贵的底层数据拷贝

        for (int i = 0; i < smoothedList.Count - 1; i++)
        {
            triangles[i * 6 + 0] = i * 2 + 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 0;

            triangles[i * 6 + 3] = i * 2 + 3;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 2;
        }
        trailMesh.triangles = triangles;// 设置顶点顺序

        Vector2[] uvs = new Vector2[meshPoints.Length];
        for (int i = 0; i < smoothedList.Count; i++)
        {
            uvs[i * 2 + 0] = new Vector2((float)i / (smoothedList.Count - 1), 1);// tip顶点UV
            uvs[i * 2 + 1] = new Vector2((float)i / (smoothedList.Count - 1), 0);// base顶点UV
        }
        trailMesh.uv = uvs;// 设置UV
        trailMesh.RecalculateBounds();//更新包围盒，以防摄像机裁剪掉
    }
}
