using UnityEngine;

public class WingVortexController : MonoBehaviour
{
    public ParticleSystem leftVortex;
    public ParticleSystem rightVortex;

    public float baseEmission = 10f;
    public float turnBoost = 80f;

    void Update()
    {
        float turnAmount = Mathf.Abs(Input.GetAxis("Horizontal")) +
                           Mathf.Abs(Input.GetAxis("Vertical"));

        float emission = baseEmission + turnAmount * turnBoost;

        var left = leftVortex.emission;
        var right = rightVortex.emission;

        left.rateOverTime = emission;
        right.rateOverTime = emission;
    }
}
