using UnityEngine;
using TMPro;

public class FlightHUD : MonoBehaviour
{
    public Rigidbody playerRB;

    public TMP_Text speedText;
    public TMP_Text ammoText;
    public TMP_Text objectiveText;

 

    void Update()
    {
        UpdateSpeed();
    }

    void UpdateSpeed()
    {
        float speed = playerRB.linearVelocity.magnitude * 3.6f;
        speedText.text = "Speed: " + speed.ToString("0") + " km/h";
    }
}