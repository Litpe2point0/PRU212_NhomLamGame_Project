using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GreatswordSkeletonSwitch : MonoBehaviour
{
    [SerializeField] private float Health = 80f;
    [SerializeField] private GameObject[] enemyObjects;
    [SerializeField] private float switchCooldown = 10f;

    private float currentHealth;
    private float switchTimer = 0f;
    private int activeEnemyIndex = -1;
    private int enemyCount = 1;
    private Collider2D playerCollider;
    private void Awake()
    {
        currentHealth = Health;
        if (enemyObjects.Length == 0)
        {
            Debug.LogWarning("No enemy objects assigned!");
            return;
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();
        foreach (GameObject enemy in enemyObjects)
        {
            var attack = enemy.GetComponent<GreatswordSkeletonAttack>();
            var movement = enemy.GetComponent<GreatswordSkeletonMovement>();
            var health = enemy.GetComponent<Health>();
            var collider = enemy.GetComponent<Collider2D>();
            var anim = enemy.GetComponent<Animator>();

            if (attack) attack.enabled = false;
            if (movement) movement.enabled = false;
            if (health) health.SetMaxHealth(currentHealth);
            if (anim)
            {
                anim.SetTrigger("Death");
                anim.SetBool("isDeath", true);
                anim.SetBool("Grounded", true);
            }
            if (collider && playerCollider)
            {
                Physics2D.IgnoreCollision(collider, playerCollider, true); // Re-enable collision
            }
        }
    }

    void Start()
    {
        if (enemyObjects.Length == 0) return; // Safety check

        // Disable all Health components first
        foreach (GameObject enemy in enemyObjects)
        {
            var health = enemy.GetComponent<Health>();
            if (health) health.enabled = false;
        }

        // Randomly activate ONE enemy
        int temp = Random.Range(0, enemyObjects.Length);
        var selectedEnemy = enemyObjects[temp];

        var attack = selectedEnemy.GetComponent<GreatswordSkeletonAttack>();
        var movement = selectedEnemy.GetComponent<GreatswordSkeletonMovement>();
        var healthComp = selectedEnemy.GetComponent<Health>();
        var anim = selectedEnemy.GetComponent<Animator>();
        var collider = selectedEnemy.GetComponent<Collider2D>();

        if (attack) attack.enabled = true;
        if (movement) movement.enabled = true;
        if (healthComp) healthComp.enabled = true;
        if (anim)
        {
            anim.SetTrigger("Resurrection");
            anim.SetBool("isDeath", false);
        }
        if (collider && playerCollider)
        {
            Physics2D.IgnoreCollision(collider, playerCollider, false); // Re-enable collision
        }
    }
    private void Update()
    {
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
    private void SwitchEnemy(int value)
    {
        if (enemyObjects.Length == 0) return;

        // Disable all enemies first
        foreach (GameObject enemy in enemyObjects)
        {
            var attack = enemy.GetComponent<GreatswordSkeletonAttack>();
            var movement = enemy.GetComponent<GreatswordSkeletonMovement>();
            var health = enemy.GetComponent<Health>();
            var collider = enemy.GetComponent<Collider2D>();
            var anim = enemy.GetComponent<Animator>();

            if (attack) attack.enabled = false;
            if (movement) movement.enabled = false;
            if (health) health.enabled = false;
            if (collider && playerCollider)
            {
                Physics2D.IgnoreCollision(collider, playerCollider, true); // Disable collision
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

            var attack = selectedEnemy.GetComponent<GreatswordSkeletonAttack>();
            var movement = selectedEnemy.GetComponent<GreatswordSkeletonMovement>();
            var health = selectedEnemy.GetComponent<Health>();
            var collider = selectedEnemy.GetComponent<Collider2D>();
            var anim = selectedEnemy.GetComponent<Animator>();
            if (attack) attack.enabled = true;
            if (movement) movement.enabled = true;
            if (health)
            {
                health.enabled = true;
                health.SetCurrentHealth(Health);
            }
            if (anim)
                if (anim.GetBool("isDeath"))
                {
                    anim.SetTrigger("Resurrection");
                    anim.SetBool("isDeath", false);
                }
            if (collider && playerCollider)
            {
                Physics2D.IgnoreCollision(collider, playerCollider, false); // Re-enable collision
            }
        }

        // Update active enemies
        activeEnemyIndex = selectedIndices.First();

        Debug.Log($"Activated {value} enemies: " + string.Join(", ", selectedIndices));
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage); // Reduce shared health

        // Update all active enemies' health
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
}
