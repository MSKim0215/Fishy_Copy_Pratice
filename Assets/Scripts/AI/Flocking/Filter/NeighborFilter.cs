using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NeighborFilter
{
    public abstract List<Transform> Filters(FlockAgent agent, List<Transform> originals);
}

/// <summary>
/// 동일한 군집에 속하는 이웃들을 필터링하는 클래스
/// </summary>
public class SameFlockFilter : NeighborFilter
{
    public override List<Transform> Filters(FlockAgent agent, List<Transform> originals)
    {
        List<Transform> filtered = new List<Transform>();       // 필터링된 이웃 목록
        foreach (Transform original in originals)
        {
            FlockAgent originalAgent = original.GetComponent<FlockAgent>();
            if (originalAgent != null && originalAgent.AgentFlock == agent.AgentFlock)
            {   // 해당 이웃의 군집과 자신의 군집이 같을 경우
                filtered.Add(original);
            }
        }
        return filtered;
    }
}