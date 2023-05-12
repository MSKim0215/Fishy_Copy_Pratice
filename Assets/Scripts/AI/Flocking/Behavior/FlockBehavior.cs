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
/// ���� ���� ������ �����Ͽ� ��ü �������� ����ϴ� Ŭ����
/// </summary>
public class CompositeBehavior : FlockBehavior
{
    // �ε���, ������
    public Dictionary<int, FlockBehaviorData> BehaviorDatas { private set; get; } = new Dictionary<int, FlockBehaviorData>();

    public void Init(Vector2 flockPos)
    {
        BehaviorDatas.Add((int)FlockBehaviorType.AlignmentBehavior, new FlockBehaviorData(new AlignmentBehavior(), 1f));       // ����
        BehaviorDatas.Add((int)FlockBehaviorType.CohesionBehavior, new FlockBehaviorData(new CohesionBehavior(), 1f));         // ����
        BehaviorDatas.Add((int)FlockBehaviorType.AvoidanceBehavior, new FlockBehaviorData(new AvoidanceBehavior(), 1f));       // ȸ��
        BehaviorDatas.Add((int)FlockBehaviorType.StayInRadiusBehavior, new FlockBehaviorData(new StayInRadiusBehavior(flockPos, 1f), 0.1f));  // �ݰ� ����
    }

    public bool HasBehavior(FlockBehaviorType type)
    {
        if (BehaviorDatas.ContainsKey((int)type)) return true;
        return false;
    }

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        Vector2 move = Vector2.zero;        // �ʱ� ������ ����
        for (int i = 0; i < BehaviorDatas.Count; i++)
        {
            Vector2 partialMove = BehaviorDatas[i].behavior.CalculateMove(agent, neighbors, flock) * BehaviorDatas[i].weight;     // ������ �����Ӱ� ����ġ�� ����
            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > BehaviorDatas[i].weight * BehaviorDatas[i].weight)
                {   // �κ� �������� ũ�Ⱑ ����ġ�� �������� ū ���
                    partialMove.Normalize();                     // �κ� ������ ����ȭ
                    partialMove *= BehaviorDatas[i].weight;      // �κ� �����ӿ� ����ġ�� ���Ͽ� ũ�⸦ ����
                }
                move += partialMove;        // ���� ������ ���Ϳ� �κ� �������� ����
            }
        }
        return move;
    }
}

/// <summary>
/// ��ü�� Ư�� �ݰ� ���� ������Ű�� �ൿ�� ����ϴ� Ŭ����
/// </summary>
public class StayInRadiusBehavior : FlockBehavior
{
    public Vector2 center;      // �����ؾ��� �߽�
    public float radius;        // �����ؾ��� �߽��� ������

    public StayInRadiusBehavior(Vector2 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector2 centerOffset = center - (Vector2)agent.transform.position;      // ��ü�� ���� ��ġ�� �߽� ��ġ ���� ����
        float t = centerOffset.magnitude / radius;     
        if (t < 0.9f)
        {   // ��ü�� �ݰ� ���� �ִ� ����
            return Vector2.zero;
        }
        return centerOffset * t * t;    // ��ü�� �߽� ��ġ�κ��� �־������� �� ū �̵��� ����
    }
}