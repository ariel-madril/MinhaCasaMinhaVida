using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    Transform m_RotationToCopy;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = m_RotationToCopy.rotation;
    }
}
