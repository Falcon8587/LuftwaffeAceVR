using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JerichoSiren : MonoBehaviour
{
    [Header("Audio Source Setup")]
    [SerializeField] GameObject sirenSourceObject;

    AudioSource sirenAudio;
    Rigidbody rb;

    [Header("Activation Settings")]
    [SerializeField] float minDiveAngle = 70f; // easier to trigger for testing
    [SerializeField] float minSpeed = 10f;

    [Header("Audio Settings")]
    [SerializeField] float minVolume = 0f;
    [SerializeField] float maxVolume = 1f;

    [SerializeField] float minPitch = 0.8f;
    [SerializeField] float maxPitch = 2.0f;

    [SerializeField] float fadeSpeed = 2f;

    float currentVolume;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Get AudioSource
        if (sirenSourceObject != null)
            sirenAudio = sirenSourceObject.GetComponent<AudioSource>();
        else
            sirenAudio = GetComponent<AudioSource>();

        if (sirenAudio == null)
        {
            Debug.LogError("JerichoSiren: No AudioSource found!");
            return;
        }

        sirenAudio.loop = true;
        sirenAudio.playOnAwake = false;

        Debug.Log("Jericho Siren initialized");
    }

    void Update()
    {
        if (sirenAudio == null) return;

        float speed = rb.linearVelocity.magnitude;
        float diveAngle = Vector3.Angle(transform.forward, Vector3.down);

        Debug.Log($"Speed: {speed} | DiveAngle: {diveAngle}");

        bool isDiving = diveAngle < minDiveAngle && speed > minSpeed;

        if (isDiving)
        {
            Debug.Log("DIVING - SIREN ACTIVE");

            if (!sirenAudio.isPlaying)
                sirenAudio.Play();

            float speedFactor = Mathf.InverseLerp(minSpeed, 150f, speed);
            float angleFactor = Mathf.InverseLerp(minDiveAngle, 0f, diveAngle);

            float intensity = speedFactor * angleFactor;

            currentVolume = Mathf.Lerp(currentVolume, intensity, Time.deltaTime * fadeSpeed);

            sirenAudio.volume = Mathf.Lerp(minVolume, maxVolume, currentVolume);
            sirenAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedFactor);
        }
        else
        {
            currentVolume = Mathf.Lerp(currentVolume, 0f, Time.deltaTime * fadeSpeed);
            sirenAudio.volume = currentVolume;

            if (currentVolume < 0.01f && sirenAudio.isPlaying)
                sirenAudio.Stop();
        }
    }
}