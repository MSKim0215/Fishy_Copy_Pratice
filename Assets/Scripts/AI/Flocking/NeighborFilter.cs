using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NeighborFilter : MonoBehaviour
{
    public abstract List<Transform> Filters(FlockAgent agent, List<Transform> originals);
}