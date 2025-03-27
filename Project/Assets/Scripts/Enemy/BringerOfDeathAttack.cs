using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.SocialPlatforms;

public class BringerOfDeathAttack : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float meleeAttackCooldown;
    [SerializeField] private float rangeAttackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float height;

    [Header("Collider Parameter")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Spell Parameter")]
    [SerializeField] private PlayerSwitch playerSwitch;
    [SerializeField] private GameObject[] projectilesPrefab;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private GameObject boss;
    private float meleeCooldownTimer = Mathf.Infinity;
    private float rangeCooldownTimer = Mathf.Infinity;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private Health enemyHealth;
    private Transform target;

    private BringerOfDeathMovement movement;
    private AudioPlayer audioPlayer;
    private void Awake()
    {
        baseDamage = damage;
        anim = GetComponent<Animator>();
        movement = GetComponent<BringerOfDeathMovement>();
        enemyHealth = GetComponentInParent<Health>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    void Update()
    {
        CheckCurrentPlayer();
        meleeCooldownTimer += Time.deltaTime;
        rangeCooldownTimer += Time.deltaTime;
        if (PlayerInSight())
        {
            anim.SetBool("isWalking", false);
            if (meleeCooldownTimer >= meleeAttackCooldown)
            {
                Attack();
                meleeCooldownTimer = 0;
            }
        }
        if (Vector2.Distance(transform.position, target.position) > 10f && rangeCooldownTimer >= rangeAttackCooldown)
        {
            movement.enabled = false;
            Cast();
            rangeCooldownTimer = 0;
        }
        if (movement != null)
            movement.enabled = !PlayerInSight();
        if (boss.GetComponent<Health>().IsDead())
        {
            enemyHealth.TakeDamage(9999);
            Collider2D enemyCollider = GetComponentInParent<Collider2D>();
            Collider2D player1Collider = playerSwitch.GetPlayer1().GetComponent<Collider2D>();
            Collider2D player2Collider = playerSwitch.GetPlayer2().GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(enemyCollider, player1Collider, true);
            Physics2D.IgnoreCollision(enemyCollider, player2Collider, true);
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            int projectileLayer = LayerMask.NameToLayer("PlayerProjectile");
            Physics2D.IgnoreLayerCollision(enemyLayer, projectileLayer, true);
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
    void Attack()
    {
        anim.SetTrigger("Attack");
        audioPlayer.PlaySlashCLip();
    }
    void Cast()
    {
        anim.SetTrigger("Cast");
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + (float)1.7, target.position.z);
        projectilesPrefab[FindPrefab()].transform.position = targetPosition;
        projectilesPrefab[FindPrefab()].GetComponent<DarkHandSpellAttack>().SetActivate();
    }
    private int FindPrefab()
    {
        for (int i = 0; i < projectilesPrefab.Length; i++)
        {
            if (!projectilesPrefab[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance + Vector3.up * ((boxCollider.bounds.size.y * (height - 1)) / 2),
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y * height, boxCollider.bounds.size.z),
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
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance + Vector3.up * ((boxCollider.bounds.size.y * (height - 1)) / 2),
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y * height, boxCollider.bounds.size.z));
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
