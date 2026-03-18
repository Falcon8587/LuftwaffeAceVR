using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCam;
    public Camera leftCam;
    public Camera rightCam;

    void Start()
    {
        SetFarClip();
        ActivateMain();
    }

    void SetFarClip()
    {
        mainCam.farClipPlane = 8000f;
        leftCam.farClipPlane = 8000f;
        rightCam.farClipPlane = 8000f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            ActivateLeft();

        if (Input.GetKeyDown(KeyCode.X))
            ActivateRight();

        if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X))
            ActivateMain();
    }

    void ActivateMain()
    {
        mainCam.enabled = true;
        leftCam.enabled = false;
        rightCam.enabled = false;
    }

    void ActivateLeft()
    {
        mainCam.enabled = false;
        leftCam.enabled = true;
        rightCam.enabled = false;
    }

    void ActivateRight()
    {
        mainCam.enabled = false;
        leftCam.enabled = false;
        rightCam.enabled = true;
    }
}