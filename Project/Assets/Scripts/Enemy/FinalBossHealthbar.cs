using UnityEngine;
using UnityEngine.UI;

public class FinalBossHealthbar : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Slider slider;

    private float currentHealth;
    private float MaxHealth;
    void Start()
    {
        MaxHealth = enemy.GetComponent<Health>().GetMaxHealth();
        currentHealth = MaxHealth;
    }
    void Update()
    {
        currentHealth = enemy.GetComponent<Health>().GetCurrentHealth();
        slider.value = currentHealth / MaxHealth;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
