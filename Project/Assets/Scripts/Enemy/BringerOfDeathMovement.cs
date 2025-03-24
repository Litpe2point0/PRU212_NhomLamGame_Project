using UnityEngine;

public class BringerOfDeathMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private PlayerSwitch playerSwitch;
    [SerializeField] private int initialDirection;
    private Animator anim;
    private Rigidbody2D rb;
    private Transform target;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckCurrentPlayer();
        FlipSprite();
        Move();
    }
    void CheckCurrentPlayer()
    {
        if (playerSwitch.GetPlayer1Active())
        {
            target = playerSwitch.GetPlayer1().transform;
        }
        else
        {
            target = playerSwitch.GetPlayer2().transform;
        }
    }
    private void Move()
    {
        if (target == null) return;

        anim.SetBool("isWalking", true);
        float direction = Mathf.Sign(target.position.x - transform.position.x); // Left (-1) or Right (1)

        // Move only if not already at the target
        if (Vector2.Distance(transform.position, target.position) > 0.3f)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y); // Move in X direction, keep Y velocity
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving when close
        }
    }
    void FlipSprite()
    {
        if (Mathf.Sign(target.position.x - transform.position.x) > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * initialDirection, transform.localScale.y, transform.localScale.z);
        }
        else if (Mathf.Sign(target.position.x - transform.position.x) < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x) * initialDirection, transform.localScale.y, transform.localScale.z);
        }
    }
    public int GetInitialDirection()
    {
        return initialDirection;
    }
}
