using UnityEngine;
using System.Collections;
public class BombDropperAdvanced : MonoBehaviour
{
    [Header("Prefabs (REAL BOMBS)")]
    public GameObject smallBombPrefab;
    public GameObject bigBombPrefab;
    [Header("Spawn Points")]
    public Transform bigBombSpawn;
    public Transform[] smallBombSpawns; // 0,1 = LEFT | 2,3 = RIGHT
    [Header("Visual Bombs (FAKE)")]
    public GameObject[] smallBombVisuals; // same order as spawns
    public GameObject bigBombVisual;
    [Header("Settings")]
    public float dropForce = 25f;
    public float dropDelay = 0.4f;
    public float reloadTime = 8f;
    private bool bigBombDropped = false;
    private bool isDropping = false;
    private bool isReloading = false;
    private bool dropLeftSide = true;
    private int sidesDropped = 0;
    Rigidbody planeRb;
    Collider planeCol;
    void Start()
    {
        planeRb = GetComponent<Rigidbody>();
        planeCol = GetComponent<Collider>();
    }
    void Update()
    {
        // BIG BOMB
        if (Input.GetKeyDown(KeyCode.Space) && !bigBombDropped && !isReloading)
        {
            DropBigBomb();
        }
        // SMALL BOMBS
        if (Input.GetKeyDown(KeyCode.B) && !isDropping && !isReloading)
        {
            StartCoroutine(DropSideBombs());
        }
    }
    //  BIG BOMB
    void DropBigBomb()
    {
        // Hide visual
        if (bigBombVisual != null)
            bigBombVisual.SetActive(false);
        // Spawn real bomb
        GameObject bomb = Instantiate(
            bigBombPrefab,
            bigBombSpawn.position,
            bigBombSpawn.rotation
        );
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = planeRb.linearVelocity * 0.8f;
            rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
        }
        IgnorePlaneCollision(bomb);
        bigBombDropped = true;
    }
    //  LEFT / RIGHT SYSTEM
    IEnumerator DropSideBombs()
    {
        isDropping = true;
        if (smallBombSpawns.Length < 4)
        {
            Debug.LogWarning("Need 4 spawn points!");
            yield break;
        }
        if (dropLeftSide)
        {
            yield return StartCoroutine(DropBomb(0));
            yield return StartCoroutine(DropBomb(1));
        }
        else
        {
            yield return StartCoroutine(DropBomb(2));
            yield return StartCoroutine(DropBomb(3));
        }
        dropLeftSide = !dropLeftSide;
        sidesDropped++;
        isDropping = false;
        if (sidesDropped >= 2)
        {
            StartCoroutine(ReloadBombs());
            sidesDropped = 0;
        }
    }
    //  SINGLE BOMB DROP
    IEnumerator DropBomb(int index)
    {
        Transform spawn = smallBombSpawns[index];
        // Hide visual bomb
        if (smallBombVisuals[index] != null)
            smallBombVisuals[index].SetActive(false);
        // Spawn real bomb
        GameObject bomb = Instantiate(
            smallBombPrefab,
            spawn.position,
            spawn.rotation
        );
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = planeRb.linearVelocity * -0.8f;
            rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
        }
        IgnorePlaneCollision(bomb);
        yield return new WaitForSeconds(dropDelay);
    }
    //  IGNORE COLLISION
    void IgnorePlaneCollision(GameObject bomb)
    {
        Collider bombCol = bomb.GetComponent<Collider>();
        if (bombCol != null && planeCol != null)
        {
            Physics.IgnoreCollision(bombCol, planeCol);
        }
    }
    //  RELOAD
    IEnumerator ReloadBombs()
    {
        isReloading = true;
        Debug.Log("RELOADING...");
        yield return new WaitForSeconds(reloadTime);
        bigBombDropped = false;
        // Show small bomb visuals again
        for (int i = 0; i < smallBombVisuals.Length; i++)
        {
            if (smallBombVisuals[i] != null)
                smallBombVisuals[i].SetActive(true);
        }
        // Show big bomb visual again
        if (bigBombVisual != null)
            bigBombVisual.SetActive(true);
        isReloading = false;
        Debug.Log("RELOADED");
    }
}