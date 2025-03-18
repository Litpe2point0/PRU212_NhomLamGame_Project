using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float health = 3;
    public float currentHealth;
    private PlayerKnight script;
    private bool isDead = false;

    [Header("IFrame")]
    [SerializeField] private float iFrameDuration = 0.5f;
    [SerializeField] private float numberOfFlashes = 5;

    [Header("Stun Parameter")]
    [SerializeField] private float flashSpeed;
    [SerializeField] private float stunDuration = 1.5f;
    private bool isStunned = false;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, health);
        if(currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
            StartCoroutine(InvunerabilityFrames());
        }
        else
        {
            if(!isDead)
            {
                anim.SetTrigger("Death");
                Physics2D.IgnoreLayerCollision(7, 10, true);
                //Player
                if (GetComponent<PlayerKnight>() != null)
                {
                    GetComponent<PlayerKnight>().enabled = false;
                }
                //Enemy
                if(GetComponent<MeleeEnemy>() != null)
                {
                    GetComponent<MeleeEnemy>().enabled = false;
                }
                if(GetComponent<EnemyPatrol>() != null)
                {
                    GetComponentInParent<EnemyPatrol>().enabled = false;
                }
                isDead = true;
            }
        }
    }
    private IEnumerator InvunerabilityFrames()
    {
        Physics2D.IgnoreLayerCollision(7, 10, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(7, 10, false);
    }
    public void Stun()
    {
        if (!isStunned)
        {
            isStunned = true;
            //Player
            if (GetComponent<PlayerKnight>() != null)
            {
                GetComponent<PlayerKnight>().enabled = false;
            }
            //Enemy
            if (GetComponent<MeleeEnemy>() != null)
            {
                GetComponent<MeleeEnemy>().enabled = false;
            }
            if (GetComponent<EnemyPatrol>() != null)
            {
                GetComponent<EnemyPatrol>().enabled = false;
            }
            StartStunEffect(stunDuration, flashSpeed);
            StartCoroutine(RecoverFromStun());
        }
    }

    private IEnumerator RecoverFromStun()
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        if (!isDead)
        {
            if (GetComponent<PlayerKnight>() != null)
            {
                GetComponent<PlayerKnight>().enabled = true;
            }
            //Enemy
            if (GetComponent<MeleeEnemy>() != null)
            {
                GetComponent<MeleeEnemy>().enabled = true;
            }
            if (GetComponent<EnemyPatrol>() != null)
            {
                GetComponent<EnemyPatrol>().enabled = true;
            }
        }
    }
    public void StartStunEffect(float duration, float flashSpeed)
    {
        StartCoroutine(StunFlashEffect(duration, flashSpeed));
    }

    private IEnumerator StunFlashEffect(float duration, float flashSpeed)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;
        bool isBlue = true;

        while (elapsedTime < duration)
        {
            spriteRenderer.color = isBlue ? Color.blue : Color.white;
            isBlue = !isBlue;
            yield return new WaitForSeconds(flashSpeed);
            elapsedTime += flashSpeed;
        }

        spriteRenderer.color = Color.white; // Reset color after stun
    }
    public bool IsDead()
    {
        return isDead;
    }
}
