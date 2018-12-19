using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DaynightCycle : MonoBehaviour
{
    public static DaynightCycle instance;

    public float m_DayDuration;

    float m_CurrentTime;

    public Action DayStarted;

    public Action NightStarted;

    public UnityEvent DayStartedEvent;

    public UnityEvent NightStartedEvent;

    bool m_AnnouncedNight = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        m_CurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrentTime += Time.deltaTime;

        if(!m_AnnouncedNight && m_CurrentTime > (m_DayDuration/2))
        {
            Debug.Log("NightStarted");
            m_AnnouncedNight = true;
            if(NightStarted != null)
            {
                NightStarted.Invoke();
            }

            if(NightStartedEvent != null)
            {
                NightStartedEvent.Invoke();
            }
        }

        if(m_CurrentTime > m_DayDuration)
        {
            Debug.Log("DayStarted");
            m_CurrentTime = 0;
            m_AnnouncedNight = false;
            if (DayStarted != null)
            {
                DayStarted.Invoke();
            }

            if (DayStartedEvent != null)
            {
                DayStartedEvent.Invoke();
            }
        }
    }

    public float DayTimeRatio()
    {
        return m_CurrentTime / m_DayDuration;
    }
}
