using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;      // ��ü�� ������
    public FlockBehavior behavior;      // ��ü�� ������

    [Header("���� �ɼ�")]
    [Range(3, 12)] public int startingCount = 5;                // ��ü �ʱ� ����
    [Range(1f, 100f)] public float moveSpeed = 50f;             // ��ü �̵��ӵ�
    [Range(1f, 100f)] public float moveSpeedMax = 50f;          // ��ü �ִ� �̵��ӵ�
    [Range(1f, 10f)] public float findNeighborRadius = 1.5f;    // ��ü �̿� Ž�� �ݰ�
    [Range(0f, 1f)] public float avoidanceRadius = 0.5f;        // ��ü ȸ�� �ݰ�

    private List<FlockAgent> agents = new List<FlockAgent>();   // ��ü �迭
    private const float AgentDensity = 0.1f;                    // ��ü �е�

    // ���� ���� ������ ����Ű�� ���� ���� ���� ����
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
            Vector2 flockPos = transform.position;                                          // ���� ��ġ
            Vector2 randomOffset = Random.insideUnitCircle * startingCount * AgentDensity;  // ������ �������� ���
            Vector2 agentPos = flockPos + randomOffset;                                     // ���� ��ġ

            // ���� ���� ���� ��ü �ʱ� ������ �е��� ���Ͽ� ��ġ�� ������ ����.
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
            {   // �ڱ� �ڽ��� �̿� ��ü�� �ν����� �ʵ��� ����
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