using UnityEngine;
using TMPro;

public class PlaneSelector : MonoBehaviour
{
    public TMP_Text planeName;
    public TMP_Text planeStats;
    public TMP_Text planeHistory;

    int currentPlane = 0;

    string[] planes =
    {
        "JU-87 Stuka",
        "BF-109 Fighter"
    };

    string[] stats =
    {
        "Speed: Medium\nArmor: Heavy\nFirepower: High",
        "Speed: High\nArmor: Medium\nFirepower: Medium"
    };

    string[] history =
    {
        "The JU-87 was Germany's famous dive bomber used for precision strikes.",
        "The BF-109 was one of the most successful fighter aircraft of WWII."
    };

    void Start()
    {
        UpdatePlane();
    }

    public void NextPlane()
    {
        currentPlane++;

        if (currentPlane >= planes.Length)
            currentPlane = 0;

        UpdatePlane();
    }

    public void PreviousPlane()
    {
        currentPlane--;

        if (currentPlane < 0)
            currentPlane = planes.Length - 1;

        UpdatePlane();
    }

    void UpdatePlane()
    {
        planeName.text = planes[currentPlane];
        planeStats.text = stats[currentPlane];
        planeHistory.text = history[currentPlane];

        GameManager.instance.selectedPlane = currentPlane;
    }
}