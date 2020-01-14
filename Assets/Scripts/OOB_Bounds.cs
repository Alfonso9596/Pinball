using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OOB_Bounds {

    private Vector3 center;
    public Vector3 Center
    {
        get { return center; }
        set { center = value; }
    }
    [SerializeField]
    private Vector3 size;
    public Vector3 Size
    {
        get { return size; }
        set { size = value; }
    }

    public Vector3 Extents
    {
        get { return size / 2; }
    }

    public Vector3 AxisAlignedMax
    {
        get { return Extents; }
    }

    public Vector3 AxisAlignedMaxWS
    {
        get { return Center + Extents; }
    }

    public Vector3 AxisAlignedMin
    {
        get { return -Extents; }
    }

    public Vector3 AxisAlignedMinWS
    {
        get { return Center - Extents; }
    }

    private MeshFilter meshFilter;

    private Quaternion rotation;
    private Transform transform;

    private Vector3[] corners;

    public Vector3[] Corners
    {
        get { return corners; }
    }

    public OOB_Bounds(Transform target, MeshFilter mesh)
    {
        meshFilter = mesh;
        transform = target;
        rotation = transform.rotation;
        size = Vector3.zero;
        center = transform.position;
        corners = new Vector3[8];

        Create();
    }

    private void Create()
    {
        rotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        if (meshFilter != null)
        {
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float minZ = float.PositiveInfinity;

            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;
            float maxZ = float.NegativeInfinity;

            foreach (Vector3 vertex in meshFilter.sharedMesh.vertices)
            {
                Vector3 vec = transform.TransformPoint(vertex);
                if (vec.x < minX)
                    minX = vec.x;
                else if (vec.x > maxX)
                    maxX = vec.x;
                else if (vec.y < minY)
                    minY = vec.y;
                else if (vec.y > maxY)
                    maxY = vec.y;
                if (vec.z < minZ)
                    minZ = vec.z;
                else if (vec.z > maxZ)
                    maxZ = vec.z;
            }
            Vector3 max = new Vector3(maxX, maxY, maxZ);
            Vector3 min = new Vector3(minX, minY, minZ);
            size = (max - min);
            center = min + center + size.normalized * (size.magnitude / 2f);
        }

        transform.rotation = rotation;
        Update();
    }

    public void Update()
    {
        center = transform.position;
        rotation = transform.rotation;

        var x = Extents.x * transform.localScale.x;
        var y = Extents.y * transform.localScale.y;
        var z = Extents.z * transform.localScale.z;

        corners[0] = (rotation * new Vector3(-x, y, -z)) + center;
        corners[1] = (rotation * new Vector3(-x, y, z)) + center;
        corners[2] = (rotation * new Vector3(x, y, -z)) + center;
        corners[3] = (rotation * new Vector3(x, y, z)) + center;
        corners[4] = (rotation * new Vector3(-x, -y, -z)) + center;
        corners[5] = (rotation * new Vector3(-x, -y, z)) + center;
        corners[6] = (rotation * new Vector3(x, -y, -z)) + center;
        corners[7] = (rotation * new Vector3(x, -y, z)) + center;
    }

    public Vector3 ClosestPoint(Vector3 point)
    {
        Vector3 d = point - center;
        Vector3 q = center;
        float upDist = Vector3.Dot(d, transform.up);

        if (upDist > Extents.y) upDist = Extents.y;
        if (upDist < -Extents.y) upDist = -Extents.y;
        q += upDist * transform.up;

        float rightDist = Vector3.Dot(d, transform.right);
        if (rightDist > Extents.x) rightDist = Extents.x;
        if (rightDist < -Extents.x) rightDist = -Extents.x;
        q += rightDist * transform.right;

        float forwardDist = Vector3.Dot(d, transform.forward);
        if (forwardDist > Extents.z) forwardDist = Extents.z;
        if (forwardDist < -Extents.z) forwardDist = -Extents.z;
        q += forwardDist * transform.forward;
        return q;
    }

    public bool Contains(Vector3 point)
    {
        Vector3 p = transform.InverseTransformPoint(point);

        bool result = false;
        if (p.x > AxisAlignedMin.x && p.x < AxisAlignedMax.x && p.y > AxisAlignedMin.y && p.y < AxisAlignedMax.y && p.z > AxisAlignedMin.z && p.z < AxisAlignedMax.z)
        {
            result = true;
        }
        return result;
    }

    public bool CheckForIntersection(OOB_Objects oobOther)
    {
        Vector3[] SeparateAxis = new Vector3[15] {
            transform.forward,
            transform.right,
            transform.up,
            oobOther.transform.forward,
            oobOther.transform.right,
            oobOther.transform.up,
            Vector3.Cross(transform.forward, oobOther.transform.forward).normalized,
            Vector3.Cross(transform.forward, oobOther.transform.right).normalized,
            Vector3.Cross(transform.forward, oobOther.transform.up).normalized,
            Vector3.Cross(transform.right, oobOther.transform.forward).normalized,
            Vector3.Cross(transform.right, oobOther.transform.right).normalized,
            Vector3.Cross(transform.right, oobOther.transform.up).normalized,
            Vector3.Cross(transform.up, oobOther.transform.forward).normalized,
            Vector3.Cross(transform.up, oobOther.transform.right).normalized,
            Vector3.Cross(transform.up, oobOther.transform.up).normalized
        };

        bool result = true;
        foreach (Vector3 axis in SeparateAxis)
        {
            result = result && IntersectsWhenProjected(Corners, oobOther.OrientedBounds.Corners, axis);
            if (!result)
            {
                return false;
            }
        }

        return result;
    }

    private static bool IntersectsWhenProjected(Vector3[] aCorners, Vector3[] bCorners, Vector3 axis)
    {

        if (axis == Vector3.zero)
            return true;

        float aMin = float.MaxValue;
        float aMax = float.MinValue;

        float bMin = float.MaxValue;
        float bMax = float.MinValue;

        for (int i = 0; i < 8; i++)
        {
            float aDist = Vector3.Dot(aCorners[i], axis);
            aMin = (aDist < aMin) ? aDist : aMin;
            aMax = (aDist > aMax) ? aDist : aMax;
            float bDist = Vector3.Dot(bCorners[i], axis);
            bMin = (bDist < bMin) ? bDist : bMin;
            bMax = (bDist > bMax) ? bDist : bMax;
        }

        float longSpan = Mathf.Max(aMax, bMax) - Mathf.Min(aMin, bMin);
        float sumSpan = aMax - aMin + bMax - bMin;
        return longSpan < sumSpan;
    }
}
