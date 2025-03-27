using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("GameOver")]
    public CanvasGroup gameOverPanel;

    [Header("Win")]
    public CanvasGroup winPanel;

    private Coroutine currentFade;
    public float fadeDuration = 1f;

    public void ShowGameOver()
    {
        HideAllPanels();
        SetPanelOrder(gameOverPanel);
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeMenu(true, gameOverPanel));
    }

    public void ShowWin()
    {
        HideAllPanels();
        SetPanelOrder(winPanel);
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeMenu(true, winPanel));
    }

    private void HideAllPanels()
    {
        // Instantly hide both panels before showing the new one
        gameOverPanel.alpha = 0;
        gameOverPanel.interactable = false;
        gameOverPanel.blocksRaycasts = false;

        winPanel.alpha = 0;
        winPanel.interactable = false;
        winPanel.blocksRaycasts = false;
    }

    private void SetPanelOrder(CanvasGroup panel)
    {
        panel.transform.SetAsLastSibling(); // Bring to top
    }

    private IEnumerator FadeMenu(bool show, CanvasGroup menu)
    {
        float startAlpha = menu.alpha;
        float endAlpha = show ? 1 : 0;
        float elapsedTime = 0f;

        // Enable raycasts immediately for smooth transition
        if (show)
        {
            menu.blocksRaycasts = true;
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            menu.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        menu.alpha = endAlpha;

        // Fully enable/disable interactions only when fade completes
        menu.interactable = show;
        menu.blocksRaycasts = show;
    }
}
