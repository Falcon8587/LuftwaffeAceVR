using UnityEngine;

public class PlaneCameraFOV : MonoBehaviour
{
    public PhysicsPlaneController plane;

    public float minFOV = 65f;
    public float maxFOV = 85f;

    public float smoothSpeed = 3f;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        float throttle = plane.GetThrottle();

        float targetFOV = Mathf.Lerp(minFOV, maxFOV, throttle);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }
}