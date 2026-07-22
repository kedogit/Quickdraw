using System;
using System.Collections.Generic;
using UnityEngine;

public enum EVENT
{
    ON_ENEMY_HURT,
    ON_PLAYER_DEATH,
    ON_SENS_CHANGE,
    ON_VOLUME_CHANGE,
    ON_LEVEL_COMPLETE
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

    private Dictionary<EVENT, Action> m_eventList;

    private Observer()
    {
        m_eventList = new Dictionary<EVENT, Action>();
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

    public void SetGameState(GAME_STATE newState)
    {
        if (newState == GAME_STATE.NEWGAME)
        {
            PlayerPrefs.SetInt("CheckpointIndex", 0);
        }
        m_currentState = newState;
    }
}
