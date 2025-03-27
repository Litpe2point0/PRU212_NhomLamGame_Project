using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DarkWizardAttack : MonoBehaviour
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

    [Header("Second Stage")]

    private float meleeCooldownTimer = Mathf.Infinity;
    private float rangeCooldownTimer = Mathf.Infinity;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private Health enemyHealth;
    private int parryCount = 0;
    private bool isDisable = false;
    Rigidbody2D rb;
    private int stage = 1;
    private Transform target;
    private AudioPlayer audioPlayer;

    private DarkWizardMovement movement;
    private void Awake()
    {
        baseDamage = damage;
        anim = GetComponent<Animator>();
        movement = GetComponent<DarkWizardMovement>();
        enemyHealth = GetComponentInParent<Health>();
        rb = GetComponent<Rigidbody2D>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    void Update()
    {
        CheckCurrentPlayer();
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        meleeCooldownTimer += Time.deltaTime;
        rangeCooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            anim.SetBool("isWalking", false);
            if (meleeCooldownTimer >= meleeAttackCooldown)
            {
                if (parryCount >= 2)
                {
                    Attack2();
                    parryCount = 0;
                }
                else if(Random.Range(0, 2) == 0)
                {
                    Attack1();
                }
                else
                {
                    Attack2();
                }
                meleeCooldownTimer = 0;
            }
        }
        if (Vector2.Distance(transform.position, target.position) > 6f && rangeCooldownTimer >= rangeAttackCooldown && stage == 2)
        {
            movement.enabled = false;
            Cast();
            rangeCooldownTimer = 0;
        }
        if (movement != null && !isDisable)
            movement.enabled = !PlayerInSight();
        if(isDisable)
            movement.enabled = false;
        if(stage == 1 && enemyHealth.GetCurrentHealth() <= enemyHealth.GetMaxHealth() / 2)
        {
            stage = 2;
            isDisable = true;
            enemyHealth.SetInvunerbility(true);
            Stage2();
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
    void Attack1()
    {
        anim.SetTrigger("Attack1");
        audioPlayer.PlayMagicSpellClip();
    }
    void Attack2()
    {
        anim.SetTrigger("Attack2");
        audioPlayer.PlayMagicSpellClip();
    }
    void Cast()
    {
        isDisable = true;
        anim.SetTrigger("Cast");

        rb.linearVelocityX = 0;
        anim.SetBool("isCasting", true);

        float distance = Vector2.Distance(transform.position, target.position);
        float width = projectilesPrefab[0].GetComponent<BoxCollider2D>().size.x * projectilesPrefab[0].transform.localScale.x;
        float spacing = 0.1f;

        StartCoroutine(DarkHandCast(target.position, width, distance, spacing));
    }

    private IEnumerator DarkHandCast(Vector3 target, float width, float distance, float spacing)
    {
        float step = width + spacing;
        int projectileCount = Mathf.CeilToInt(distance / step);

        for (int i = 0; i < projectileCount; i++)
        {
            if (i == 0) continue;
            Vector3 targetPosition = new Vector3(transform.position.x + (step * i * Mathf.Sign(target.x - transform.position.x)), target.y + 1.7f, target.z);
            projectilesPrefab[FindPrefab()].transform.position = targetPosition;
            projectilesPrefab[FindPrefab()].GetComponent<DarkHandSpellAttack>().SetActivate();

            yield return new WaitForSeconds(0.2f);
        }
        anim.SetBool("isCasting", false);
        isDisable = false;
    }
    void Stage2()
    {
        StartCoroutine(DelaySummon());
    }
    private IEnumerator DelaySummon()
    {
        yield return new WaitForSeconds(1.5f);

        anim.SetTrigger("Cast");
        anim.SetBool("isCasting", true);

        yield return StartCoroutine(SummonMinion()); // Ensures casting stays true until finished

        isDisable = false;
        anim.SetBool("isCasting", false);
        enemyHealth.SetInvunerbility(false);
    }
    private IEnumerator SummonMinion()
    {
        FindFirstObjectByType<BringerOfDeathSummon>().Summon();
        yield return new WaitForSeconds(2f);
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
    private void DamagePlayer1()
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
                }
            }
            playerHealth.TakeDamage(damage);
            damage = baseDamage;
        }
    }
    private void DamagePlayer2()
    {
        if (PlayerInSight())
        {
            BlockAndParry playerBP = playerHealth.GetComponent<BlockAndParry>();
            if (playerBP != null)
            {
                if (Mathf.Sign(playerBP.GetDirection()) != Mathf.Sign(transform.localScale.x * movement.GetInitialDirection()))
                {
                    if (playerBP.IsBlocking())
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
