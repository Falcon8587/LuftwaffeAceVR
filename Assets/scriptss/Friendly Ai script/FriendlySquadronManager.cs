using UnityEngine;

public class FriendlySquadronManager : MonoBehaviour
{
    public FriendlyArcadeAI[] wingmen;

    public float spacing = 80f;

    Transform player;

    enum Mode
    {
        Formation,
        Attack,
        Protect
    }

    Mode currentMode = Mode.Formation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.J))
            currentMode = Mode.Formation;

        if (Input.GetKeyDown(KeyCode.K))
            currentMode = Mode.Attack;

        if (Input.GetKeyDown(KeyCode.L))
            currentMode = Mode.Protect;

        if (currentMode == Mode.Formation)
            UpdateLineFormation();

        if (currentMode == Mode.Attack)
            AssignAttackTargets();

        if (currentMode == Mode.Protect)
            SetProtect();
    }

    void UpdateLineFormation()
    {
        Vector3 right = player.right;

        // move formation line slightly forward
        Vector3 center = player.position + player.forward * 80f;

        for (int i = 0; i < wingmen.Length; i++)
        {
            float offset = (i - (wingmen.Length - 1) / 2f) * spacing;

            Vector3 slot = center + right * offset;

            wingmen[i].formationPosition = slot;
            wingmen[i].currentState = FriendlyArcadeAI.AIState.InFormation;
        }
    }

    void SetProtect()
    {
        foreach (var w in wingmen)
            w.currentState = FriendlyArcadeAI.AIState.Protecting;
    }

    void AssignAttackTargets()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0) return;

        for (int i = 0; i < wingmen.Length; i++)
        {
            int enemyIndex = i % enemies.Length;

            wingmen[i].SetTarget(enemies[enemyIndex].transform);
            wingmen[i].currentState = FriendlyArcadeAI.AIState.Attacking;
        }
    }
}