using UnityEngine;

public class ArcadeEnemyAI : MonoBehaviour
{
    enum Maneuver
    {
        Chase,
        StrafeLeft,
        StrafeRight,
        LoopUp,
        HeadOn,
        BreakAway
    }

    public enum AIState
    {
        InFormation,
        Chasing,
        ReturningToFormation
    }

    [Header("Role")]
    public bool isLeader = false;

    AIState currentState = AIState.InFormation;
    Maneuver currentManeuver;

    [Header("Movement")]
    public float cruiseSpeed = 130f;
    public float boostSpeed = 160f;
    public float turnSpeed = 2.5f;
    public float maneuverTimeMin = 3f;
    public float maneuverTimeMax = 6f;
    [Header("Banking")]
    public float bankAngle = 60f;
    public float bankSpeed = 4f;
    float currentBank;

    [Header("Combat")]
    public float fireRange = 650f;
    public float fireRate = 0.12f;
    public float fireSpread = 0.08f;
    public float attackAngle = 25f;
    [Header("Aim Prediction")]
    public float bulletSpeed = 900f;

    [Header("Damage Settings")]
    public float bulletDamage = 20f;

    [Range(0f, 1f)]
    public float critChance = 0.1f;
    public float critMultiplier = 2f;

    public bool useDistanceFalloff = true;
    public float minDamageMultiplier = 0.5f;

    public bool useAngleBonus = true;
    public float headOnBonusMultiplier = 1.5f;

    [Header("Flyby")]
    public float flybyDistance = 120f;
    public float flybyBoostMultiplier = 1.2f;

    [Header("Barrel Roll")]
    public float rollSpeed = 350f;

    [Header("Altitude")]
    public float minAltitude = 100f;

    [Header("Formation")]
    public Vector3 formationPosition;

    [Header("Clamp Settings")]
    public float maxPitchAngle = 45f;
    public float minChaseDistance = 80f;

    [Header("Tracer Visual")]
    public GameObject tracerPrefab;
    public Transform firePoint;

    [Header("State Settings")]
    public float detectionRadius = 800f;
    public float chaseDuration = 30f;
    public float formationDuration = 10f;

    [Header("Regroup Settings")]
    public float regroupSpeedMultiplier = 2f;

    [Header("Separation")]
    public float separationDistance = 25f;
    public float separationStrength = 2f;

    [Header("Targeting")]
    public LayerMask shootMask;

    [Header("Audio")]
    public AudioSource gunSound;
    public AudioSource flybySound;

    Transform player;
    Rigidbody rb;

    float maneuverTimer;
    float fireTimer;
    float rollDirection;
    float currentSpeed;
    float stateTimer;

    bool flybyPlayed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = 0f;
        rb.angularDamping = 2f;

        currentSpeed = cruiseSpeed;

