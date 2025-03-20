using UnityEngine;

public class MeleePlayer : MonoBehaviour
{
    [Header("Attack Parameter")]
    //[SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    [SerializeField] private float range;

    [Header("Collider Parameter")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Enemy Layer")]
    [SerializeField] private LayerMask enemyMask;

    private Health enemyHealth;
    private bool InRange()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0f,
            Vector2.right,
            0f,
            enemyMask);
        if (hit.collider != null)
        {
            enemyHealth = hit.transform.GetComponent<Health>();
        }
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
    public void DamageEnemy()
    {
        if (InRange())
        {
            if (enemyHealth.GetComponent<Animator>() != null)
                if (enemyHealth.GetComponent<Animator>().GetBool("isDeath")) return;
            enemyHealth.TakeDamage(damage);
        }
    }
}
