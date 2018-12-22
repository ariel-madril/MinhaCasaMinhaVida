using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeController : MonoBehaviour
{
    public static ResizeController Instance;

    public List<Grabber> m_HandsReference;

    public Transform m_ResizerAnchor1;

    public Transform m_ResizerAnchor2;

    public Transform m_ResizableObject;

    public Rigidbody m_ResizableObjectBody;

    public Grabber m_ResizingHand;

    Transform m_ResizerPivot;

    float m_LastDistance;

    private Transform m_MainCamera;

    public enum ResizeMode
    {
        LOCAL,
        GLOBAL
    }

    [SerializeField]
    private ResizeMode m_ResizeMode;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        m_HandsReference = new List<Grabber>();
        m_ResizerPivot = new GameObject().transform;
        m_ResizerPivot.name = "ResizerPivot";

        m_MainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_HandsReference.Count < 2)
        {
            return;
        }

        if(m_ResizerAnchor1 == null || m_ResizerAnchor2 == null)
        {
            m_ResizerAnchor1 = m_HandsReference[0].transform;
            m_ResizerAnchor2 = m_HandsReference[1].transform;
        }

        m_ResizerPivot.position = (m_ResizerAnchor1.position + m_ResizerAnchor2.position) / 2;

        if (m_ResizableObject != null)
        {
            PerformResize();
        }
    }

    public void EnterResizeMode(GameObject resizableObject, Grabber resizingHand)
    {
        m_ResizerPivot.localScale = Vector3.one;
        m_ResizableObject = resizableObject.transform;
        m_ResizableObjectBody = m_ResizableObject.GetComponent<Rigidbody>();
        m_ResizingHand = resizingHand;
        m_LastDistance = 0;
    }

    public void ExitResizeMode()
    {
        if(m_ResizableObject != null)
        {
            m_ResizableObject.parent = null;
        }

        m_ResizableObject = null;

        m_ResizableObjectBody = null;

        m_ResizingHand = null;

    }

    void PerformResize()
    {
        if (m_ResizerAnchor1 == null || m_ResizerAnchor2 == null || m_ResizingHand == null)
        {
            return;
        }

        m_ResizerPivot.position = m_ResizableObjectBody.worldCenterOfMass;

        m_ResizerPivot.rotation = m_MainCamera.rotation;

        m_ResizableObject.parent = m_ResizerPivot;

        float currentDistance = Vector3.Distance(m_ResizerAnchor1.position, m_ResizerAnchor2.position);

        if(m_LastDistance == 0)
        {
            m_LastDistance = currentDistance;
        }

        float distanceDelta = currentDistance - m_LastDistance;

        Vector3 currentScale = m_ResizerPivot.localScale;

        switch (m_ResizeMode)
        {
            case ResizeMode.LOCAL:
                m_ResizerPivot.localScale = new Vector3(currentScale.x + distanceDelta, currentScale.y, currentScale.z);
                break;
            case ResizeMode.GLOBAL:
                m_ResizerPivot.localScale = new Vector3(currentScale.x + distanceDelta, currentScale.y + distanceDelta, currentScale.z + distanceDelta);
                break;
            default:
                break;
        }

        m_ResizableObject.parent = null;

        m_LastDistance = currentDistance;
    }
}
