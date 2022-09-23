using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMRandomState : FSMState
{
    public FSMRandomState(string name, Func<bool> condition, Action action) : base(name, condition, action) { }

    public new bool CheckTransitionToChildren(out FSMState nextState)
    {
        nextState = children[UnityEngine.Random.Range(0, children.Count)];
        return true;
    }

}
