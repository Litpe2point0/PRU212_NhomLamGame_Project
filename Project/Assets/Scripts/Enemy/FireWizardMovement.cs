using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FireWizardMovement : MonoBehaviour
{
    [Header("Height Parameter")]
    [SerializeField] private Transform air;
    [SerializeField] private Transform ground;
    [SerializeField] private float speed;
    [SerializeField] private Transform minX;
    [SerializeField] private Transform maxX;

    [Header("Target")]
    [SerializeField] private PlayerSwitch playerSwitch;

    [Header("Dive Attack Parameter")]
    [SerializeField] private float diveSpeed;

    private Vector2 previousPosition;
    private bool isAttacking = false;
    public float attackTimer;
    private float startY;
    private float floatSpeed;
    private float floatHeight;
    private FireWizardAttack attack;
    private Animator anim;
    private Transform player;
    private bool isDead = false;
    private Health health;
    private bool ignore = false;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        attack = GetComponent<FireWizardAttack>();
        health = GetComponent<Health>();
    }
    private void Start()
    {
        startY = air.position.y; // Save initial Y position
        floatSpeed = Random.Range(1.5f, 3f); // Randomize floating speed
        floatHeight = Random.Range(0.3f, 0.8f);
    }
    void Update()
    {
        SetProjectileCollisions();
        if (isDead) return;

        CheckCurrentPlayer();
        FlipSprite();
        if(!isAttacking)
        {
            attackTimer += Time.deltaTime;
        }
        if (!isAttacking)
        {
            Bobbling();
            Hover();
        }
        if (attackTimer >= 5f)
        {
            previousPosition = transform.position;
            isAttacking = true;
            attackTimer = 0;
            DiveAttack();
        }
        MoveForward();

        CheckHealth();
    }
    void CheckCurrentPlayer()
    {
        if(playerSwitch.GetPlayer1Active())
        {
            player = playerSwitch.GetPlayer1().transform;
        }
        else
        {
            player = playerSwitch.GetPlayer2().transform;
        }
    }
    void DiveAttack()
    {
        StartCoroutine(DiveAttackRoutine());
    }
    IEnumerator DiveAttackRoutine()
    {
        Vector2 diveTarget = new Vector2(player.position.x - transform.localScale.x, ground.position.y + 1);

        // Move towards the dive target
        while (Vector2.Distance(transform.position, diveTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, diveTarget, diveSpeed * Time.deltaTime);
            yield return null; // Wait until next frame
        }

        // Attack after reaching the target
        attack.FlameThrowerAttack();
    }
    public void StartReturning()
    {
        StartCoroutine(ReturnToHoverPosition());
    }
    IEnumerator ReturnToHoverPosition()
    {
        Vector2 airPosition = new Vector2(transform.position.x, air.position.y);

        while (Vector2.Distance(transform.position, previousPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, previousPosition, diveSpeed * Time.deltaTime);
            yield return null;
        }

        isAttacking = false; // Resume normal behavior
    }

    void Hover()
    {
        anim.SetBool("isWalking", true);
        transform.position = new Vector3(
        Mathf.PingPong(Time.time * speed, maxX.position.x - minX.position.x) + minX.position.x,
        transform.position.y,
        transform.position.z
        );
    }
    void MoveForward()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Check if the attack animation is playing
        if (stateInfo.IsName("Attack"))
        {
            float moveSpeed = 1f; // Adjust speed
            transform.position += new Vector3(transform.localScale.x * moveSpeed * Time.deltaTime, 0, 0);
        }
    }
    void Bobbling()
    {
        // Calculate the new Y position using a sine wave
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Apply the floating effect
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    void FlipSprite()
    {
        if (transform.position.x < player.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    public void SetIsAttacking(bool value)
    {
        isAttacking = value;
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
        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D player1Collider = playerSwitch.GetPlayer1().GetComponent<Collider2D>();
        Collider2D player2Collider = playerSwitch.GetPlayer2().GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(enemyCollider, player1Collider, true);
        Physics2D.IgnoreCollision(enemyCollider, player2Collider, true);
        ignore = true;
        StopAllCoroutines();
        enabled = false;
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
}
