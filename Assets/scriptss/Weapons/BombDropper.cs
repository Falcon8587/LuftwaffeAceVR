//using UnityEngine;

//public class BombDropper : MonoBehaviour
//{
//    [Header("Bomb Settings")]
//    public GameObject bombPrefab;
//    public Transform bombSpawnPoint;

//    public int maxBombs = 4;
//    int currentBombs;

//    public float reloadTime = 5f;
//    float reloadTimer;

//    [Header("Reticle Settings")]
//    public Transform reticle; // assign your BombReticle object
//    public LayerMask groundMask;

//    public float maxSimulationTime = 10f;
//    public float simulationStep = 0.05f;

//    Rigidbody planeRb;

//    void Start()
//    {
//        planeRb = GetComponent<Rigidbody>();
//        currentBombs = maxBombs;
//    }

//    void Update()
//    {
//        var input = InputManager.Instance;
//        if (input == null) return;

//        HandleBombDrop(input);
//        HandleReload();
//        UpdateReticle();
//    }

//    // =======================
//    //  BOMB DROP
//    // =======================
//    void HandleBombDrop(InputManager input)
//    {
//        if (input.BombHeld && currentBombs > 0)
//        {
//            DropBomb();
//        }
//    }

//    void DropBomb()
//    {
//        currentBombs--;

//        GameObject bomb = Instantiate(
//            bombPrefab,
//            bombSpawnPoint.position,
//            bombSpawnPoint.rotation
//        );

//        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();

//        if (bombRb != null)
//        {
//            //  KEY: inherit plane velocity
//            bombRb.linearVelocity = planeRb.linearVelocity;
//        }

//        Debug.Log("Bomb Dropped. Remaining: " + currentBombs);
//    }

//    // =======================
//    //  RELOAD SYSTEM
//    // =======================
//    void HandleReload()
//    {
//        if (currentBombs < maxBombs)
//        {
//            reloadTimer += Time.deltaTime;

//            if (reloadTimer >= reloadTime)
//            {
//                currentBombs++;
//                reloadTimer = 0f;

//                Debug.Log("Bomb Reloaded. Count: " + currentBombs);
//            }
//        }
//    }

//    // =======================
//    //  RETICLE SYSTEM
//    // =======================
//    void UpdateReticle()
//    {
//        if (reticle == null || bombSpawnPoint == null || planeRb == null) return;

//        Vector3 position = bombSpawnPoint.position;
//        Vector3 velocity = planeRb.linearVelocity;

//        float time = 0f;

//        while (time < maxSimulationTime)
//        {
//            velocity += Physics.gravity * simulationStep;
//            position += velocity * simulationStep;

//            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 5f, groundMask))
//            {
//                reticle.position = hit.point + Vector3.up * 0.5f;

//                // Optional: face camera (VR friendly)
//                if (Camera.main)
//                    reticle.forward = Camera.main.transform.forward;

//                return;
//            }

//            time += simulationStep;
//        }
//    }
//}

//using UnityEngine;
//using UnityEngine.Windows;
//using TMPro;

//public class BombDropper : MonoBehaviour
//{
//    [Header("Bomb Settings")]
//    public GameObject bombPrefab;
//    public Transform bombSpawnPoint;

//    public int maxBombs = 4;
//    int currentBombs;

//    public float reloadTime = 5f;
//    float reloadTimer;

//    Rigidbody planeRb;

//    [Header("UI Reticle")]
//    public RectTransform reticleUI;
//    public Canvas canvas;
//    public LayerMask groundMask;
//    [SerializeField] Camera cam;

//    [Header("UI")]
//    public TMP_Text bombText;

//    public float maxSimulationTime = 10f;
//    public float simulationStep = 0.05f;

//    void Start()
//    {
//        planeRb = GetComponent<Rigidbody>();
//        currentBombs = maxBombs;

//        if (cam == null)
//            cam = Camera.main;

//        if (cam == null)
//            Debug.LogError(" No Camera assigned to BombDropper!");

//        UpdateBombUI();
//    }

//    void OnEnable()
//    {
//        if (InputManager.Instance != null)
//            InputManager.Instance.OnBombPressed += DropBomb;
//    }

//    void OnDisable()
//    {
//        if (InputManager.Instance != null)
//            InputManager.Instance.OnBombPressed -= DropBomb;
//    }

//    void Update()
//    {
//        var input = InputManager.Instance;
//        HandleReload();
//        UpdateReticle();

//        HandleBombDrop(input);
//    }

//    // =======================
//    //  BOMB DROP
//    // =======================
//    void HandleBombDrop(InputManager input)
//    {
//        if (input.BombHeld && currentBombs > 0)
//        {
//            DropBomb();
//        }
//    }

//    void DropBomb()
//    {
//        currentBombs--;

//        GameObject bomb = Instantiate(
//            bombPrefab,
//            bombSpawnPoint.position,
//            bombSpawnPoint.rotation
//        );

