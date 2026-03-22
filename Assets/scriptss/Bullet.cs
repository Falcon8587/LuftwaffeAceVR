using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float damage = 10f;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health health = collision.transform.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("Hit: " + collision.transform.name);
        }

        Destroy(gameObject);
    }
}