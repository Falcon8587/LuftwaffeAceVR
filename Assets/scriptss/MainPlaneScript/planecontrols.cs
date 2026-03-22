//using UnityEngine;

//public class PhysicsPlaneController : MonoBehaviour
//{
//    [Header("Flight Settings")]
//    [SerializeField] float rotateSpeed = 80f;
//    [SerializeField] float rollSpeed = 120f;

//    [SerializeField] float maxSpeed = 80f;
//    [SerializeField] float acceleration = 20f;
//    [SerializeField] float deceleration = 15f;

//    [SerializeField] float stallSpeed = 25f;
//    [SerializeField] float stallFallSpeed = 10f;

//    [SerializeField] float pitchDrag = 0.6f;

//    Rigidbody rb;

//    float currentSpeed;

//    float horizontalInput;
//    float verticalInput;
//    float thrustInput;
//    float rollInput;

//    public bool rolling;

//    [Header("Engine Sound")]
//    AudioSource engineSound;

//    [SerializeField] float minPitch = 0.8f;
//    [SerializeField] float maxPitch = 1.6f;

//    [SerializeField] float minVolume = 0.35f;
//    [SerializeField] float maxVolume = 1f;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        engineSound = GetComponent<AudioSource>();

//        rb.interpolation = RigidbodyInterpolation.Interpolate;
//        Application.targetFrameRate = 60;

//        currentSpeed = maxSpeed * 0.4f;
//    }

//    void Update()
//    {
//        horizontalInput = Input.GetAxis("Horizontal");

//        verticalInput = Mathf.Clamp(
//            ((Input.mousePosition.y - (Screen.height / 2)) * 2 / Screen.height),
//            -1, 1
//        );

//        verticalInput = Mathf.Abs(verticalInput) >= 0.2f ? verticalInput : 0;

//        thrustInput = Input.GetAxis("Vertical");

//        rollInput = 0;

//        if (Input.GetKey(KeyCode.Q))
//        {
//            rollInput = -1;
//            rolling = true;
//        }
//        else if (Input.GetKey(KeyCode.E))
//        {
//            rollInput = 1;
//            rolling = true;
//        }
//        else
//        {
//            rolling = false;
//        }

//        UpdateEngineSound();
//    }

//    void FixedUpdate()
//    {
//        // ACCELERATE
//        if (thrustInput > 0)
//        {
//            currentSpeed += acceleration * thrustInput * Time.fixedDeltaTime;
//        }

//        // DECELERATE
//        if (thrustInput < 0)
//        {
//            currentSpeed += deceleration * thrustInput * Time.fixedDeltaTime;
//        }

//        // NATURAL DRAG
//        if (thrustInput == 0)
//        {
//            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * 0.5f, Time.fixedDeltaTime);
//        }

//        // PITCH DRAG (climbing slows plane)
//        if (verticalInput > 0)
//        {
//            currentSpeed -= pitchDrag * verticalInput;
//        }

//        currentSpeed = Mathf.Clamp(currentSpeed, 10f, maxSpeed);

//        // CONTROL LOSS AT LOW SPEED
//        float controlMultiplier = 1f;

//        if (currentSpeed < stallSpeed)
//        {
//            controlMultiplier = 0.4f;
//        }

//        // ROTATION
//        Quaternion rotation = Quaternion.Euler(
//            -verticalInput * rotateSpeed * controlMultiplier * Time.fixedDeltaTime,
//            horizontalInput * rotateSpeed * controlMultiplier * Time.fixedDeltaTime,
//            -rollInput * rollSpeed * controlMultiplier * Time.fixedDeltaTime
//        );

//        rb.MoveRotation(rb.rotation * rotation);

//        // FORWARD MOVEMENT
//        rb.linearVelocity = transform.forward * currentSpeed;

//        // STALL FALL
//        if (currentSpeed < stallSpeed)
//        {
//            rb.linearVelocity += Vector3.down * stallFallSpeed * Time.fixedDeltaTime;
//        }
//    }

//    void UpdateEngineSound()
//    {
//        if (!engineSound) return;

//        float speedPercent = currentSpeed / maxSpeed;

//        // smoother curve
//        float smoothSpeed = Mathf.SmoothStep(0f, 1f, speedPercent);

//        engineSound.volume = Mathf.Lerp(minVolume, maxVolume, smoothSpeed);

