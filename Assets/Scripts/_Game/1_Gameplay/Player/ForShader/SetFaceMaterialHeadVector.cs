using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetFaceMaterialHeadVector : MonoBehaviour
{
    public Transform Head;
    public Transform HeadForward;
    public Transform HeadRight;
    public Transform HeadUp;
    public Material FaceMaterial;

    void Update()
    {
        Vector3 headForward = Vector3.Normalize(HeadForward.position - Head.position);
        Vector3 headRight = Vector3.Normalize(HeadRight.position - Head.position);
        Vector3 headUp = Vector3.Normalize(HeadUp.position - Head.position);

        FaceMaterial.SetVector("_HeadForward", headForward);
        FaceMaterial.SetVector("_HeadRight", headRight);
        FaceMaterial.SetVector("_HeadUp", headUp);
    }
}
