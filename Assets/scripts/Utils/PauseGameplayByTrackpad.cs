using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameplayByTrackpad : MonoBehaviour
{
#if UNITY_EDITOR
    float m_LastAxis;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentAxis = Input.GetAxis("RightGripPress");
        if (currentAxis > 0 && m_LastAxis == 0)
        {
            Debug.Break();
        }
        m_LastAxis = currentAxis;
    }
#endif
}
