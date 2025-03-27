using System.Collections;
using UnityEngine;

public class FireWizardAttack : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float attackDuration;
    [SerializeField] private float damage;
    [SerializeField] private float range;

    [Header("Collider Parameter")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private PlayerSwitch playerSwitch;
    [SerializeField] private LayerMask playerMask;

    public float cooldownTimer = Mathf.Infinity;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private AudioPlayer audioPlayer;
    private Transform target;
    private void Awake()
    {
        baseDamage = damage;
        anim = GetComponent<Animator>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    private void Update()
    {
        CheckCurrentPlayer();
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
                if (Mathf.Sign(playerBP.GetDirection()) != Mathf.Sign(transform.localScale.x * gameObject.transform.localScale.x))
                {
                    if (playerBP.IsBlocking())
                    {
                        damage /= 2/3;
                    }
                }
            }
            playerHealth.SetInvunerbility(false);
            playerHealth.TakeDamage(damage);
            damage = baseDamage;
        }
    }
    public void FlameThrowerAttack()
    {
        anim.SetBool("isAttacking", true);
        audioPlayer.PlayFireBreathingClip();
        anim.SetBool("isWalking", false);
        StartCoroutine(FlameThrower());
    }
    IEnumerator FlameThrower()
    {
        // Start attack animation
        anim.SetBool("isAttacking", true);
        cooldownTimer = 0f;
        // Keep attacking while the animation is playing
        while (anim.GetBool("isAttacking"))
        {
            cooldownTimer += Time.deltaTime;
            // Stop attack after cooldown
            if (cooldownTimer >= attackDuration)
            {
                GetComponent<FireWizardMovement>().StartReturning();
                anim.SetBool("isAttacking", false);
                break;
            }

            yield return null; // Wait for the next frame
        }
    }
}
