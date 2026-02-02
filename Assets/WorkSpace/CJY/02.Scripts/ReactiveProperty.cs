using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReactiveProperty<T> where T : struct
{
    private T value;

    private Action<T> actions;

    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            if(Equals(this.value,value))
            {
                return;
            }
            this.value = value;
            actions?.Invoke(this.value);
        }
    }

    public void AddAction(Action<T> action)
    {
        this.actions += action;
    }

    // 수정 필요
    public void RemoveAction(Action<T> action)
    {
        this.actions -= action;
    }

    private void OnDestroy()
    {
        actions = null;
    }

    public ReactiveProperty(T init = default)
    {
        this.value = init;
    }
}