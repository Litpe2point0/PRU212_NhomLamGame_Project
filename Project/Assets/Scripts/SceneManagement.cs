using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void LoadStage1(float delay)
    {
        StartCoroutine(WaitAndLoad("Stage1", delay));
    }
    public void LoadCurrentStage(float delay)
    {
        StartCoroutine(WaitAndLoad(SceneManager.GetActiveScene().name, delay));
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadGameStart(float delay)
    {
        StartCoroutine(WaitAndLoad("GameStart", delay));
    }
    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
