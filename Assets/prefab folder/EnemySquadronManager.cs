using UnityEngine;

public class EnemySquadronManager : MonoBehaviour
{
    public enum FormationType { Line, Box, V_Wide, V_Tight, Diamond }
    public FormationType formationType = FormationType.V_Wide;

    [Header("Planes")]
    public ArcadeEnemyAI[] enemyPlanes;
    public float spacing = 60f;

    [Header("Combat Settings")]
    public float detectionRadius = 400f;
    public float attackDuration = 20f;
    public float retreatDistance = 500f;
    public float regroupDuration = 6f;

    [Header("Leader Settings")]
    public ArcadeEnemyAI leader;   // Assign manually in Inspector
    private enum SquadronState
    {
        FormationChase,
        Attacking,
        Retreating,
        Regrouping
    }

    private SquadronState squadronState = SquadronState.FormationChase;

    private Transform player;
 
    private float stateTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (leader == null)
        {
            Debug.LogError("Leader not assigned in Inspector!");
            return;
        }

        leader.isLeader = true;

        SetAllPlanesState(ArcadeEnemyAI.AIState.InFormation);
    }

    void Update()
    {
        if (player == null) return;

        switch (squadronState)
        {
            case SquadronState.FormationChase:
                AssignFormationPositions();
                MoveLeaderTowardsPlayer();

                float dist = Vector3.Distance(leader.transform.position, player.position);

                if (dist < detectionRadius)
                {
                    squadronState = SquadronState.Attacking;
                    stateTimer = attackDuration;
                    SetAllPlanesState(ArcadeEnemyAI.AIState.Chasing);
                }
                break;

            case SquadronState.Attacking:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    squadronState = SquadronState.Retreating;
                    stateTimer = 5f;
                    SetAllPlanesState(ArcadeEnemyAI.AIState.ReturningToFormation);
                }
                break;

            case SquadronState.Retreating:
                AssignRetreatPositions();
                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    squadronState = SquadronState.Regrouping;
                    stateTimer = regroupDuration;
                }
                break;

            case SquadronState.Regrouping:
                AssignFormationPositions();
                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    squadronState = SquadronState.FormationChase;
                    SetAllPlanesState(ArcadeEnemyAI.AIState.InFormation);
                }
                break;
        }
    }

    void MoveLeaderTowardsPlayer()
    {
        Vector3 dir = (player.position - leader.transform.position).normalized;
        leader.transform.rotation = Quaternion.Slerp(
            leader.transform.rotation,
            Quaternion.LookRotation(dir),
            2f * Time.deltaTime
        );
    }

    void AssignRetreatPositions()
    {
        Vector3 retreatPoint = leader.transform.position - leader.transform.forward * retreatDistance;

        foreach (var plane in enemyPlanes)
        {
            plane.formationPosition = retreatPoint;
        }
    }

    void AssignFormationPositions()
    {
        Vector3 leaderPos = leader.transform.position;
        Vector3 forward = leader.transform.forward;
        Vector3 right = leader.transform.right;

        for (int i = 0; i < enemyPlanes.Length; i++)
        {
            if (enemyPlanes[i] == leader)
            {
                enemyPlanes[i].formationPosition = leaderPos;
                continue;
            }

            Vector3 offset = Vector3.zero;

            switch (formationType)
            {
                case FormationType.Line:
                    {
                        int centerIndex = 3; // 4th plane is leader
                        int relativeIndex = i - centerIndex;

                        offset = right * relativeIndex * spacing;
                        break;
                    }

                case FormationType.Box:
                    int row = i / 3;
                    int col = i % 3;
                    offset = (-forward * row * spacing) + (right * (col - 1) * spacing);
                    break;

                case FormationType.V_Wide:
                    int side = (i % 2 == 0) ? 1 : -1;
                    int rank = (i + 1) / 2;
                    offset = (-forward * rank * spacing) + (right * side * rank * spacing);
                    break;

                case FormationType.V_Tight:
                    side = (i % 2 == 0) ? 1 : -1;
                    rank = (i + 1) / 2;
                    offset = (-forward * rank * spacing * 0.6f) + (right * side * rank * spacing * 0.4f);
                    break;

                case FormationType.Diamond:
                    if (i == 0) offset = -forward * spacing;
                    else if (i == 1) offset = right * spacing;
                    else if (i == 2) offset = -right * spacing;
                    else offset = -forward * spacing * 2;
                    break;
            }

            enemyPlanes[i].formationPosition = leaderPos + offset;
        }
    }

    void SetAllPlanesState(ArcadeEnemyAI.AIState state)
    {
        foreach (var plane in enemyPlanes)
        {
            plane.SetAIState(state);
        }
    }
}
