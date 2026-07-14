using System;
using System.Collections.Generic;
using UnityEngine;

public enum EVENT
{
    ON_ENEMY_HURT,
    ON_PLAYER_DEATH
}

public class Observer
{
    private static Observer m_instance;

    private Dictionary<EVENT, Action> m_eventList;

    private Observer()
    {
        m_eventList = new Dictionary<EVENT, Action>();
    }

    public static Observer GetInstance()
    {
        if (m_instance == null)
        {
            m_instance = new Observer();
        }
        return m_instance;
    }

    public void SubscribeTo(EVENT eventName, Action function)
    {
        if (m_eventList.ContainsKey(eventName))
        {
            m_eventList[eventName] += function;
        }
        else
        {
            m_eventList.Add(eventName, function);
        }
    }

    public void UnsubscribeTo(EVENT eventName, Action function)
    {
        if (m_eventList[eventName] != null)
        {
            m_eventList[eventName] -= function;
        }
    }

    public void TriggerEvent(EVENT eventName)
    {
        if (m_eventList[eventName] != null)
        {
            m_eventList[eventName].Invoke();
        }
    }
}
