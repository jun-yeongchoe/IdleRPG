using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private Dictionary<string, UnityAction> eventDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);

            eventDict = new Dictionary<string, UnityAction>();
        }
        else
            Destroy(gameObject);
    }

    public void StartList(string eventName, UnityAction list)
    {
        UnityAction thisEvent;
        if (eventDict.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += list;
            eventDict[eventName] = thisEvent;
        }
        else
        {
            thisEvent += list;
            eventDict.Add(eventName, thisEvent);
        }
    }

    public void StopList(string eventName, UnityAction list)
    {
        if(Instance==null)return;
        UnityAction thisEvent;
        if (eventDict.TryGetValue(eventName, out thisEvent)) 
        { 
            thisEvent -= list;
            eventDict[eventName] = thisEvent;
        }
    }

    public void TriggerEvent(string eventName)
    {
        UnityAction thisEvent;
        if (eventDict.TryGetValue(eventName, out thisEvent))
        { 
            thisEvent.Invoke();
        }
    }
}
