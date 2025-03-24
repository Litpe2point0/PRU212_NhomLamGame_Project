using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("GameOver")]
    public CanvasGroup gameOverPanel;

    [Header("Win")]
    public CanvasGroup winPanel;
    private bool isMenuVisible = false;
    private bool isGameOverVisible = false;
    private bool isWinVisible = false;
    public float fadeDuration = 1f;
    void Update()
    {

    }
    public void ShowGameOver()
    {
        isGameOverVisible = true;
        StopAllCoroutines();
        StartCoroutine(FadeMenu(isGameOverVisible, gameOverPanel));
    }

    public void ShowWin()
    {
        isWinVisible = true;
        StopAllCoroutines();
        StartCoroutine(FadeMenu(isWinVisible, winPanel));
    }

    IEnumerator FadeMenu(bool show, CanvasGroup menu)
    {
        float startAlpha = menu.alpha;
        float endAlpha = show ? 1 : 0;
        float elapsedTime = 0f;

        menu.interactable = show;
        menu.blocksRaycasts = show;

        while (elapsedTime < fadeDuration)
        {
            menu.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        menu.alpha = endAlpha;
    }
}
