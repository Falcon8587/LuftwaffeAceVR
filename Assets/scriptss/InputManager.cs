using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // PlayerInput
    private PlayerInput input;

    // Input Actions
    private InputAction pitchAction;
    private InputAction rollAction;
    private InputAction yawAction;
    private InputAction throttleAction;

    private InputAction gunAction;
    private InputAction bombAction;
    private InputAction formationAAction;
    private InputAction formationBAction;
    private InputAction formationCAction;

    // Events
    public System.Action OnGunPressed;
    public System.Action OnBombPressed;
    public System.Action OnFormationAPressed;
    public System.Action OnFormationBPressed;
    public System.Action OnFormationCPressed;

    // Axis values
    public float Pitch { get; private set; }
    public float Roll { get; private set; }
    public float Yaw { get; private set; }
    public float Throttle { get; private set; }

    // Button states
    public bool GunHeld { get; private set; }
    public bool BombHeld { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        var actions = input.actions;

        pitchAction = actions["Pitch"];
        rollAction = actions["Roll"];
        yawAction = actions["Yaw"];
        throttleAction = actions["Throttle"];

        gunAction = actions["Gun"];
        bombAction = actions["Bomb"];
        formationAAction = actions["FormationA"];
        formationBAction = actions["FormationB"];
        formationCAction = actions["FormationC"];

        // Axis
        pitchAction.performed += OnPitch;
        pitchAction.canceled += OnPitch;

        rollAction.performed += OnRoll;
        rollAction.canceled += OnRoll;

        yawAction.performed += OnYaw;
        yawAction.canceled += OnYaw;

        throttleAction.performed += OnThrottle;
        throttleAction.canceled += OnThrottle;

        // Buttons
        gunAction.performed += OnGun;
        gunAction.canceled += OnGun;

        bombAction.performed += OnBomb;
        bombAction.canceled += OnBomb;

        formationAAction.performed += OnFormationA;
        formationBAction.performed += OnFormationB;
        formationCAction.performed += OnFormationC;
    }

    private void OnDisable()
    {
        pitchAction.performed -= OnPitch;
        pitchAction.canceled -= OnPitch;

        rollAction.performed -= OnRoll;
        rollAction.canceled -= OnRoll;

        yawAction.performed -= OnYaw;
        yawAction.canceled -= OnYaw;

        throttleAction.performed -= OnThrottle;
        throttleAction.canceled -= OnThrottle;

        gunAction.performed -= OnGun;
        gunAction.canceled -= OnGun;

        bombAction.performed -= OnBomb;
        bombAction.canceled -= OnBomb;

        formationAAction.performed -= OnFormationA;
        formationBAction.performed -= OnFormationB;
        formationCAction.performed -= OnFormationC;
    }

    // ===== Axis =====

    private void OnPitch(InputAction.CallbackContext ctx)
    {
        Pitch = ctx.ReadValue<float>();
        //Debug.Log("Pitch: " + Pitch);
    }

    private void OnRoll(InputAction.CallbackContext ctx)
    {
        Roll = ctx.ReadValue<float>();
        //Debug.Log("Roll: " + Roll);
    }

    private void OnYaw(InputAction.CallbackContext ctx)
    {
        Yaw = ctx.ReadValue<float>();
        //Debug.Log("Yaw: " + Yaw);
    }

    private void OnThrottle(InputAction.CallbackContext ctx)
    {
        Throttle = ctx.ReadValue<float>();
        //Debug.Log("Throttle: " + Throttle);
    }

    // ===== Buttons =====

    private void OnGun(InputAction.CallbackContext ctx)
    {
        GunHeld = ctx.ReadValue<float>() > 0.1f;

        if (ctx.performed)
        {
            //Debug.Log("Gun Pressed");
            OnGunPressed?.Invoke();
        }

        if (ctx.canceled)
        {
            //Debug.Log("Gun Released");
        }
    }

    private void OnBomb(InputAction.CallbackContext ctx)
    {
        BombHeld = ctx.ReadValue<float>() > 0.1f;

        if (ctx.performed)
        {
            Debug.Log("Bomb Pressed");
            OnBombPressed?.Invoke();
        }

        if (ctx.canceled)
        {
            Debug.Log("Bomb Released");
        }
    }

    private void OnFormationA(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Formation A Triggered");
            OnFormationAPressed?.Invoke();
        }
    }

    private void OnFormationB(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Formation B Triggered");
            OnFormationBPressed?.Invoke();
        }
    }

    private void OnFormationC(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Formation C Triggered");
            OnFormationCPressed?.Invoke();
        }
    }
}