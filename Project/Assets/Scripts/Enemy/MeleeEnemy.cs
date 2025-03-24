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
    [SerializeField] private LayerMask playerMask;

    private float cooldownTimer = Mathf.Infinity;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private Health enemyHealth;
    private AudioPlayer audioPlayer;

    private EnemyPatrol enemyPatrol;
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
        cooldownTimer += Time.deltaTime;

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
