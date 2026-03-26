using UnityEngine;

public class BombDropper : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public Transform bombSpawnPoint;

    public int maxBombs = 4;
    int currentBombs;

    public float reloadTime = 5f;
    float reloadTimer;

    [Header("Reticle Settings")]
    public Transform reticle; // assign your BombReticle object
    public LayerMask groundMask;

    public float maxSimulationTime = 10f;
    public float simulationStep = 0.05f;

    Rigidbody planeRb;

    void Start()
    {
        planeRb = GetComponent<Rigidbody>();
        currentBombs = maxBombs;
    }

    void Update()
    {
        var input = InputManager.Instance;
        if (input == null) return;

        //HandleBombDrop(input);
        HandleReload();
        UpdateReticle();
    }

    // =======================
    //  BOMB DROP
    // =======================
    //void HandleBombDrop(InputManager input)
    //{
    //    if (input.BombHeld && currentBombs > 0)
    //    {
    //        DropBomb();
    //    }
    //}

    void DropBomb()
    {
        currentBombs--;

        GameObject bomb = Instantiate(
            bombPrefab,
            bombSpawnPoint.position,
            bombSpawnPoint.rotation
        );

        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();

        if (bombRb != null)
        {
            // KEY: inherit plane velocity
            bombRb.linearVelocity = planeRb.linearVelocity;
        }

        Debug.Log("Bomb Dropped. Remaining: " + currentBombs);
    }

    // =======================
    // RELOAD SYSTEM
    // =======================
    void HandleReload()
    {
        if (currentBombs < maxBombs)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= reloadTime)
            {
                currentBombs++;
                reloadTimer = 0f;

                Debug.Log("Bomb Reloaded. Count: " + currentBombs);
            }
        }
    }

    // =======================
    // RETICLE SYSTEM
    // =======================
    void UpdateReticle()
    {
        if (reticle == null || bombSpawnPoint == null || planeRb == null) return;

        Vector3 position = bombSpawnPoint.position;
        Vector3 velocity = planeRb.linearVelocity;

        float time = 0f;

        while (time < maxSimulationTime)
        {
            velocity += Physics.gravity * simulationStep;
            position += velocity * simulationStep;

            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 5f, groundMask))
            {
                reticle.position = hit.point + Vector3.up * 0.5f;

                // Optional: face camera (VR friendly)
                if (Camera.main)
                    reticle.forward = Camera.main.transform.forward;

                return;
            }

            time += simulationStep;
        }
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnBombPressed += DropBomb;
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnBombPressed -= DropBomb;
    }
}