using UnityEngine;

public class PlayerPlaneSpawner : MonoBehaviour
{
    public GameObject[] playerPlanes;

    void Start()
    {
        int planeIndex = GameManager.instance.selectedPlane;

        Instantiate(playerPlanes[planeIndex], transform.position, transform.rotation);
    }
}