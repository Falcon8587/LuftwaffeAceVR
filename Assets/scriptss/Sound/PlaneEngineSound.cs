using UnityEngine;

public class PlaneEngineSound : MonoBehaviour
{
    public AudioSource engineSound;

    public float minPitch = 0.8f;
    public float maxPitch = 1.5f;

    public float minVolume = 0.3f;
    public float maxVolume = 1f;

    PhysicsPlaneController plane;

    void Start()
    {
        plane = GetComponent<PhysicsPlaneController>();
        engineSound.Play();
    }

    void Update()
    {
        float throttle = plane.GetThrottle(); // value 0–1

        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, throttle);
        engineSound.volume = Mathf.Lerp(minVolume, maxVolume, throttle);
    }
}