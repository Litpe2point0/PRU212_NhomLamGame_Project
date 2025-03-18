using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;

    private bool isGrounded;
    private Sensor_PlayerKnight m_groundSensor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_PlayerKnight>();
    }

    private void Update()
    {
        CheckGrounded();
        Move();
    }

    private void CheckGrounded()
    {
        // Cast a ray downwards to check if NPC is on the ground
        if (!isGrounded && m_groundSensor.State())
        {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
        }

        //Check if character just started falling
        if (isGrounded && !m_groundSensor.State())
        {
            isGrounded = false;
            anim.SetBool("Grounded", isGrounded);
        }
    }

    private void Move()
    {
        if (player == null) return;

        float followDistance = 1.5f; // Distance NPC should maintain behind the player
        float direction = Mathf.Sign(player.position.x - transform.position.x); // Determine direction (left or right)

        // Calculate follow point behind the player based on facing direction
        Vector2 followPoint = new Vector2(
            player.position.x - (player.localScale.x * followDistance),
            transform.position.y
        );

        float distanceToFollowPoint = Mathf.Abs(followPoint.x - transform.position.x);

        // Move NPC if it's farther than the stop threshold
        if (distanceToFollowPoint > 0.1f && isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            anim.SetBool("isWalking", true);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving
            anim.SetBool("isWalking", false);
        }

        // Flip NPC to always face the movement direction
        if (direction > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If NPC collides with an obstacle, jump
        if (other.CompareTag("Obstacle") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }
    }

    private void OnDrawGizmos()
    {
        // Debugging: Draw ground check ray
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
    }
}
