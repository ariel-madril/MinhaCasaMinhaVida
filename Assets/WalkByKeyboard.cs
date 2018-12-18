using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkByKeyboard : MonoBehaviour
{
    [SerializeField]
    float m_Velocity = 1;

    [SerializeField]
    Transform m_TransformToUse;

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

        if(Input.GetKey(KeyCode.Space))
        {
            m_Fly = !m_Fly;
            y = transform.position.y;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            vel *= 3;
        }
        if(Input.GetKey(KeyCode.W))
        {
            transform.position += m_TransformToUse.transform.forward * vel;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += m_TransformToUse.transform.right * vel;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += -m_TransformToUse.transform.right * vel;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -m_TransformToUse.transform.forward * vel;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(transform.up, 1f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(transform.up, -1f);
        }

        if(!m_Fly)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}
