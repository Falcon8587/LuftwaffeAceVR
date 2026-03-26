using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float damageRadius = 20f;
    public float damage = 100f;
    public GameObject explosionPrefab;
    public float lifeTime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (var hit in hits)
        {
            Health h = hit.GetComponentInParent<Health>();
            if (h != null)
                h.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}