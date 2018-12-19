using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrabbableObject))]
public class LightSwitch : MonoBehaviour
{
    [SerializeField]
    List<LightBulb> m_LigtsToSwitch;

    GrabbableObject m_Grabbable;

    bool m_SwitchState = false;

    private void Start()
    {
    }

    public void Interacted()
    {
        m_SwitchState = !m_SwitchState;
        foreach(LightBulb bulb in m_LigtsToSwitch)
        {
            bulb.SetLight(m_SwitchState);
        }
    }
}
