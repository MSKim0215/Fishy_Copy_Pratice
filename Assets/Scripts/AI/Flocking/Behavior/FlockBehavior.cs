using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehavior
{
    public abstract Vector2 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock);
}