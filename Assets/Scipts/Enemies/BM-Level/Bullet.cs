using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;
    EnemyController enemyController;

    bool isFacingRight;

    bool isFollowingPath;
    Vector3 pathStartPoint;
    Vector3 pathEndPoint;
    Vector3 pathMidPoint;
    float pathTimeStart;

    public float bezierTime = 1f;
    public float bezierDistance = 1f;
    public Vector3 bezierHeight = new Vector3(0, 0.8f, 0);

    [SerializeField] RuntimeAnimatorController racBulletOrange;
    public enum MoveDirections { Left, Right };
    [SerializeField] MoveDirections moveDirection = MoveDirections.Left;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        animator = enemyController.GetComponent<Animator>();
        rb2d = enemyController.GetComponent<Rigidbody2D>();
        isFacingRight = false;
        if (moveDirection == MoveDirections.Right)
        {
            isFacingRight = false;
            enemyController.Flip();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.freezeEnemy)
        {
            pathTimeStart += Time.deltaTime;
            return;
        }
        if (!isFollowingPath)
        {
            float distance = (isFacingRight) ? bezierDistance : -bezierDistance;
            pathStartPoint = rb2d.transform.position;
            pathEndPoint = new Vector3(pathStartPoint.x + distance, pathStartPoint.y, pathStartPoint.z);
            pathMidPoint = pathStartPoint + (((pathEndPoint - pathStartPoint) / 2) + bezierHeight);
            pathTimeStart = Time.time;
            isFollowingPath = true;
        }
        else
        {
            float percentage = (Time.time - pathTimeStart) / bezierTime;
            rb2d.transform.position = Functions.CalculateQuadraticBezierPoint(pathStartPoint, pathMidPoint, pathEndPoint, percentage);
            if (percentage >= 1f)
            {
                bezierHeight *= -1;
                isFollowingPath = false;
            }
        }
    }
    //Flip based on what direction the sprite is moving
    public void SetMoveDirection(MoveDirections direction)
    {
        moveDirection = direction;
        if (moveDirection == MoveDirections.Left)
        {
            if (isFacingRight)
            {
                isFacingRight = !isFacingRight;
                enemyController.Flip();
            }
        }
        else
        {
            if (!isFacingRight)
            {
                isFacingRight = !isFacingRight;
                enemyController.Flip();
            }
        }
    }
    //Reset enemy pathing
    public void ResetFollowingPath()
    {
        isFollowingPath = false;
    }
}
