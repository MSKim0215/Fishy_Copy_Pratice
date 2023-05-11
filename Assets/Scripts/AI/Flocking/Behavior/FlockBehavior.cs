using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehavior
{
    public abstract Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock);
}

/// <summary>
/// ���� ���� ������ �����Ͽ� ��ü �������� ����ϴ� Ŭ����
/// </summary>
public class CompositeBehavior : FlockBehavior
{
    public FlockBehavior[] behaviors;       // ���� �迭
    public float[] weights;                 // ���� ����ġ

    public void Init()
    {
        behaviors = new FlockBehavior[3];
        behaviors[0] = new AlignmentBehavior();     // ����
        behaviors[1] = new CohesionBehavior();      // ����
        behaviors[2] = new AvoidanceBehavior();     // ȸ��

        weights = new float[3];
        weights[0] = 1;
        weights[1] = 0.1f;
        weights[2] = 1;
    }

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        if (weights.Length != behaviors.Length)
        {   // ������ ������ ����ġ�� ������ �������� ������ ���� ����
            Debug.LogWarning("������ ���� ����");
            return Vector2.zero;
        }

        Vector2 move = Vector2.zero;        // �ʱ� ������ ����
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector2 partialMove = behaviors[i].CalculateMove(agent, neighbors, flock) * weights[i];     // ������ �����Ӱ� ����ġ�� ����
            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {   // �κ� �������� ũ�Ⱑ ����ġ�� �������� ū ���
                    partialMove.Normalize();        // �κ� ������ ����ȭ
                    partialMove *= weights[i];      // �κ� �����ӿ� ����ġ�� ���Ͽ� ũ�⸦ ����
                }
                move += partialMove;        // ���� ������ ���Ϳ� �κ� �������� ����
            }
        }
        return move;
    }
}