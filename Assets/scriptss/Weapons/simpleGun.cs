using TMPro;
using UnityEngine;

public class SimpleGun : MonoBehaviour
{
    public Transform[] firePoints;
    public float range = 1000f;
    public float fireRate = 10f;
    public float damage = 10f;

    public LineRenderer linePrefab;
    public float lineDuration = 0.05f;

    [Header("Gun Smoke")]
    public ParticleSystem gunSmokeLeft;
    public ParticleSystem gunSmokeRight;

    [Header("Gun Sound")]
    public AudioSource gunSound;

    float nextTimeToFire = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0)) // HOLD LEFT MOUSE
        {
            // PLAY SOUND
            if (gunSound != null && !gunSound.isPlaying)
                gunSound.Play();

            // START SMOKE
            if (gunSmokeLeft != null && !gunSmokeLeft.isPlaying)
                gunSmokeLeft.Play();

            if (gunSmokeRight != null && !gunSmokeRight.isPlaying)
                gunSmokeRight.Play();

            // FIRE BULLETS
            if (Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else
        {
            // STOP SOUND
            if (gunSound != null && gunSound.isPlaying)
                gunSound.Stop();

            // STOP SMOKE
            if (gunSmokeLeft != null && gunSmokeLeft.isPlaying)
                gunSmokeLeft.Stop();

            if (gunSmokeRight != null && gunSmokeRight.isPlaying)
                gunSmokeRight.Stop();
        }
        {
            if (Input.GetMouseButtonDown(0) && ammo > 0)
            {
                ammo--;
                ammoText.text = "Ammo: " + ammo;
            }
        }
    }

    void Shoot()
    {
        foreach (Transform fp in firePoints)
        {
            RaycastHit hit;
            Vector3 hitPoint;

            if (Physics.Raycast(fp.position, fp.forward, out hit, range))
            {
                hitPoint = hit.point;

                Debug.Log("Hit " + hit.transform.name);

                Health targetHealth = hit.transform.GetComponent<Health>();

                if (targetHealth != null)
                    targetHealth.TakeDamage(damage);
            }
            else
            {
                hitPoint = fp.position + fp.forward * range;
            }

            LineRenderer line = Instantiate(linePrefab);
            line.SetPosition(0, fp.position);
            line.SetPosition(1, hitPoint);

            Destroy(line.gameObject, lineDuration);
        }
    }
    public TMP_Text ammoText;
    int ammo = 1000;

    void Start()
    {
        ammoText.text = "Ammo: " + ammo;
    }

}