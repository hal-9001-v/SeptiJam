using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMState
{
    public string name;
    protected Action _action;
    protected Func<bool> _condition;

    public List<FSMState> children { get; private set; }

    public FSMState(string name, Func<bool> condition, Action action)
    {
        this.name = name;

        _condition = condition;
        _action = action;

        children = new List<FSMState>();
    }

    public bool CheckTransitionToChildren(out FSMState nextState)
    {
        foreach (FSMState child in children)
        {
            if (child.CheckCondition())
            {
                nextState = child;
                return true;
            }
        }

        nextState = this;

        return false;
    }

    public void Execute()
    {
        if (_action != null)
        {
            _action.Invoke();
        }
    }

    public bool CheckCondition()
    {
        return _condition.Invoke();
    }

}
