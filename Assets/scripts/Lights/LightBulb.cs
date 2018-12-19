using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    Light m_Light;

    List<Material> m_Mats;

    // Start is called before the first frame update
    void Start()
    {
        m_Mats = new List<Material>();

        foreach(Transform child in transform)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            if(renderer != null)
            {
                m_Mats.Add(renderer.material);
            }
        }

        m_Light = gameObject.GetComponentInChildren<Light>();
        TurnLightOff();
        DaynightCycle.instance.DayStarted += DayStarted;
        DaynightCycle.instance.NightStarted += NightStarted;
    }

    void DayStarted()
    {
        TurnLightOff();
    }

    void NightStarted()
    {
        TurnLightOn();
    }

    public void SetLight(bool on)
    {
        if(on)
        {
            TurnLightOn();
        }
        else
        {
            TurnLightOff();
        }
    }

    void TurnLightOn()
    {
        m_Light.enabled = true;
        foreach (Material mat in m_Mats)
        {
            mat.SetColor("_EmissionColor", Color.white);
        }
    }

    void TurnLightOff()
    {
        m_Light.enabled = false;
        foreach(Material mat in m_Mats)
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
}
