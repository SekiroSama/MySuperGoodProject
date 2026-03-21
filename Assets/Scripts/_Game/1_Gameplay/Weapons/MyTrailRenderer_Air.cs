using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyTrailRenderer_Air : MonoBehaviour
{
    private Mesh clonedMesh;
    private MeshFilter clonedMeshFilter;
    void Start()
    {
        clonedMesh = this.GetComponentInParent<MyTrailRenderer>().GetTrailMesh();
        clonedMeshFilter = this.GetComponent<MeshFilter>();
        clonedMeshFilter.mesh = clonedMesh;
    }

    void LateUpdate()
    {

    }
}
