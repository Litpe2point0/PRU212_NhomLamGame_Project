using Unity.Cinemachine;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioSource cameraAudio;
    [SerializeField] private Collider2D originalConfiner;
    [SerializeField] private Collider2D newConfiner;
    [SerializeField] private GameObject[] bossObjects;

    void Start()
    {
        foreach (GameObject bossObject in bossObjects)
        {
            bossObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cinemachineConfiner.BoundingShape2D = newConfiner;
            cinemachineConfiner.InvalidateBoundingShapeCache(); // Refresh the confiner
            cameraAudio.clip = bossMusic;
            cameraAudio.Play();
            foreach (GameObject bossObject in bossObjects)
            {
                bossObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
    public void SwitchBounderBack()
    {
        cinemachineConfiner.BoundingShape2D = originalConfiner;
        cinemachineConfiner.InvalidateBoundingShapeCache();
    }
}
