using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private GameObject[] walls;
    private GameObject[] flippers;
    private Ball ball;
    private Floor floor;

    void Start()
    {
        walls = GameObject.FindGameObjectsWithTag("Wall");
        flippers = GameObject.FindGameObjectsWithTag("Flipper");
        floor = GameObject.FindGameObjectWithTag("Floor").GetComponent<Floor>();
    }

    void FixedUpdate()
    {
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();

        BallToFloorCollision();

        foreach (GameObject wallObject in walls)
        {
            Wall wall = wallObject.GetComponent<Wall>();
            Debug.Log(wall.name);
            bool isBallCollidingWithWall = BallToWallCollision(wall);
            if (isBallCollidingWithWall)
            {
                Vector3 normal = wall.GetNormal();
                if (Vector3.Dot(normal, ball.Movement) < 0)
                {
                    //Debug.Log(wall.name);
                    ball.BounceOff(normal, ball.Bounciness * wall.Bounciness);
                }
            }
        }

        foreach (GameObject flipperObject in flippers)
        {
            Flipper flipper = flipperObject.GetComponent<Flipper>();
            Debug.Log(flipper.name);

            bool ballToFlipperCollision = BallToFlipperCollision(flipper);
            if (ballToFlipperCollision)
            {
                Vector3 normal = flipper.GetNormal();
                if (Vector3.Dot(normal, ball.Movement) < 0)
                {
                    //Debug.Log(flipper.name);
                    ball.BounceOff(normal, ball.Bounciness * flipper.Bounciness);
                }
            }
        }
    }

    private void BallToFloorCollision()
    {
        Vector3 localx = new Vector3(floor.transform.localScale.x / 2, 0, 0);
        Vector3 localz = new Vector3(0, 0, floor.transform.localScale.z / 2);
        Vector3 cross = Vector3.Cross(localz, localx);
        float r = ball.transform.localScale.x / 2;

        float D = -cross.x * floor.transform.position.x - cross.y * floor.transform.position.y - cross.z * floor.transform.position.z;
        float y0 = (-cross.x * ball.transform.position.x - cross.z * ball.transform.position.z - D) / cross.y;
        float d = Mathf.Abs(cross.x * (ball.transform.position.x - ball.transform.position.x) + cross.y * (ball.transform.position.y - y0) + cross.z * (ball.transform.position.z - ball.transform.position.z)) / Mathf.Sqrt(cross.x * cross.x + cross.y * cross.y + cross.z * cross.z);
        if (d <= floor.transform.position.y + r)
        {
            ball.transform.position = new Vector3(ball.transform.position.x, r, ball.transform.position.z);
            ball.Gravity = new Vector3(ball.Gravity.x, 0, ball.Gravity.z);
        }
    }

    private bool BallToFlipperCollision(Flipper flipper)
    {
        Vector3 closestPoint = flipper.OrientedBounds.ClosestPoint(ball.Center);
        float distance = (ball.Center - closestPoint).magnitude;

        return distance <= ball.Radius + 0.2f;
    }

    private bool BallToWallCollision(Wall wall)
    {
        Vector3 closestPoint = wall.OrientedBounds.ClosestPoint(ball.Center);
        float distance = (ball.Center - closestPoint).magnitude;

        return distance <= ball.Radius + 0.2f;
    }
}
