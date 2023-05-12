using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlockBehaviorType
{
    AlignmentBehavior,
    CohesionBehavior,
    AvoidanceBehavior,
    StayInRadiusBehavior
}

public abstract class FlockBehavior
{
    public abstract Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock);
}

public class FlockBehaviorData
{
    public FlockBehavior behavior;
    public float weight;

    public FlockBehaviorData(FlockBehavior behavior, float weight)
    {
        this.behavior = behavior;
        this.weight = weight;
    }
}

/// <summary>
/// 여러 개의 동작을 조합하여 전체 움직임을 계산하는 클래스
/// </summary>
public class CompositeBehavior : FlockBehavior
{
    // 인덱스, 데이터
    public Dictionary<int, FlockBehaviorData> BehaviorDatas { private set; get; } = new Dictionary<int, FlockBehaviorData>();

    public void Init(Vector2 flockPos)
    {
        BehaviorDatas.Add((int)FlockBehaviorType.AlignmentBehavior, new FlockBehaviorData(new AlignmentBehavior(), 1f));       // 정렬
        BehaviorDatas.Add((int)FlockBehaviorType.CohesionBehavior, new FlockBehaviorData(new CohesionBehavior(), 1f));         // 밀집
        BehaviorDatas.Add((int)FlockBehaviorType.AvoidanceBehavior, new FlockBehaviorData(new AvoidanceBehavior(), 1f));       // 회피
        BehaviorDatas.Add((int)FlockBehaviorType.StayInRadiusBehavior, new FlockBehaviorData(new StayInRadiusBehavior(flockPos, 1f), 0.1f));  // 반경 유지
    }

    public bool HasBehavior(FlockBehaviorType type)
    {
        if (BehaviorDatas.ContainsKey((int)type)) return true;
        return false;
    }

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        Vector2 move = Vector2.zero;        // 초기 움직임 벡터
        for (int i = 0; i < BehaviorDatas.Count; i++)
        {
            Vector2 partialMove = BehaviorDatas[i].behavior.CalculateMove(agent, neighbors, flock) * BehaviorDatas[i].weight;     // 동작의 움직임과 가중치를 곱함
            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > BehaviorDatas[i].weight * BehaviorDatas[i].weight)
                {   // 부분 움직임의 크기가 가중치의 제곱보다 큰 경우
                    partialMove.Normalize();                     // 부분 움직임 정규화
                    partialMove *= BehaviorDatas[i].weight;      // 부분 움직임에 가중치를 곱하여 크기를 조정
                }
                move += partialMove;        // 최종 움직임 벡터에 부분 움직임을 더함
            }
        }
        return move;
    }
}

/// <summary>
/// 개체를 특정 반경 내에 유지시키는 행동을 계산하는 클래스
/// </summary>
public class StayInRadiusBehavior : FlockBehavior
{
    public Vector2 center;      // 유지해야할 중심
    public float radius;        // 유지해야할 중심의 반지름

    public StayInRadiusBehavior(Vector2 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector2 centerOffset = center - (Vector2)agent.transform.position;      // 개체의 현재 위치와 중심 위치 간의 차이
        float t = centerOffset.magnitude / radius;     
        if (t < 0.9f)
        {   // 개체가 반경 내에 있는 상태
            return Vector2.zero;
        }
        return centerOffset * t * t;    // 개체는 중심 위치로부터 멀어질수록 더 큰 이동을 수행
    }
}