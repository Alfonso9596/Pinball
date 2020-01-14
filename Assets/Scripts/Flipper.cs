using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : OOB_Objects
{
    private float bounciness = 0.8f;

    private MeshRenderer mesh;

    public float Bounciness
    {
        get { return bounciness; }
    }

    public Vector3 GetNormal()
    {
        mesh = GetComponent<MeshRenderer>();

        Vector3 p1 = new Vector3(mesh.bounds.max.x, mesh.bounds.min.y, mesh.bounds.min.z);
        Vector3 p2 = new Vector3(mesh.bounds.min.x, mesh.bounds.max.y, mesh.bounds.min.z);
        Vector3 p3 = new Vector3(mesh.bounds.min.x, mesh.bounds.min.y, mesh.bounds.max.z);

        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;

        return Vector3.Cross(v1, v2);
    }
}
