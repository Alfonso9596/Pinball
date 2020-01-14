using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OOB_Objects : MonoBehaviour
{
    private OOB_Bounds orientedBounds;
    public OOB_Bounds OrientedBounds
    {
        get { return orientedBounds; }
    }
    private MeshFilter meshFilter;

    void Start()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        orientedBounds = new OOB_Bounds(transform, meshFilter);
    }

    void Update()
    {
        orientedBounds.Update();
    }
}
