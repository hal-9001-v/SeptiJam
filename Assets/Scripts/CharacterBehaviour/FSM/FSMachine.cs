using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMachine
{
    public FSMState currentState { get; private set; }

    public FSMachine(FSMState startingState, bool executeCondition)
    {
        currentState = startingState;

        if (executeCondition)
        {
            currentState.CheckCondition();
        }
    }

    public void Update()
    {
        FSMState nextState;

        if (currentState.CheckTransitionToChildren(out nextState))
        {
            SetState(nextState);
        }

        currentState.Execute();
    }

    void SetState(FSMState state)
    {
        currentState = state;

        currentState.Execute();
    }

}
