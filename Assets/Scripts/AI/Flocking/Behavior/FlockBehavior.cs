using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehavior
{
    public abstract Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock);
}

/// <summary>
/// 여러 개의 동작을 조합하여 전체 움직임을 계산하는 클래스
/// </summary>
public class CompositeBehavior : FlockBehavior
{
    public FlockBehavior[] behaviors;       // 동작 배열
    public float[] weights;                 // 동작 가중치

    public void Init()
    {
        behaviors = new FlockBehavior[3];
        behaviors[0] = new AlignmentBehavior();     // 정렬
        behaviors[1] = new CohesionBehavior();      // 밀집
        behaviors[2] = new AvoidanceBehavior();     // 회피

        weights = new float[3];
        weights[0] = 1;
        weights[1] = 0.1f;
        weights[2] = 1;
    }

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        if (weights.Length != behaviors.Length)
        {   // 동작의 개수와 가중치의 개수가 동일하지 않으면 정지 상태
            Debug.LogWarning("개수가 맞지 않음");
            return Vector2.zero;
        }

        Vector2 move = Vector2.zero;        // 초기 움직임 벡터
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector2 partialMove = behaviors[i].CalculateMove(agent, neighbors, flock) * weights[i];     // 동작의 움직임과 가중치를 곱함
            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {   // 부분 움직임의 크기가 가중치의 제곱보다 큰 경우
                    partialMove.Normalize();        // 부분 움직임 정규화
                    partialMove *= weights[i];      // 부분 움직임에 가중치를 곱하여 크기를 조정
                }
                move += partialMove;        // 최종 움직임 벡터에 부분 움직임을 더함
            }
        }
        return move;
    }
}