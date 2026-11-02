using UnityEngine;
using System;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T m_instance;

    [SerializeField] private bool m_dontDestroyOnLoad;

    protected virtual void Awake()
    {
        if (m_instance != null)
        {
            Destroy(this.gameObject);

            //wow. i wasted a lot of time on this. NOTE TO SELF: DESTROY CALLS DO NOT IMMEDIATELY DESTROY THE OBJECT.
            //DESTROY() FLAGS THE OBJECT FOR DELETION AT THE ***END OF THE FRAME***, WHICH MEANS THE FUNCTION STILL CONTINUES.
            //hence the need for a return here
            return;
        }

        m_instance = this as T;

        if(m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public static T GetInstance()
    {
        return m_instance;
    }
}
