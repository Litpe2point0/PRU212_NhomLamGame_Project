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

    private bool isInvunerable = false;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private AudioPlayer audioPlayer;
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamage(float damage)
    {
        if(isInvunerable)
        {
            return;
        }
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, health);
        if (GetComponentInParent<GreatswordSkeletonSwitch>() != null)
        {
            GetComponentInParent<GreatswordSkeletonSwitch>().TakeDamage(damage);
        }
        if (currentHealth > 0)
        {
            if (GetComponent<PlayerKnight>() != null)
            {
                anim.SetTrigger("Hurt");
                StartCoroutine(InvunerabilityFrames(iFrameDuration, numberOfFlashes));
            }
            else
            {
                StartCoroutine(FlashSprite(iFrameDuration, numberOfFlashes));
            }
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
                if(GetComponent<PlayerWizard>() != null)
                {
                    GetComponent<PlayerWizard>().enabled = false;
                }
                //Enemy
                if(GetComponent<FireWizardAttack>() != null)
                {
                    GetComponent<FireWizardAttack>().enabled = false;
                }
                if (GetComponent<MeleeEnemy>() != null)
                {
                    GetComponent<MeleeEnemy>().enabled = false;
                }
                if(GetComponent<EnemyPatrol>() != null)
                {
                    GetComponentInParent<EnemyPatrol>().enabled = false;
                }
                if (GetComponent<GreatswordSkeletonAttack>() != null)
                {
                    GetComponent<GreatswordSkeletonAttack>().enabled = false;
                }
                if (GetComponent<GreatswordSkeletonMovement>() != null)
                {
                    GetComponent<GreatswordSkeletonMovement>().enabled = false;
                }
                if(GetComponent<BringerOfDeathAttack>() != null)
                {
                    GetComponent<BringerOfDeathAttack>().enabled = false;
                }
                if (GetComponent<BringerOfDeathMovement>() != null)
                {
                    GetComponent<BringerOfDeathMovement>().enabled = false;
                }
                if(GetComponent<DarkWizardAttack>() != null)
                {
                    GetComponent<DarkWizardAttack>().enabled = false;
                }
                if (GetComponent<DarkWizardMovement>() != null)
                {
                    GetComponent<DarkWizardMovement>().enabled = false;
                }
                isDead = true;
            }
        }
    }
    private IEnumerator InvunerabilityFrames(float iFrameDuration, float numberOfFlashes)
    {
        isInvunerable = true;
        Physics2D.IgnoreLayerCollision(7, 10, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));
        }
        isInvunerable = false;
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
            if (GetComponent<GreatswordSkeletonAttack>() != null)
            {
                GetComponent<GreatswordSkeletonAttack>().enabled = false;
            }
            if (GetComponent<GreatswordSkeletonMovement>() != null)
            {
                GetComponent<GreatswordSkeletonMovement>().enabled = false;
            }
            if (GetComponent<BringerOfDeathAttack>() != null)
            {
                GetComponent<BringerOfDeathAttack>().enabled = false;
            }
            if (GetComponent<BringerOfDeathMovement>() != null)
            {
                GetComponent<BringerOfDeathMovement>().enabled = false;
            }
            if (GetComponent<DarkWizardAttack>() != null)
            {
                GetComponent<DarkWizardAttack>().enabled = false;
            }
            if (GetComponent<DarkWizardMovement>() != null)
            {
                GetComponent<DarkWizardMovement>().enabled = false;
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
            if (GetComponent<GreatswordSkeletonAttack>() != null)
            {
                GetComponent<GreatswordSkeletonAttack>().enabled = true;
            }
            if (GetComponent<GreatswordSkeletonMovement>() != null)
            {
                GetComponent<GreatswordSkeletonMovement>().enabled = true;
            }
            if (GetComponent<BringerOfDeathAttack>() != null)
            {
                GetComponent<BringerOfDeathAttack>().enabled = true;
            }
            if (GetComponent<BringerOfDeathMovement>() != null)
            {
                GetComponent<BringerOfDeathMovement>().enabled = true;
            }
            if (GetComponent<DarkWizardAttack>() != null)
            {
                GetComponent<DarkWizardAttack>().enabled = true;
            }
            if (GetComponent<DarkWizardMovement>() != null)
            {
                GetComponent<DarkWizardMovement>().enabled = true;
            }
        }
    }
    public void StartStunEffect(float duration, float flashSpeed)
    {
        StartCoroutine(StunFlashEffect(duration, flashSpeed));
    }
    private IEnumerator FlashSprite(float duration, float flashes)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        for (int i = 0; i < flashes; i++)
        {
            sprite.color = new Color(1, 1, 1, 0.3f); // Transparent effect
            yield return new WaitForSeconds(duration / (flashes * 2));
            sprite.color = Color.white; // Normal color
            yield return new WaitForSeconds(duration / (flashes * 2));
        }
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
    public void SetMaxHealth(float newHealth)
    {
        health = newHealth;
        currentHealth = health;
    }
    public void SetCurrentHealth(float health)
    {
        currentHealth = health;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return health;
    }
    public void SetInvunerbility(bool value)
    {
        isInvunerable = value;
    }
}
