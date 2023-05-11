using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;      // 개체의 프리팹
    public FlockBehavior behavior;      // 개체의 움직임

    [Header("군집 옵션")]
    [Range(3, 12)] public int startingCount = 5;                // 개체 초기 개수
    [Range(1f, 100f)] public float moveSpeed = 50f;             // 개체 이동속도
    [Range(1f, 100f)] public float moveSpeedMax = 50f;          // 개체 최대 이동속도
    [Range(1f, 10f)] public float findNeighborRadius = 1.5f;    // 개체 이웃 탐색 반경
    [Range(0f, 1f)] public float avoidanceRadius = 0.5f;        // 개체 회피 반경

    private List<FlockAgent> agents = new List<FlockAgent>();   // 개체 배열
    private const float AgentDensity = 0.1f;                    // 개체 밀도

    // 벡터 연산 성능을 향상시키기 위해 제곱 값을 저장
    private float squareMoveSpeedMax;
    private float squareNeighborRadius;
    private float squareAvoidanceRadius;

    public float SquareNeighborRadius { get => squareNeighborRadius; }
    public float SquareAvoidanceRadius { get => squareAvoidanceRadius; }

    private void Start()
    {
        behavior = new AvoidanceBehavior();

        squareMoveSpeedMax = moveSpeedMax * moveSpeedMax;
        squareNeighborRadius = findNeighborRadius * findNeighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadius * avoidanceRadius;

        for (int i = 0; i < startingCount; i++)
        {
            Vector2 flockPos = transform.position;                                          // 군집 위치
            Vector2 randomOffset = Random.insideUnitCircle * startingCount * AgentDensity;  // 무작위 오프셋을 계산
            Vector2 agentPos = flockPos + randomOffset;                                     // 최종 위치

            // 일정 범위 내의 개체 초기 개수와 밀도를 곱하여 위치의 분포를 제어.
            FlockAgent agent = Instantiate(agentPrefab, agentPos, Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)), transform);
            agent.name = $"{agentPrefab.name} {i}";
            agent.Init(this);
            agents.Add(agent);
        }
    }

    private void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> neighbors = GetNearbyObjects(agent);
            Vector2 moveVec = behavior.CalculateMove(agent, neighbors, this);
            moveVec *= moveSpeed;

            if (moveVec.sqrMagnitude > squareMoveSpeedMax)
            {
                moveVec = moveVec.normalized * moveSpeedMax;
            }
            agent.Move(moveVec);
        }
    }

    private List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> neighbors = new List<Transform>();
        Collider2D[] neighborColliders = Physics2D.OverlapCircleAll(agent.transform.position, findNeighborRadius);
        foreach(Collider2D neighborCollider in neighborColliders)
        {
            if(neighborCollider != agent.AgentCollider)
            {   // 자기 자신을 이웃 개체로 인식하지 않도록 제외
                neighbors.Add(neighborCollider.transform);
            }
        }
        return neighbors;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, startingCount * AgentDensity);
    }
}