//        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();

//        if (bombRb != null)
//        {
//            //  KEY: inherit plane velocity
//            bombRb.linearVelocity = planeRb.linearVelocity;
//        }

//        Debug.Log("Bomb Dropped. Remaining: " + currentBombs);

//        UpdateBombUI();
//    }

//    // =======================
//    //  RELOAD SYSTEM
//    // =======================
//    void HandleReload()
//    {
//        if (currentBombs < maxBombs)
//        {
//            reloadTimer += Time.deltaTime;

//            if (reloadTimer >= reloadTime)
//            {
//                currentBombs++;
//                reloadTimer = 0f;

//                Debug.Log("Bomb Reloaded. Count: " + currentBombs);

//                UpdateBombUI();
//            }
//        }
//    }

//    // =======================
//    //  UI RETICLE SYSTEM
//    // =======================
//    void UpdateReticle()
//    {
//        if (reticleUI == null || bombSpawnPoint == null || planeRb == null) return;

//        Vector3 position = bombSpawnPoint.position;
//        Vector3 velocity = planeRb.linearVelocity;

//        float time = 0f;

//        while (time < maxSimulationTime)
//        {
//            velocity += Physics.gravity * simulationStep;
//            position += velocity * simulationStep;

//            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 5f, groundMask))
//            {
//                Vector3 screenPos = cam.WorldToScreenPoint(hit.point);

//                // Hide if behind camera
//                if (screenPos.z < 0)
//                {
//                    reticleUI.gameObject.SetActive(false);
//                    return;
//                }

//                reticleUI.gameObject.SetActive(true);

//                // Convert to UI space
//                Vector2 uiPos;
//                RectTransformUtility.ScreenPointToLocalPointInRectangle(
//                    canvas.transform as RectTransform,
//                    screenPos,
//                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
//                    out uiPos
//                );

//                reticleUI.anchoredPosition = uiPos;

//                return;
//            }

//            time += simulationStep;
//        }

//        // No hit found → hide reticle
//        reticleUI.gameObject.SetActive(false);
//    }

//    void UpdateBombUI()
//    {
//        if (bombText != null)
//        {
//            bombText.text = "" + currentBombs;
//        }
//    }
//}

using UnityEngine;
using TMPro;

public class BombDropper : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public Transform bombSpawnPoint;

    public int maxBombs = 4;
    int currentBombs;

    public float reloadTime = 5f;
    float reloadTimer;

    Rigidbody planeRb;

    [Header("UI Reticle")]
    public RectTransform reticleUI;
    public Canvas canvas;
    public LayerMask groundMask;
    [SerializeField] Camera cam;

    [Header("UI")]
    public TMP_Text bombText;

    public float maxSimulationTime = 10f;
    public float simulationStep = 0.05f;

    void Start()
    {
        planeRb = GetComponent<Rigidbody>();
        currentBombs = maxBombs;

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            Debug.LogError(" No Camera assigned to BombDropper!");

        UpdateBombUI();
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

    void Update()
    {
        HandleReload();
        UpdateReticle();
    }

    // =======================
    //  BOMB DROP (ONE PER PRESS)
    // =======================
    void DropBomb()
    {
        if (currentBombs <= 0) return;

        currentBombs--;

        GameObject bomb = Instantiate(
            bombPrefab,
            bombSpawnPoint.position,
            bombSpawnPoint.rotation
        );

        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();

        if (bombRb != null)
        {
            // Correct velocity inheritance
            bombRb.linearVelocity = planeRb.linearVelocity;
        }

        Debug.Log("Bomb Dropped. Remaining: " + currentBombs);

        UpdateBombUI();
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

                UpdateBombUI();
            }
        }
    }

    // =======================
    //  UI RETICLE SYSTEM
    // =======================
    void UpdateReticle()
    {
        if (reticleUI == null || bombSpawnPoint == null || planeRb == null || cam == null) return;

        Vector3 position = bombSpawnPoint.position;
        Vector3 velocity = planeRb.linearVelocity;

        float time = 0f;

        while (time < maxSimulationTime)
        {
            velocity += Physics.gravity * simulationStep;
            position += velocity * simulationStep;

            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 5f, groundMask))
            {
                Vector3 screenPos = cam.WorldToScreenPoint(hit.point);

                // Hide if behind camera
                if (screenPos.z < 0)
                {
                    reticleUI.gameObject.SetActive(false);
                    return;
                }

                reticleUI.gameObject.SetActive(true);

                // Convert to UI space
                Vector2 uiPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPos,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
                    out uiPos
                );

                reticleUI.anchoredPosition = uiPos;

                return;
            }

            time += simulationStep;
        }

        // No hit found → hide reticle
        reticleUI.gameObject.SetActive(false);
    }

    // =======================
    //  UI UPDATE
    // =======================
    void UpdateBombUI()
    {
        if (bombText != null)
        {
            bombText.text = "Bombs: " + currentBombs;
        }
    }
}