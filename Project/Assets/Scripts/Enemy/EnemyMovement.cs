using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private Rigidbody2D rb;
    private Animator ani;

    public bool isFacingRight = true;
    public Transform pointA;
    public Transform pointB;
    public float distance = 1f;
    private Transform currentPoint;

    Transform Player;
    private Vector2 pointAStartPos;
    private Vector2 pointBStartPos;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        pointAStartPos = pointA.position;
        pointBStartPos = pointB.position;
        currentPoint = pointB.transform;
    }

    private void Update()
    {
        pointA.position = pointAStartPos;
        pointB.position = pointBStartPos;
        Move();
        CheckPosition();
    }

    private void Move()
    {
        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        ani.SetBool("isWalking", true);
    }

    private void CheckPosition()
    {
        if (currentPoint == pointB.transform)
        {
            isFacingRight = true;
        }
        else
        {
            isFacingRight = false;
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            
            if (currentPoint == pointB.transform)
            {
                Flip();
                currentPoint = pointA.transform;
            }
            else
            {
                Flip();
                currentPoint = pointB.transform;
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
