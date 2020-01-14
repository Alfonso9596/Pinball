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
        foreach (GameObject wallObject in walls)
        {
            Wall wall = wallObject.GetComponent<Wall>();

            //Vector3 normal = (ball.transform.position - wall.transform.position);
            Vector3 normal = new Vector3(ball.transform.position.x - wall.transform.position.x, 0, ball.transform.position.z - wall.transform.position.z);

            bool isBallCollidingWithWall = BallToWallCollision(wall);
            if (isBallCollidingWithWall)
            {
                Debug.Log(wall.name);
                ball.BounceOff(normal, ball.Bounciness * wall.Bounciness);
            }
        }

        BallToFloorCollision();
        foreach (GameObject flipperObject in flippers)
        {
            Flipper flipper = flipperObject.GetComponent<Flipper>();

            Vector3 normal = new Vector3(ball.transform.position.x - flipper.transform.position.x, 0, ball.transform.position.z - flipper.transform.position.z);

            bool ballToFlipperCollision = BallToFlipperCollision(flipper);
            if (ballToFlipperCollision)
            {
                Debug.Log(flipper.name);
                ball.BounceOff(normal, ball.Bounciness * flipper.Bounciness);
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
        MeshRenderer flipperMesh = flipper.GetComponent<MeshRenderer>();

        float wallWidth = flipperMesh.bounds.size.x;
        float wallHeight = flipperMesh.bounds.size.y;
        float wallDepth = flipperMesh.bounds.size.z;

        float ballXDistance = Mathf.Abs(ball.transform.position.x - flipper.transform.position.x);
        float ballYDistance = Mathf.Abs(ball.transform.position.y - flipper.transform.position.y);
        float ballZDistance = Mathf.Abs(ball.transform.position.z - flipper.transform.position.z);

        float ballRadius = ball.transform.localScale.x / 2;

        if
            (ballXDistance >= (wallWidth - ballRadius) ||
             ballYDistance >= (wallHeight - ballRadius) ||
             ballZDistance >= (wallDepth - ballRadius))
        {
            return false;
        }
        else if
            (ballXDistance < wallWidth ||
             ballYDistance < wallHeight ||
             ballZDistance < wallDepth)
        {
            return true;
        }

        float cornerDistance =
            Mathf.Pow(ballXDistance - wallWidth, 2) *
            Mathf.Pow(ballYDistance - wallHeight, 2) *
            Mathf.Pow(ballZDistance - wallDepth, 2);

        return (cornerDistance < Mathf.Pow(ballRadius, 2));
    }

    private bool BallToWallCollision(Wall wall)
    {
        MeshRenderer wallMesh = wall.GetComponent<MeshRenderer>();

        float wallWidth = wallMesh.bounds.size.x / 2;
        float wallHeight = wallMesh.bounds.size.y / 2;
        float wallDepth = wallMesh.bounds.size.z / 2;

        float ballXDistance = Mathf.Abs(ball.transform.position.x - wall.transform.position.x);
        float ballYDistance = Mathf.Abs(ball.transform.position.y - wall.transform.position.y);
        float ballZDistance = Mathf.Abs(ball.transform.position.z - wall.transform.position.z);

        float ballRadius = ball.transform.localScale.x / 2;

        if 
            (ballXDistance >= (wallWidth + ballRadius) ||
             ballYDistance >= (wallHeight + ballRadius) ||
             ballZDistance >= (wallDepth + ballRadius))
        {
            return false;
        }
        else if
            (ballXDistance < wallWidth + ballRadius ||
             ballYDistance < wallHeight + ballRadius ||
             ballZDistance < wallDepth + ballRadius)
        {
            return true;
        }

        float cornerDistance = 
            Mathf.Pow(ballXDistance - wallWidth + ballRadius, 2) * 
            Mathf.Pow(ballYDistance - wallHeight + ballRadius, 2) * 
            Mathf.Pow(ballZDistance - wallDepth + ballRadius, 2);

        return (cornerDistance < Mathf.Pow(ballRadius, 2));
    }
}