//        float pitch = Mathf.Lerp(minPitch, maxPitch, smoothSpeed);

//        // small pitch change when climbing/diving
//        float verticalFactor = rb.linearVelocity.y * 0.01f;

//        engineSound.pitch = Mathf.Clamp(pitch + verticalFactor, minPitch, maxPitch);
//    }

//    public float GetThrottle()
//    {
//        return currentSpeed / maxSpeed;
//    }
//}

using UnityEngine;

public class PhysicsPlaneController : MonoBehaviour
{
    [Header("Flight Settings")]
    [SerializeField] float rotateSpeed = 80f;
    [SerializeField] float rollSpeed = 120f;

    [SerializeField] float maxSpeed = 80f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float deceleration = 15f;

    [SerializeField] float stallSpeed = 25f;
    [SerializeField] float stallFallSpeed = 10f;

    [SerializeField] float pitchDrag = 0.6f;

    [Header("Input Settings")]
    [SerializeField] bool invertPitch = true;

    Rigidbody rb;

    float currentSpeed;

    float horizontalInput; // Yaw
    float verticalInput;   // Pitch
    float thrustInput;     // Throttle
    float rollInput;       // Roll

    public bool rolling;

    [Header("Engine Sound")]
    AudioSource engineSound;

    [SerializeField] float minPitch = 0.8f;
    [SerializeField] float maxPitch = 1.6f;

    [SerializeField] float minVolume = 0.35f;
    [SerializeField] float maxVolume = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        Application.targetFrameRate = 60;

        currentSpeed = maxSpeed * 0.4f;
    }

    void Update()
    {
        // Get input from FlightInputController
        var input = InputManager.Instance;

        if (input == null) return;

        // YAW  horizontal
        horizontalInput = input.Yaw;

        // PITCH  vertical (with invert option)
        verticalInput = input.Pitch * (invertPitch ? -1f : 1f);

        // THROTTLE
        thrustInput = input.Throttle;

        // ROLL (analog now)
        rollInput = input.Roll;
        rolling = Mathf.Abs(rollInput) > 0.1f;

        UpdateEngineSound();
    }

    void FixedUpdate()
    {
        // ACCELERATE
        if (thrustInput > 0)
        {
            currentSpeed += acceleration * thrustInput * Time.fixedDeltaTime;
        }

        // DECELERATE
        if (thrustInput < 0)
        {
            currentSpeed += deceleration * thrustInput * Time.fixedDeltaTime;
        }

        // NATURAL DRAG
        if (Mathf.Abs(thrustInput) < 0.01f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * 0.5f, Time.fixedDeltaTime);
        }

        // PITCH DRAG (climbing slows plane)
        if (verticalInput > 0)
        {
            currentSpeed -= pitchDrag * verticalInput;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 10f, maxSpeed);

        // CONTROL LOSS AT LOW SPEED
        float controlMultiplier = 1f;

        if (currentSpeed < stallSpeed)
        {
            controlMultiplier = 0.4f;
        }

        // ROTATION
        Quaternion rotation = Quaternion.Euler(
            -verticalInput * rotateSpeed * controlMultiplier * Time.fixedDeltaTime,
            horizontalInput * rotateSpeed * controlMultiplier * Time.fixedDeltaTime,
            -rollInput * rollSpeed * controlMultiplier * Time.fixedDeltaTime
        );

        rb.MoveRotation(rb.rotation * rotation);

        // FORWARD MOVEMENT
        rb.linearVelocity = transform.forward * currentSpeed;

        // STALL FALL
        if (currentSpeed < stallSpeed)
        {
            rb.linearVelocity += Vector3.down * stallFallSpeed * Time.fixedDeltaTime;
        }
    }

    void UpdateEngineSound()
    {
        if (!engineSound) return;

        float speedPercent = currentSpeed / maxSpeed;
        float smoothSpeed = Mathf.SmoothStep(0f, 1f, speedPercent);

        engineSound.volume = Mathf.Lerp(minVolume, maxVolume, smoothSpeed);

        float pitch = Mathf.Lerp(minPitch, maxPitch, smoothSpeed);

        float verticalFactor = rb.linearVelocity.y * 0.01f;

        engineSound.pitch = Mathf.Clamp(pitch + verticalFactor, minPitch, maxPitch);
    }

    public float GetThrottle()
    {
        return currentSpeed / maxSpeed;
    }
}