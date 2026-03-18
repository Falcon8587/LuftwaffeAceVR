using UnityEngine;

public class PlaneCrashHandler : MonoBehaviour
{
    private Health health;
    private Rigidbody rb;

    [Header("Crash Settings")]
    public float crashTorque = 40f;
    public float crashForwardForce = 30f;

    void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();

        if (health != null)
            health.OnDeath += HandleCrash;
    }

    void HandleCrash()
    {
        Debug.Log($"{gameObject.name} crashed!");

        DisableAllControlScripts();

        EnableCrashPhysics();
    }

    void DisableAllControlScripts()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            if (script == this)
                continue;

            if (script.GetType().Name == "PlayerDeathHandler")
                continue;

            script.enabled = false;
        }
    }

    void EnableCrashPhysics()
    {
        if (rb == null) return;

        rb.useGravity = true;

        // Make it fall faster
        rb.linearDamping = 0.1f;      // less air resistance
        rb.angularDamping = 0.05f;

        rb.mass = 3f;                // increase weight (important)

        // FORCE NOSE DOWN HARD
        rb.AddTorque(transform.right * 150f, ForceMode.Impulse);

        // Add roll spin
        rb.AddTorque(transform.forward * 80f, ForceMode.Impulse);

        // Push downward aggressively
        rb.AddForce(Vector3.down * 200f, ForceMode.Impulse);

        // Keep forward motion so it dives
        rb.AddForce(transform.forward * 120f, ForceMode.Impulse);
    }
}