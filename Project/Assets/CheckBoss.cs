using UnityEngine;
using UnityEngine.UI;

public class CheckBoss : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject boss;
    [SerializeField] private BossTrigger bossTrigger;
    private bool isDead = false;
    private void Update()
    {
        if (!isDead)
        {
            if(boss.GetComponent<Health>().GetCurrentHealth() <=0)
            {
                canvas.enabled = false;
                bossTrigger.SwitchBounderBack();
                isDead = true;
            }
        }
    }
}
