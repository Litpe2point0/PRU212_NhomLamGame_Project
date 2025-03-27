using UnityEngine;

public class HeavyBanditMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private PlayerSwitch playerSwitch;
    [SerializeField] private int initialDirection;
    private Animator anim;
    private Rigidbody2D rb;
    private Transform target;
    private bool isDead = false;
    private Health health;
    public bool ignore = false;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Grounded", true);
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        SetProjectileCollisions();
        if (isDead) return;
        CheckCurrentPlayer();
        FlipSprite();
        Move();
        CheckHealth();
    }
    void SetProjectileCollisions()
    {
        GameObject holder = GameObject.FindGameObjectWithTag("PlayerProjectile");

        if (holder != null)
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();

            foreach (Transform child in holder.transform)
            {
                if (child.gameObject.activeInHierarchy) // Only active projectiles
                {
                    Collider2D projectileCollider = child.GetComponent<Collider2D>();
                    if (projectileCollider != null)
                    {
                        Physics2D.IgnoreCollision(enemyCollider, projectileCollider, ignore);
                    }
                }
            }
        }
    }
    public void SetCollision(bool value)
    {
        if (value)
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();
            Collider2D player1Collider = playerSwitch.GetPlayer1().GetComponent<Collider2D>();
            Collider2D player2Collider = playerSwitch.GetPlayer2().GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(enemyCollider, player1Collider, false);
            Physics2D.IgnoreCollision(enemyCollider, player2Collider, false);
            ignore = false;
        }
        else
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();
            Collider2D player1Collider = playerSwitch.GetPlayer1().GetComponent<Collider2D>();
            Collider2D player2Collider = playerSwitch.GetPlayer2().GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(enemyCollider, player1Collider, true);
            Physics2D.IgnoreCollision(enemyCollider, player2Collider, true);
            ignore = true;
        }
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
        if (Vector2.Distance(transform.position, target.position) > 0.2f)
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
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * initialDirection, transform.localScale.y, transform.localScale.z);
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x) * initialDirection, transform.localScale.y, transform.localScale.z);
        }
    }
    public int GetInitialDirection()
    {
        return initialDirection;
    }
    void CheckHealth()
    {
        if (health.GetCurrentHealth() <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("Death");
        anim.SetBool("isDeath", true);
        GetComponent<Rigidbody2D>().gravityScale = 1;
        SetCollision(false);
    }
}
