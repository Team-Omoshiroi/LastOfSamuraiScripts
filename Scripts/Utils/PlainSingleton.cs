using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainSingleton<T> where T : class, new()
{
    // Check to see if we're about to be destroyed
    private static bool m_ShuttingDown = false;
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                return null;
            }

            if (m_Instance == null)
            {
                m_Instance = new T();
                // Unity raises this event(=> Application.quitting) when the Player application is quitting.
                Application.quitting += () => Destory();
            }
            return m_Instance;
        }
    }

    private static void Destory()
    {
        m_ShuttingDown = true;
        m_Instance = null;
    }
}
