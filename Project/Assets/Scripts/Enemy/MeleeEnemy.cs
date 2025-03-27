using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private string attackAnimation;

    [Header("Collider Parameter")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private PlayerSwitch playerSwitch;
    [SerializeField] private LayerMask playerMask;

    private float cooldownTimer = Mathf.Infinity;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private Health enemyHealth;
    private AudioPlayer audioPlayer;
    private Transform target;
    private EnemyPatrol enemyPatrol;
    private bool ignore = false;
    private void Awake()
    {
        baseDamage = damage;
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        enemyHealth = GetComponent<Health>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    void Update()
    {
        SetProjectileCollisions();
        if (enemyHealth.IsDead()) return;
        cooldownTimer += Time.deltaTime;
        CheckCurrentPlayer();
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                anim.SetTrigger(attackAnimation);
                audioPlayer.PlaySlashCLip();
                cooldownTimer = 0;
            }
        }
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
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
    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0f,
            Vector2.right,
            0f,
            playerMask);
        if (hit.collider != null && hit.transform == target)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            return true;
        }
        return false;
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
                if (Mathf.Sign(playerBP.GetDirection()) != Mathf.Sign(transform.localScale.x * enemyPatrol.GetInitialDirection()))
                {
                    if (playerBP.IsParrying())
                    {
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
            audioPlayer.PlayHitCLip();
            damage = baseDamage;
        }
    }
}