        PickNewManeuver();
    }

    void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case AIState.InFormation:
                if (!isLeader && Vector3.Distance(transform.position, player.position) < detectionRadius)
                {
                    currentState = AIState.Chasing;
                    stateTimer = chaseDuration;
                }
                break;

            case AIState.Chasing:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    currentState = AIState.ReturningToFormation;
                    stateTimer = formationDuration;
                }
                break;

            case AIState.ReturningToFormation:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    currentState = AIState.InFormation;
                }
                break;
        }

        maneuverTimer -= Time.deltaTime;
        if (maneuverTimer <= 0f)
            PickNewManeuver();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        MaintainAltitude();

        if (currentState == AIState.Chasing)
            HandleChasing();
        else
            HandleFormation();
    }

    void MaintainAltitude()
    {
        if (transform.position.y < minAltitude)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, minAltitude + 10f, Time.fixedDeltaTime * 2f);
            transform.position = pos;
        }
    }

    void HandleChasing()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        Vector3 dir;

        if (dist < minChaseDistance)
            dir = transform.forward;
        else
            dir = (player.position - transform.position).normalized;

        RotateTowards(dir);

        HandleSpeed(dist);

        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            transform.forward * currentSpeed,
            Time.fixedDeltaTime * 2f
        );

        HandleBarrelRoll();
        TryShoot();

        HandleFlyby(dist);
    }

    void HandleFormation()
    {
        if (isLeader)
        {
            rb.linearVelocity = transform.forward * cruiseSpeed;
            return;
        }

        Vector3 dirToFormation = (formationPosition - transform.position).normalized;
        Vector3 separation = GetSeparationForce();

        Vector3 finalDir = (dirToFormation + separation).normalized;

        RotateTowards(finalDir);

        float distance = Vector3.Distance(transform.position, formationPosition);

        float targetSpeed = cruiseSpeed;

        if (currentState == AIState.ReturningToFormation)
            targetSpeed *= regroupSpeedMultiplier;

        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            finalDir * targetSpeed,
            Time.fixedDeltaTime * 2f
        );
    }

    void RotateTowards(Vector3 dir)
    {
        Quaternion targetRot = Quaternion.LookRotation(dir);

        // Calculate bank angle
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

    void HandleSpeed(float dist)
    {
        float targetSpeed = cruiseSpeed;

        if (dist < flybyDistance)
            targetSpeed = boostSpeed * flybyBoostMultiplier;

        if (currentManeuver == Maneuver.BreakAway)
            targetSpeed = boostSpeed;

        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed,
            Time.fixedDeltaTime * 1.5f
        );
    }

    void HandleBarrelRoll()
    {
        if (rollDirection != 0f)
            transform.Rotate(Vector3.forward * rollDirection * rollSpeed * Time.fixedDeltaTime);
    }

    void HandleFlyby(float dist)
    {
        if (dist < flybyDistance && !flybyPlayed)
        {
            if (flybySound != null)
                flybySound.Play();

            flybyPlayed = true;
        }

        if (dist > flybyDistance * 2f)
            flybyPlayed = false;
    }

    void TryShoot()
    {
        fireTimer -= Time.fixedDeltaTime;
        if (fireTimer > 0f) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > fireRange) return;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Vector3 playerVelocity = playerRb != null ? playerRb.linearVelocity : Vector3.zero;

        float travelTime = distance / bulletSpeed;
        Vector3 predictedPos = player.position + playerVelocity * travelTime;

        Vector3 toPlayer = (predictedPos - transform.position).normalized;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > attackAngle) return;

        fireTimer = fireRate;

        // 🔊 PLAY GUN SOUND
        if (gunSound != null)
        {
            gunSound.PlayOneShot(gunSound.clip);
        }

        Vector3 shootDir = toPlayer;

        shootDir += new Vector3(
            Random.Range(-fireSpread, fireSpread),
            Random.Range(-fireSpread, fireSpread),
            Random.Range(-fireSpread, fireSpread)
        );

        Vector3 startPos = firePoint != null ? firePoint.position : transform.position;

        if (Physics.Raycast(startPos, shootDir, out RaycastHit hit, fireRange, shootMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Health playerHealth = hit.collider.GetComponentInParent<Health>();

                if (playerHealth != null)
                    playerHealth.TakeDamage(bulletDamage);
            }

            SpawnTracer(startPos, hit.point);
        }
        else
        {
            SpawnTracer(startPos, startPos + shootDir * fireRange);
        }
    }

    void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null) return;

        GameObject tracer = Instantiate(tracerPrefab, start, Quaternion.identity);
        LineRenderer lr = tracer.GetComponent<LineRenderer>();

        if (lr != null)
        {
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }

        Destroy(tracer, 0.05f);
    }

    void PickNewManeuver()
    {
        currentManeuver = (Maneuver)Random.Range(0, 6);
        maneuverTimer = Random.Range(maneuverTimeMin, maneuverTimeMax);

        rollDirection = 0f;

        if (currentManeuver == Maneuver.StrafeLeft)
            rollDirection = -1f;

        if (currentManeuver == Maneuver.StrafeRight)
            rollDirection = 1f;
    }

    Vector3 GetSeparationForce()
    {
        Vector3 force = Vector3.zero;

        Collider[] nearby = Physics.OverlapSphere(transform.position, separationDistance);

        foreach (Collider col in nearby)
        {
            if (col.gameObject == gameObject) continue;

            if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Vector3 away = transform.position - col.transform.position;
                force += away.normalized / away.magnitude;
            }
        }

        return force * separationStrength;
    }

    public void SetAIState(AIState newState)
    {
        currentState = newState;

        if (newState == AIState.Chasing)
            stateTimer = chaseDuration;

        if (newState == AIState.ReturningToFormation)
            stateTimer = formationDuration;
    }
}