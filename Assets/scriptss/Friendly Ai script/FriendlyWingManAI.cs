using UnityEngine;

public class FriendlyArcadeAI : MonoBehaviour
{
    public enum AIState
    {
        InFormation,
        Attacking,
        Protecting
    }

    public AIState currentState = AIState.InFormation;

    [Header("Flight")]
    public float cruiseSpeed = 130f;
    public float boostSpeed = 160f;
    public float turnSpeed = 2.5f;

    [Header("Formation")]
    public Vector3 formationPosition;

    Vector3 smoothedFormationPos;

    [Header("Combat")]
    public float detectionRadius = 900f;
    public float fireRange = 650f;
    public float attackAngle = 25f;

    [Header("Banking")]
    public float bankAngle = 60f;
    public float bankSpeed = 4f;

    float currentBank;

    Rigidbody rb;
    Transform enemyTarget;
    Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = 0f;
        rb.angularDamping = 2f;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb.linearVelocity = transform.forward * cruiseSpeed;

        smoothedFormationPos = formationPosition;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        switch (currentState)
        {
            case AIState.InFormation:
                HandleFormation();
                break;

            case AIState.Attacking:
                HandleAttack();
                break;

            case AIState.Protecting:
                HandleProtect();
                break;
        }
    }

    void HandleFormation()
    {
        smoothedFormationPos = Vector3.Lerp(smoothedFormationPos, formationPosition, Time.fixedDeltaTime * 2f);

        Vector3 dir = (smoothedFormationPos - transform.position).normalized;

        RotateTowards(dir);

        float playerSpeed = player.GetComponent<Rigidbody>().linearVelocity.magnitude;

        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            dir * playerSpeed,
            Time.fixedDeltaTime * 2f
        );
    }

    void HandleAttack()
    {
        if (enemyTarget == null)
            enemyTarget = FindNearestEnemy();

        if (enemyTarget == null)
        {
            currentState = AIState.InFormation;
            return;
        }

        Vector3 dir = (enemyTarget.position - transform.position).normalized;

        RotateTowards(dir);

        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            dir * boostSpeed,
            Time.fixedDeltaTime * 2f
        );
    }

    void HandleProtect()
    {
        Transform enemy = FindNearestEnemy();

        if (enemy != null &&
            Vector3.Distance(enemy.position, player.position) < detectionRadius)
        {
            enemyTarget = enemy;
            HandleAttack();
            return;
        }

        HandleFormation();
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float closest = Mathf.Infinity;
        Transform best = null;

        foreach (GameObject e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);

            if (d < closest)
            {
                closest = d;
                best = e.transform;
            }
        }

        return best;
    }

    void RotateTowards(Vector3 dir)
    {
        Quaternion targetRot = Quaternion.LookRotation(dir);

        float turnAmount = Vector3.Dot(transform.right, dir);
        float targetBank = -turnAmount * bankAngle;

        currentBank = Mathf.Lerp(
            currentBank,
            targetBank,
            Time.fixedDeltaTime * bankSpeed
        );

        targetRot *= Quaternion.Euler(0, 0, currentBank);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRot,
                turnSpeed * Time.fixedDeltaTime
            )
        );
    }

    public void SetTarget(Transform target)
    {
        enemyTarget = target;
    }
}