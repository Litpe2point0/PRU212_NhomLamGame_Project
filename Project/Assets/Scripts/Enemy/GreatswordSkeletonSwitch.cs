using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GreatswordSkeletonSwitch : MonoBehaviour
{
    [SerializeField] private float Health = 80f;
    [SerializeField] private GameObject[] enemyObjects;
    [SerializeField] private float switchCooldown = 10f;
    [SerializeField] private Slider slider;

    private float currentHealth = 2f;
    private float switchTimer = 0f;
    private int activeEnemyIndex = -1;
    private int enemyCount = 1;
    private void Awake()
    {
        currentHealth = Health;
    }
    void Start()
    {
        currentHealth = Health;
        if (enemyObjects.Length == 0)
        {
            Debug.LogWarning("No enemy objects assigned!");
            return;
        }
        foreach (GameObject enemy in enemyObjects)
        {
            var anim1 = enemy.GetComponent<Animator>();
            var attack1 = enemy.GetComponent<GreatswordSkeletonAttack>();
            var movement1 = enemy.GetComponent<GreatswordSkeletonMovement>();

            if (attack1 != null) attack1.SetIsActive(false);
            if (movement1 != null)
            {
                movement1.SetIsActive(false);
                movement1.SetCollision(false);
            }
            if (anim1)
            {
                anim1.SetTrigger("Death");
                anim1.SetBool("isDeath", true);
                anim1.SetBool("Grounded", true);
            }
        }
        if (enemyObjects.Length == 0) return;

        // Randomly activate ONE enemy
        int temp = Random.Range(0, enemyObjects.Length);
        var selectedEnemy = enemyObjects[temp];

        var anim = selectedEnemy.GetComponent<Animator>();
        var attack = selectedEnemy.GetComponent<GreatswordSkeletonAttack>();
        var movement = selectedEnemy.GetComponent<GreatswordSkeletonMovement>();

        if (attack != null) attack.SetIsActive(true);
        if (movement != null)
        {
            movement.SetIsActive(true);
            movement.SetCollision(true);
        }
        if (anim)
        {
            anim.SetTrigger("Resurrection");
            anim.SetBool("isDeath", false);
        }
        SetHealth();
    }
    private void Update()
    {
        Debug.Log(currentHealth);
        slider.value = currentHealth / Health;
        if (currentHealth <= 0) return;
        switchTimer += Time.deltaTime;
        if (switchTimer >= switchCooldown)
        {
            switchTimer = 0;
            SwitchEnemy(enemyCount);
        }
        if (currentHealth <= Health * 2 / 3)
        {
            enemyCount = 2;
        }
        if (currentHealth <= Health / 3)
        {
            enemyCount = 3;
        }
    }
    void SetHealth()
    {
        foreach (var enemy in enemyObjects)
        {
            enemy.GetComponent<Health>().SetCurrentHealth(currentHealth);
        }
    }
    private void SwitchEnemy(int value)
    {
        if (enemyObjects.Length == 0) return;

        // Disable all enemies first
        foreach (GameObject enemy in enemyObjects)
        {
            var anim = enemy.GetComponent<Animator>();
            var attack = enemy.GetComponent<GreatswordSkeletonAttack>();
            var movement = enemy.GetComponent<GreatswordSkeletonMovement>();

            if (attack != null) attack.SetIsActive(false);
            if (movement != null)
            {
                movement.SetIsActive(false);
                movement.SetCollision(false);
            }
            if (anim)
                if (!anim.GetBool("isDeath"))
                {
                    anim.SetTrigger("Death");
                    anim.SetBool("isDeath", true);
                }
        }

        // Ensure value doesn't exceed available enemies
        value = Mathf.Clamp(value, 1, enemyObjects.Length);

        // Pick unique random indices for enemies to activate
        HashSet<int> selectedIndices = new HashSet<int>();
        while (selectedIndices.Count < value)
        {
            int newIndex = Random.Range(0, enemyObjects.Length);
            selectedIndices.Add(newIndex);
        }

        // Activate selected enemies
        foreach (int index in selectedIndices)
        {
            var selectedEnemy = enemyObjects[index];

            var anim = selectedEnemy.GetComponent<Animator>();
            var attack = selectedEnemy.GetComponent<GreatswordSkeletonAttack>();
            var movement = selectedEnemy.GetComponent<GreatswordSkeletonMovement>();

            if (attack != null) attack.SetIsActive(true);
            if (movement != null)
            {
                movement.SetIsActive(true);
                movement.SetCollision(true);
            }
            if (anim)
                if (anim.GetBool("isDeath"))
                {
                    anim.SetTrigger("Resurrection");
                    anim.SetBool("isDeath", false);
                }
        }

        // Update active enemies
        activeEnemyIndex = selectedIndices.First();

        Debug.Log($"Activated {value} enemies: " + string.Join(", ", selectedIndices));
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        foreach (GameObject enemy in enemyObjects)
        {
            var health = enemy.GetComponent<Health>();
            if (health)
            {
                health.SetCurrentHealth(currentHealth);
            }
        }
        Debug.Log($"Shared Health: {currentHealth}");
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
