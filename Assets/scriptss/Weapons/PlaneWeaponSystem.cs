using UnityEngine;
using static missile;

public class PlaneWeaponSystem : MonoBehaviour
{
    [SerializeField] GameObject missilePrefab;

    [SerializeField] Transform firePointLeftM;
    [SerializeField] Transform firePointRightM;
    [SerializeField] GameObject leftMissileVisual;
    [SerializeField] GameObject rightMissileVisual;
    Rigidbody rb;

    bool fireLeftNext = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
   
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            float planeSpeed = rb.linearVelocity.magnitude * 2f;

            if (fireLeftNext && leftMissileVisual.activeSelf)
            {
                FireMissile(firePointLeftM);
                leftMissileVisual.SetActive(false);
                fireLeftNext = false;
            }
            else if (!fireLeftNext && rightMissileVisual.activeSelf)
            {
                FireMissile(firePointRightM);
                rightMissileVisual.SetActive(false);
                fireLeftNext = true;
            }
        }
        void FireMissile(Transform spawnPoint)
        {
            GameObject missileObj = Instantiate(
                missilePrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            UnguidedMissile missileScript = missileObj.GetComponent<UnguidedMissile>();

            if (missileScript != null)
            {
                float planeSpeed = rb.linearVelocity.magnitude * 2f;
                missileScript.Launch(planeSpeed, gameObject);
            }
        }
    }
}