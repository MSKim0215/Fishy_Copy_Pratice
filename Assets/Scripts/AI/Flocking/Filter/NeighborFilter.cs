using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NeighborFilter
{
    public abstract List<Transform> Filters(FlockAgent agent, List<Transform> originals);
}

/// <summary>
/// ������ ������ ���ϴ� �̿����� ���͸��ϴ� Ŭ����
/// </summary>
public class SameFlockFilter : NeighborFilter
{
    public override List<Transform> Filters(FlockAgent agent, List<Transform> originals)
    {
        List<Transform> filtered = new List<Transform>();       // ���͸��� �̿� ���
        foreach (Transform original in originals)
        {
            FlockAgent originalAgent = original.GetComponent<FlockAgent>();
            if (originalAgent != null && originalAgent.AgentFlock == agent.AgentFlock)
            {   // �ش� �̿��� ������ �ڽ��� ������ ���� ���
                filtered.Add(original);
            }
        }
        return filtered;
    }
}