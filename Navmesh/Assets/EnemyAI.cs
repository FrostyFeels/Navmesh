using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PathFinding,
        CoverSeeking,
        foundCover,
        attack,
        Shooting,
        attacked,
        ThrowingGrenade,
        Swarming
    }

    public State state;
    public virtual void Start()
    {
        state = State.PathFinding;
    }

    public virtual void Update()
    {
        
    }
}
