using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 3f;
    public int maxHealth = 100;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    public float knockbackSmoothness = 5f;
    public GameObject deathEffect;
    public GameObject healthBarPrefab;

    private int currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private float knockbackEndTime;
    private HealthBarEnemy healthBarInstance;

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        // Instantiate and attach the health bar
        GameObject healthBarObject = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, transform);
        healthBarInstance = healthBarObject.GetComponent<HealthBarEnemy>();
    }

    private void Update()
    {
        if (isKnockedBack)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, knockbackSmoothness * Time.deltaTime);
            if (Time.time >= knockbackEndTime)
            {
                isKnockedBack = false;
            }
        }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }

        // Update health bar position to follow the enemy
        if (healthBarInstance != null)
        {
            Vector3 healthBarPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            healthBarInstance.transform.position = Camera.main.WorldToScreenPoint(healthBarPosition);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Vector2 knockbackDirection = (transform.position - player.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;

        if (healthBarInstance != null)
        {
            healthBarInstance.UpdateHealthBar(maxHealth, currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }

        Destroy(gameObject);
    }
}
