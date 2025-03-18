using UnityEngine;

public class RangePlayer : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] projectilesPrefab;

    private void Awake()
    {

    }
    public void Attack()
    {
        projectilesPrefab[FindMagicOrb()].transform.position = firePoint.position;
        projectilesPrefab[FindMagicOrb()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
    private int FindMagicOrb()
    {
        for (int i = 0; i < projectilesPrefab.Length; i++)
        {
            if (!projectilesPrefab[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
