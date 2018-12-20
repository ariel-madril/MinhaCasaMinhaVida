using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour
{

    LineRenderer m_Line;

    [SerializeField]
    Transform m_Hand;

    [SerializeField]
    Transform m_target;

    // Start is called before the first frame update
    void Start()
    {
        m_Line = GetComponent<LineRenderer>();    
    }

    // Update is called once per frame
    void Update()
    {
        float axisValue = Input.GetAxis("RightThumbstickVertical");

        if(axisValue > 0)
        {
            m_Line.enabled = true;
        }
        else
        {
            m_Line.enabled = false;
        }

        if (m_Line.enabled)
        {
            Ray ray = new Ray(m_Hand.position, m_Hand.forward);
            RaycastHit hit;

            m_Line.SetPosition(0, m_Hand.position);

            if (Physics.Raycast(ray, out hit))
            {
                m_Line.SetPosition(1, hit.point);
                m_target.gameObject.SetActive(true);
                m_target.transform.position = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);
            }
            else
            {

                m_Line.SetPosition(1, m_Hand.position + m_Hand.forward * 10);
            }
        }
    }

    IEnumerator PerformTeleport(Vector3 point)
    {
        yield return null;
    }
}
