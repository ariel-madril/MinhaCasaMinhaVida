using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
public class LightSwitch : MonoBehaviour
{
    [SerializeField]
    List<LightBulb> m_LigtsToSwitch;

    Grabbable m_Grabbable;

    bool m_SwitchState = false;

    private void Start()
    {
        m_Grabbable = GetComponent<Grabbable>();
        m_Grabbable.GrabbedEvents.AddListener(Interacted);
    }

    void Interacted()
    {
        m_SwitchState = !m_SwitchState;
        foreach(LightBulb bulb in m_LigtsToSwitch)
        {
            bulb.SetLight(m_SwitchState);
        }
    }
}
