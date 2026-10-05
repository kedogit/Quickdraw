using System;
using System.Collections.Generic;
using UnityEngine;

public enum EVENT
{
    ON_ENEMY_HURT,
    ON_PLAYER_HURT,
    ON_PLAYER_EXIT_CRITICAL,
    ON_PLAYER_DEATH,
    ON_SENS_CHANGE,
    ON_VOLUME_CHANGE,
    ON_LEVEL_COMPLETE,
    ON_CHEAT_GODMODE,
    ON_CHEAT_INSTAKILL,
    ON_ARENA_START,
    ON_PLAYER_DASH_START
}

public enum GAME_STATE
{
    NEWGAME,
    SAVEDGAME
}

public class Observer
{
    private GAME_STATE m_currentState;
    public GAME_STATE GameState => m_currentState;

    private static Observer m_instance;

    private Dictionary<EVENT, Action<Dictionary<string, object>>> m_eventList;

    private Observer()
    {
        m_eventList = new Dictionary<EVENT, Action<Dictionary<string, object>>>();
        m_currentState = GAME_STATE.NEWGAME;
    }

    public static Observer GetInstance()
    {
        if (m_instance == null)
        {
            m_instance = new Observer();
        }
        return m_instance;
    }

    public void SubscribeTo(EVENT eventName, Action<Dictionary<string, object>> function)
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

    public void SubscribeTo(EVENT eventName, Action function)
    {
        SubscribeTo(eventName, ConvertToParamEvent(function));
    }

    public void UnsubscribeTo(EVENT eventName, Action<Dictionary<string, object>> function)
    {
        if (m_eventList.ContainsKey(eventName))
        {
            m_eventList[eventName] -= function;
        }
    }

    public void UnsubscribeTo(EVENT eventName, Action function)
    {
        UnsubscribeTo(eventName, ConvertToParamEvent(function));
    }

    public void TriggerEvent(EVENT eventName)
    {
        if (m_eventList.ContainsKey(eventName))
        {
            m_eventList[eventName].Invoke(null);
        }
    }

    public void TriggerEvent(EVENT eventName, Dictionary<string, object> eventParams)
    {
        if (m_eventList.ContainsKey(eventName))
        {
            m_eventList[eventName].Invoke(eventParams);
        }
    }

    public void SetGameState(GAME_STATE newState)
    {
        if (newState == GAME_STATE.NEWGAME)
        {
            PlayerPrefs.SetInt("CheckpointIndex", 0);
        }
        m_currentState = newState;
    }

    private Action<Dictionary<string, object>> ConvertToParamEvent(Action action)
    {
        Action<Dictionary<string, object>> parameterlessAction = (dict) => action();
        return parameterlessAction;
    }
}
