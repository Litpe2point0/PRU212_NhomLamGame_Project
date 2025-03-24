using UnityEngine;

public class BossTrigger : MonoBehaviour
{
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
            foreach (GameObject bossObject in bossObjects)
            {
                bossObject.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}
