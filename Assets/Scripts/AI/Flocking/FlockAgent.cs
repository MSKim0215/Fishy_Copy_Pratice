using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    private Flock agentFlock;
    public Flock AgentFlock { get => agentFlock; }

    private Collider2D agentCollider;
    public Collider2D AgentCollider { get => agentCollider; }

    private void Start()
    {
        agentCollider = GetComponent<Collider2D>();
    }

    public void Init(Flock flock) => agentFlock = flock;

    public void Move(Vector2 velocity)
    {
        Vector3 curPos = transform.position;
        Vector3 newPos = curPos + (Vector3)velocity * Time.deltaTime;

        // TODO: 이동방향 설정
        transform.up = velocity.normalized;

        // TODO: 이동속도 설정
        transform.position = newPos;
    }

    private void OnDrawGizmos()
    {
        if (AgentFlock == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AgentFlock.SquareAvoidanceRadius);        // 회피 탐색 범위

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AgentFlock.SquareNeighborRadius);        // 이웃 탐색 범위
    }
}