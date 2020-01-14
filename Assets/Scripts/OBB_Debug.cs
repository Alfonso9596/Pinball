using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class OBB_Debug : MonoBehaviour
{
    // Exposed to the inspector. Allows to toggle on and off the debug draw
    [SerializeField] private bool show = true;
    // The color of the aabb to be drawn
    [SerializeField] private Color color = Color.red;
    // caching the supplied aabb Component to prevent expensive calls of GetComponent<>
    private OOB_Objects obbComponent;

    [Header("Debug ClosestPoint (on bounds)")]
    [SerializeField] private bool showClosestPoint = true;
    [SerializeField] private Color colorClosestPoint = Color.green;
    [SerializeField] private float pointSize = 0.5f;
    [SerializeField] private Vector3 pointToCheck = Vector3.zero;
    // If object reference is null. we will fallback to the vector3 above
    [SerializeField] private Transform objectToCheck;

    [Header("Debug Intersection")]
    [SerializeField] private bool showIntersection = true;
    [SerializeField] private Color colorIntersecting = Color.green;
    [SerializeField] private Color colorNotIntersecting = Color.red;
    [Tooltip("Needed... Won't work otherwise")]
    [SerializeField] Text intersectionText;
    [Tooltip("Needed... Won't work otherwise")]
    [SerializeField] private OOB_Objects objectToCheckIntersectionAgainst;

    // Update is called once per frame
    void Update()
    {
        if (show)
        {
            if (obbComponent != null)
            {
                // Draw default debug stuff (bounding box)
                DrawDebug();

                // Draw Closest Point
                if (showClosestPoint)
                {
                    DrawDebugClosestPoint();
                }

                // Draw Intersection Info
                if (showIntersection && intersectionText != null && objectToCheckIntersectionAgainst != null)
                {
                    intersectionText.rectTransform.position = transform.position;
                    if (!intersectionText.gameObject.activeSelf)
                    {
                        intersectionText.gameObject.SetActive(true);
                    }
                    if (obbComponent.OrientedBounds.CheckForIntersection(objectToCheckIntersectionAgainst))
                    {
                        intersectionText.color = colorIntersecting;
                        intersectionText.text = "Intersecting";
                    }
                    else
                    {
                        intersectionText.color = colorNotIntersecting;
                        intersectionText.text = "Not Intersecting";
                    }

                }
            }
            else
            {
                obbComponent = GetComponent<OOB_Objects>();
            }

        }
    }

    public void DrawDebug()
    {
        if (obbComponent.OrientedBounds.Corners != null)
        {
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[0], obbComponent.OrientedBounds.Corners[1], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[2], obbComponent.OrientedBounds.Corners[3], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[4], obbComponent.OrientedBounds.Corners[5], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[6], obbComponent.OrientedBounds.Corners[7], color);

            Debug.DrawLine(obbComponent.OrientedBounds.Corners[0], obbComponent.OrientedBounds.Corners[2], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[2], obbComponent.OrientedBounds.Corners[6], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[6], obbComponent.OrientedBounds.Corners[4], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[4], obbComponent.OrientedBounds.Corners[0], color);

            Debug.DrawLine(obbComponent.OrientedBounds.Corners[1], obbComponent.OrientedBounds.Corners[3], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[3], obbComponent.OrientedBounds.Corners[7], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[7], obbComponent.OrientedBounds.Corners[5], color);
            Debug.DrawLine(obbComponent.OrientedBounds.Corners[5], obbComponent.OrientedBounds.Corners[1], color);
        }
    }

    public void DrawDebugClosestPoint()
    {
        if (obbComponent.OrientedBounds.Corners != null)
        {
            Vector3 point = pointToCheck;
            if (objectToCheck != null)
            {
                if (objectToCheck.GetComponent<OOB_Objects>())
                {
                    point = objectToCheck.GetComponent<OOB_Objects>().OrientedBounds.ClosestPoint(transform.position);
                }
                else
                {
                    point = objectToCheck.position;
                }
            }


            // Draw Origin Point then the closest point on bounds
            DebugDrawPoint(point, pointSize, colorClosestPoint);

            Vector3 closestPoint = obbComponent.OrientedBounds.ClosestPoint(point);

            DebugDrawPoint(closestPoint, pointSize, colorClosestPoint);

            Debug.DrawLine(point, closestPoint, colorClosestPoint);

        }
    }




    /// <summary>
    /// Just a simple helper method to draw points as 3-dimensional crosses
    /// </summary>
    /// <param name="point"></param>
    /// <param name="drawSize"></param>
    private void DebugDrawPoint(Vector3 point, float drawSize, Color color)
    {
        // Just simple offsets :)
        Vector3 topFrontLeft = new Vector3(-1f, 1f, -1f);
        Vector3 topFrontRight = new Vector3(1f, 1f, -1f);
        Vector3 topBackLeft = new Vector3(-1f, 1f, 1f);
        Vector3 topBackRight = new Vector3(1f, 1f, 1f);

        Vector3 bottomFrontLeft = new Vector3(-1f, -1f, -1f);
        Vector3 bottomFrontRight = new Vector3(1f, -1f, -1f);
        Vector3 bottomBackLeft = new Vector3(-1f, -1f, 1f);
        Vector3 bottomBackRight = new Vector3(1f, -1f, 1f);

        Debug.DrawLine(point + topFrontLeft * drawSize, point + bottomBackRight * drawSize, color);
        Debug.DrawLine(point + topFrontRight * drawSize, point + bottomBackLeft * drawSize, color);
        Debug.DrawLine(point + topBackLeft * drawSize, point + bottomFrontRight * drawSize, color);
        Debug.DrawLine(point + topBackRight * drawSize, point + bottomFrontLeft * drawSize, color);
    }
}
