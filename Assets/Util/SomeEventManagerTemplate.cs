using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class SomeEventManagerTemplate
{
    public delegate void SomeAction(int argument);

    public static event SomeAction gameSomeAction;

    public static void TriggerSomeEvent(int argument)
    {
        gameSomeAction?.Invoke(argument);
    }

    public static void OnDestroy()
    {
        Delegate[] clientList = gameSomeAction?.GetInvocationList();

        foreach (Delegate clientAction in clientList)
        {
            gameSomeAction -= (clientAction as SomeAction);
        }
    }
}

/*
public ModelValue(ValueType defaultValue)
{
    SomeEventManagerTemplate.gameSomeAction += TestAction;
    // use `SomeEventManagerTemplate.gameSomeAction -= TestAction;`
    // to unsubscrube
    
    SomeEventManagerTemplate.TriggerSomeEvent(11);
}

void TestAction(int value)
{
    Debug.Log(value);
}
*/