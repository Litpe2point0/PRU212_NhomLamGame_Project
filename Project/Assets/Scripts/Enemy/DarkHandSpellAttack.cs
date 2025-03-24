using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkHandSpellAttack : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float height;

    [Header("Collider Parameter")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerMask;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private SpriteRenderer spriteRenderer;

    private AudioPlayer audioPlayer;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();

    }
    public void SetActivate()
    {
        gameObject.SetActive(true);
        Activate();
    }
    public void Activate()
    {
        audioPlayer.PlayDarkHandSpellClip();
        StartCoroutine(FadeInAndAttack());
    }
    private IEnumerator FadeInAndAttack()
    {
        yield return StartCoroutine(Fade(0, 1, fadeDuration)); // Fade in
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f); // Wait for attack animation
        yield return StartCoroutine(Fade(1, 0, fadeDuration)); // Fade out
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            spriteRenderer.color = color;
            yield return null;
        }
    }
    private bool PlayerInSight()
    {
        Vector2 castDirection = transform.up * Mathf.Sign(height); // Moves down if height is negative
        Vector2 castOrigin = (Vector2)boxCollider.bounds.center + castDirection * Mathf.Abs(height) * transform.localScale.y * 0.5f;

        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin,
            new Vector2(boxCollider.bounds.size.x * range, Mathf.Abs(height) * transform.localScale.y),
            0f,
            castDirection, // Moves in the direction of height
            Mathf.Abs(height),
            playerMask);
        if (hit.collider != null)
        {
            Debug.Log("Player in sight");
            playerHealth = hit.transform.GetComponent<Health>();
        }
        else
        {
            Debug.Log("Player not in sight");
        }
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        Vector2 castDirection = transform.up * Mathf.Sign(height); // Moves down if height is negative
        Vector2 castOrigin = (Vector2)boxCollider.bounds.center + castDirection * Mathf.Abs(height) * transform.localScale.y * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            castOrigin,
            new Vector2(boxCollider.bounds.size.x * range, Mathf.Abs(height) * transform.localScale.y));
    }
    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            BlockAndParry playerBP = playerHealth.GetComponent<BlockAndParry>();
            if (playerBP != null)
            {
                if (playerBP.IsParrying())
                {
                    return;
                }
                else if (playerBP.IsBlocking())
                {
                    damage /= 2;
                }
            }
            playerHealth.TakeDamage(damage);
            damage = baseDamage;
        }
    }
}
