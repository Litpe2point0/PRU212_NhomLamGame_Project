using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health player = collision.GetComponent<Health>();
            player.TakeDamage(9999);
        }
    }
}
