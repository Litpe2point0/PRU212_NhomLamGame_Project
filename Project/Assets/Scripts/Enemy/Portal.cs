using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Summon Parameter")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform spawnPoint;
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    private float baseDamage;
    private Animator anim;
    private Health playerHealth;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetActivate()
    {
        gameObject.SetActive(true);
        Activate();
    }
    public void Activate()
    {
        StartCoroutine(FadeInAndAttack());
    }
    private IEnumerator FadeInAndAttack()
    {
        yield return StartCoroutine(Fade(0, 1, fadeDuration)); // Fade in
        enemy.transform.position = spawnPoint.position;
        enemy.SetActive(true);
        yield return new WaitForSeconds(0.5f);
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
}
