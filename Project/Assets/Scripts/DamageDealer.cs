using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damage = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
