using UnityEngine;
using System;   // <-- ADD THIS

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    public event Action OnDeath;   // <-- Important

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void KillInstantly()
    {
        if (isDead) return;
        Die();
    }
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    public GameObject explosionPrefab;
    void Die()
    {
        if (isDead) return;

        isDead = true;

        OnDeath?.Invoke();

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}