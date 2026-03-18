using UnityEngine;

public class UnguidedMissile : MonoBehaviour
{
    [SerializeField] float speed = 200f;
    [SerializeField] float maxDistance = 500f;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] GameObject explosionPrefab;

    Vector3 launchDirection;
    Vector3 startPosition;
    bool launched = false;
    GameObject owner;

    public void Launch(float launchSpeed, GameObject shooter)
    {
        if (launched) return;

        launched = true;
        owner = shooter;

        transform.parent = null;

        speed = launchSpeed;
        launchDirection = transform.forward;
        startPosition = transform.position;

        if (smoke != null)
            smoke.Play();
    }

    private void Update()
    {
        if (!launched) return;

        transform.position += launchDirection * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!launched) return;

        if (other.gameObject == owner)
            return;

        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.KillInstantly();
        }

        Explode();
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(explosion, 3f);
        }

        HandleSmoke();
        Destroy(gameObject);
    }

    void HandleSmoke()
    {
        if (smoke != null)
        {
            smoke.transform.parent = null;
            smoke.Stop();
            Destroy(smoke.gameObject, 10f);
        }
    }
}