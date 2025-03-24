using UnityEngine;
using UnityEngine.UI;

public class FireWizardHealthbar : MonoBehaviour
{
    [SerializeField] private float Health = 80f;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Slider slider;

    private float currentHealth;
    void Start()
    {
        enemy.GetComponent<Health>().SetMaxHealth(Health);
        currentHealth = Health;
    }
    void Update()
    {
        currentHealth = enemy.GetComponent<Health>().GetCurrentHealth();
        slider.value = currentHealth/Health;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
