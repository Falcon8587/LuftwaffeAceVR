using UnityEngine;

public class JerichoSiren : MonoBehaviour
{
    public AudioSource siren;

    [Header("Dive Settings")]
    public float triggerAngle = 65f; // degrees
    public float startDelay = 0.5f;

    [Header("Fade Settings")]
    public float fadeSpeed = 2f;

    [Header("Tail Loop")]
    public float loopDuration = 2f;

    private float clipLength;
    private float delayTimer = 0f;

    private bool isDiving = false;
    private bool hasStarted = false;

    void Start()
    {
        if (siren.clip != null)
            clipLength = siren.clip.length;
    }

    void Update()
    {
        HandleDiveSiren();
    }

    void HandleDiveSiren()
    {
        float diveAngle = Vector3.Angle(transform.forward, Vector3.down);

        // Check if steep enough
        if (diveAngle < triggerAngle)
        {
            isDiving = true;
            delayTimer += Time.deltaTime;

            // Wait before starting
            if (delayTimer >= startDelay)
            {
                if (!siren.isPlaying && !hasStarted)
                {
                    siren.Play();
                    hasStarted = true;
                }
            }
        }
        else
        {
            // Not diving → reset delay
            isDiving = false;
            delayTimer = 0f;
        }

        // Fade logic
        if (isDiving && hasStarted)
        {
            // Fade IN
            siren.volume = Mathf.Lerp(siren.volume, 1f, Time.deltaTime * fadeSpeed);

            HandleTailLoop();
        }
        else
        {
            // Fade OUT
            siren.volume = Mathf.Lerp(siren.volume, 0f, Time.deltaTime * fadeSpeed);

            // Stop when silent
            if (siren.volume < 0.05f && siren.isPlaying)
            {
                siren.Stop();
                hasStarted = false;
            }
        }
    }

    void HandleTailLoop()
    {
        if (siren.clip == null) return;

        float loopStart = clipLength - loopDuration;

        if (siren.time >= loopStart)
        {
            siren.time = loopStart + 0.05f;
        }
    }
}