using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FilteredFlockBehavior : FlockBehavior
{
    public NeighborFilter filter = new SameFlockFilter();
}

/// <summary>
/// 개체의 정렬 동작을 계산하는 클래스
/// </summary>
public class AlignmentBehavior : FilteredFlockBehavior
{
    /// <summary>
    /// 개체의 주변 이웃들의 방향을 평균하여 정렬된 방향을 반환
    /// agent: 개체 (본인)
    /// neighbors: 개체 집합 (이웃)
    /// flock: 군집
    /// </summary>
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        // 이웃이 없다면 개체의 현재 방향을 유지
        if (neighbors.Count == 0) return agent.transform.up;

        Vector2 alignmentMove = Vector2.zero;       // 정렬 이동 벡터
        List<Transform> filteredNeighbors = (filter == null) ? neighbors : filter.Filters(agent, neighbors);    // 필터를 기준으로 필터링된 이웃 목록 획득
        foreach (Transform neighbor in filteredNeighbors)
        {   
            alignmentMove += (Vector2)neighbor.transform.up;    // 각 이웃 개체의 방향을 누적
        }
        alignmentMove /= neighbors.Count;       // 이웃 개체들의 평균 방향
        return alignmentMove;
    }
}

/// <summary>
/// 개체의 응집 동작을 계산하는 클래스
/// </summary>
public class CohesionBehavior : FilteredFlockBehavior
{
    private Vector2 currentVelocity;        // 현재 속도

    public float agentSmoothTime = 0.5f;    // 부드러운 이동을 조절하는 시간 값

    /// <summary>
    /// 개체의 주변 이웃들의 위치를 평균하여 개체의 현재 위치와의 차이를 계산하여 응집 방향을 반환
    /// agent: 개체 (본인)
    /// neighbors: 개체 집합 (이웃)
    /// flock: 군집
    /// </summary>
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        // 이웃이 없다면 움직임 정지
        if (neighbors.Count == 0) return Vector2.zero;

        Vector2 cohesionMove = Vector2.zero;        // 응집 이동 벡터
        List<Transform> filteredNeighbors = (filter == null) ? neighbors : filter.Filters(agent, neighbors);      // 필터를 기준으로 필터링된 이웃 목록 획득
        foreach (Transform neighbor in filteredNeighbors)
        {
            cohesionMove += (Vector2)neighbor.position;     // 각 이웃 개체의 위치를 누적
        }
        cohesionMove /= neighbors.Count;                        // 개체들이 응집해야 할 목표 위치
        cohesionMove -= (Vector2)agent.transform.position;      // 개체가 응집해야 할 방향

        // 현재 속도와 목표 이동 방향을 기반으로 새로운 이동 방향 계산
        cohesionMove = Vector2.SmoothDamp(agent.transform.up, cohesionMove, ref currentVelocity, agentSmoothTime);
        return cohesionMove;
    }
}

/// <summary>
/// 개체의 회피 동작을 계산하는 클래스
/// </summary>
public class AvoidanceBehavior : FilteredFlockBehavior
{
    /// <summary>
    /// 개체의 주변 이웃들과의 거리를 확인하여 회피해야 할 이웃 개체를 식별하고, 해당 이웃들과의 백터 차이를 누적하여 회피 방향을 반환
    /// agent: 개체 (본인)
    /// neighbors: 개체 집합 (이웃)
    /// flock: 군집
    /// </summary>
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        // 이웃이 없다면 움직임 정지
        if (neighbors.Count == 0) return Vector2.zero;

        Vector2 avoidanceMove = Vector2.zero;       // 회피 이동 벡터
        int neighborAvoidCount = 0;                 // 회피하는 이웃 개체의 수
        List<Transform> filteredNeighbors = (filter == null) ? neighbors : filter.Filters(agent, neighbors);      // 필터를 기준으로 필터링된 이웃 목록 획득
        foreach (Transform neighbor in filteredNeighbors)
        {
            if (Vector2.SqrMagnitude(neighbor.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {   // 각 이웃 개체와의 거리를 계산하고, 해당 거리가 개체 회피 반경보다 작은 경우 회피 동작
                neighborAvoidCount++;
                avoidanceMove += (Vector2)(agent.transform.position - neighbor.position);   // 회피해야 할 방향
            }
        }

        if (neighborAvoidCount > 0)
        {
            avoidanceMove /= neighborAvoidCount;    // 개체가 회피해야 할 평균 방향
        }
        return avoidanceMove;
    }
}