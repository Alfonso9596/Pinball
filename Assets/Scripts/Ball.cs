using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector3 movement;
    private Vector3 gravity;
    private static float maxSpeed;

    [SerializeField]
    [Range(0f,3f)]
    private float bounciness = 1f;

    public Vector3 Center
    {
        get
        {
            return transform.position;
        }
    }

    public Vector3 Movement
    {
        get
        {
            return movement;
        }
    }

    public Vector3 Gravity
    {
        get
        {
            return gravity;
        }
        set
        {
            gravity = value;
        }
    }

    public float Bounciness
    {
        get
        {
            return bounciness;
        }
    }

    private MeshFilter meshFilter;
    private float meshRadius;

    public float Radius
    {
        get
        {
            float highestAxisScale = float.NegativeInfinity;
            if (transform.lossyScale.x > highestAxisScale)
                highestAxisScale = transform.lossyScale.x;
            if (transform.lossyScale.y > highestAxisScale)
                highestAxisScale = transform.lossyScale.y;
            if (transform.lossyScale.z > highestAxisScale)
                highestAxisScale = transform.lossyScale.z;
            if (meshFilter)
            {
                return meshRadius * (transform.lossyScale.x / 2) * highestAxisScale;
            } else
            {
                return (transform.lossyScale.x / 2) * highestAxisScale;
            }
        }
    }

    private void Awake()
    {
        gravity = new Vector3(0, -9.81f, -7f);
        movement = Vector3.zero;
        maxSpeed = 20f;
        CalculateMeshRadius();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movement += gravity * Time.fixedDeltaTime;

        if (movement.magnitude > maxSpeed)
            movement = movement.normalized * maxSpeed;

        transform.Translate(movement * Time.fixedDeltaTime, Space.World);

    }

    private void CalculateMeshRadius()
    {
        if (meshFilter)
        {
            meshRadius = 0f;
            foreach (Vector3 vertex in meshFilter.sharedMesh.vertices)
            {
                meshRadius = meshRadius < vertex.magnitude ? vertex.magnitude : meshRadius;
            }
        }
    }

    public void AddVelocity(Vector3 vlc)
    {
        movement += vlc;
    }

    public void AddAcceleration(Vector3 acc)
    {
        movement += acc * Time.fixedDeltaTime;
    }

    public void BounceOff(Vector3 normal, float bounciness)
    {
        float bounceAngle = Vector3.Angle(movement, normal);
        movement = Quaternion.AngleAxis(bounceAngle, normal) * movement;
        movement *= -bounciness;
        movement = new Vector3(movement.x, 0, movement.z);
    }

}
