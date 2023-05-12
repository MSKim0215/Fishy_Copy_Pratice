using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FilteredFlockBehavior : FlockBehavior
{
    public NeighborFilter filter = new SameFlockFilter();
}

/// <summary>
/// ��ü�� ���� ������ ����ϴ� Ŭ����
/// </summary>
public class AlignmentBehavior : FilteredFlockBehavior
{
    /// <summary>
    /// ��ü�� �ֺ� �̿����� ������ ����Ͽ� ���ĵ� ������ ��ȯ
    /// agent: ��ü (����)
    /// neighbors: ��ü ���� (�̿�)
    /// flock: ����
    /// </summary>
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        // �̿��� ���ٸ� ��ü�� ���� ������ ����
        if (neighbors.Count == 0) return agent.transform.up;

        Vector2 alignmentMove = Vector2.zero;       // ���� �̵� ����
        List<Transform> filteredNeighbors = (filter == null) ? neighbors : filter.Filters(agent, neighbors);    // ���͸� �������� ���͸��� �̿� ��� ȹ��
        foreach (Transform neighbor in filteredNeighbors)
        {   
            alignmentMove += (Vector2)neighbor.transform.up;    // �� �̿� ��ü�� ������ ����
        }
        alignmentMove /= neighbors.Count;       // �̿� ��ü���� ��� ����
        return alignmentMove;
    }
}

/// <summary>
/// ��ü�� ���� ������ ����ϴ� Ŭ����
/// </summary>
public class CohesionBehavior : FilteredFlockBehavior
{
    private Vector2 currentVelocity;        // ���� �ӵ�

    public float agentSmoothTime = 0.5f;    // �ε巯�� �̵��� �����ϴ� �ð� ��

    /// <summary>
    /// ��ü�� �ֺ� �̿����� ��ġ�� ����Ͽ� ��ü�� ���� ��ġ���� ���̸� ����Ͽ� ���� ������ ��ȯ
    /// agent: ��ü (����)
    /// neighbors: ��ü ���� (�̿�)
    /// flock: ����
    /// </summary>
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        // �̿��� ���ٸ� ������ ����
        if (neighbors.Count == 0) return Vector2.zero;

        Vector2 cohesionMove = Vector2.zero;        // ���� �̵� ����
        List<Transform> filteredNeighbors = (filter == null) ? neighbors : filter.Filters(agent, neighbors);      // ���͸� �������� ���͸��� �̿� ��� ȹ��
        foreach (Transform neighbor in filteredNeighbors)
        {
            cohesionMove += (Vector2)neighbor.position;     // �� �̿� ��ü�� ��ġ�� ����
        }
        cohesionMove /= neighbors.Count;                        // ��ü���� �����ؾ� �� ��ǥ ��ġ
        cohesionMove -= (Vector2)agent.transform.position;      // ��ü�� �����ؾ� �� ����

        // ���� �ӵ��� ��ǥ �̵� ������ ������� ���ο� �̵� ���� ���
        cohesionMove = Vector2.SmoothDamp(agent.transform.up, cohesionMove, ref currentVelocity, agentSmoothTime);
        return cohesionMove;
    }
}

/// <summary>
/// ��ü�� ȸ�� ������ ����ϴ� Ŭ����
/// </summary>
public class AvoidanceBehavior : FilteredFlockBehavior
{
    /// <summary>
    /// ��ü�� �ֺ� �̿������ �Ÿ��� Ȯ���Ͽ� ȸ���ؾ� �� �̿� ��ü�� �ĺ��ϰ�, �ش� �̿������ ���� ���̸� �����Ͽ� ȸ�� ������ ��ȯ
    /// agent: ��ü (����)
    /// neighbors: ��ü ���� (�̿�)
    /// flock: ����
    /// </summary>
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        // �̿��� ���ٸ� ������ ����
        if (neighbors.Count == 0) return Vector2.zero;

        Vector2 avoidanceMove = Vector2.zero;       // ȸ�� �̵� ����
        int neighborAvoidCount = 0;                 // ȸ���ϴ� �̿� ��ü�� ��
        List<Transform> filteredNeighbors = (filter == null) ? neighbors : filter.Filters(agent, neighbors);      // ���͸� �������� ���͸��� �̿� ��� ȹ��
        foreach (Transform neighbor in filteredNeighbors)
        {
            if (Vector2.SqrMagnitude(neighbor.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {   // �� �̿� ��ü���� �Ÿ��� ����ϰ�, �ش� �Ÿ��� ��ü ȸ�� �ݰ溸�� ���� ��� ȸ�� ����
                neighborAvoidCount++;
                avoidanceMove += (Vector2)(agent.transform.position - neighbor.position);   // ȸ���ؾ� �� ����
            }
        }

        if (neighborAvoidCount > 0)
        {
            avoidanceMove /= neighborAvoidCount;    // ��ü�� ȸ���ؾ� �� ��� ����
        }
        return avoidanceMove;
    }
}