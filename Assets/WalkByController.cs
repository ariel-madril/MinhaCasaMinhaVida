using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkByController : MonoBehaviour
{
    [SerializeField]
    float m_Velocity = 1;

    [SerializeField]
    Transform m_TransformToUse;

    [SerializeField]
    bool m_Teleport;

    bool m_Fly = false;

    float y = 0;
    // Start is called before the first frame update
    void Start()
    {
        y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float vel = m_Velocity;

        /*if(Input.GetKeyDown("LeftTrackpadPress") || Input.GetKeyDown("RightTrackpadPress"))
        {
            m_Fly = !m_Fly;
            y = transform.position.y;
        }

        if(Input.GetKey("LeftTrackpadPress") || Input.GetKey("RightTrackpadPress"))
        {
            vel *= 3;
        }*/

        if(!m_Teleport)
        {
            float axisValue = Input.GetAxis("LeftThumbstickVertical") + Input.GetAxis("RightThumbstickVertical");
            //Debug.Log(axisValue);
            transform.position += (m_TransformToUse.transform.forward * vel) * axisValue;

            axisValue = Input.GetAxis("LeftThumbstickHorizontal");
            transform.position += (m_TransformToUse.transform.right * vel) * axisValue;
        }

        transform.Rotate(transform.up, 1f * Input.GetAxis("RightThumbstickHorizontal"));

        if(!m_Fly)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}
