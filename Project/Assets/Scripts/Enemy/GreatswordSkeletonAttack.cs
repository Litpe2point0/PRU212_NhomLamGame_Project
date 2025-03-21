using UnityEngine;

public class GreatswordSkeletonAttack : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float range;

    [Header("Collider Parameter")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerMask;

    private float cooldownTimer = Mathf.Infinity;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private Health enemyHealth;
    private int parryCount = 0;

    private GreatswordSkeletonMovement movement;
    private void Awake()
    {
        baseDamage = damage;
        anim = GetComponent<Animator>();
        movement = GetComponent<GreatswordSkeletonMovement>();
        enemyHealth = GetComponentInParent<Health>();
    }
    void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            anim.SetBool("isWalking", false);
            if (cooldownTimer >= attackCooldown)
            {
                if(parryCount >= 2)
                {
                    AttackDelay();
                    parryCount = 0;
                }
                else if (Random.Range(0, 2) == 0)
                {
                    Attack1();
                }
                else
                {
                    Attack2();
                }
                cooldownTimer = 0;
            }
        }
        if (movement != null)
            movement.enabled = !PlayerInSight();
    }
    void Attack1()
    {
        anim.SetTrigger("Attack1");
    }
    void Attack2()
    {
        anim.SetTrigger("Attack2");
    }
    void AttackDelay()
    {
        anim.SetTrigger("Attack3");
    }
    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0f,
            Vector2.right,
            0f,
            playerMask);
        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
        }
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            BlockAndParry playerBP = playerHealth.GetComponent<BlockAndParry>();
            if (playerBP != null)
            {
                if (Mathf.Sign(playerBP.GetDirection()) != Mathf.Sign(transform.localScale.x * movement.GetInitialDirection()))
                {
                    if (playerBP.IsParrying())
                    {
                        parryCount++;
                        enemyHealth.Stun();
                        return;
                    }
                    else if (playerBP.IsBlocking())
                    {
                        damage /= 2;
                    }
                }
            }
            playerHealth.TakeDamage(damage);
            damage = baseDamage;
        }
    }
}
