using UnityEngine;

public class Stage3Check : MonoBehaviour
{
    [SerializeField] private GameObject boss1;
    [SerializeField] private GameObject boss2;
    [SerializeField] private GameObject levelExit;

    private bool deactivated = false;
    void Update()
    {
        if(boss1.GetComponent<FireWizardHealthbar>().GetCurrentHealth() <= 0 && boss2.GetComponent<GreatswordSkeletonSwitch>().GetCurrentHealth() <= 0)
        {
            levelExit.SetActive(true);
        }
        else
        {
            levelExit.SetActive(false);
        }
    }
}
