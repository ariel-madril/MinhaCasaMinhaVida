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

    private Transform m_MainCamera;

    float m_ResizeDistanceReference;

    Transform m_LastParent;


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

        m_ResizerPivot.rotation = m_MainCamera.rotation;

        m_ResizerPivot.position = m_ResizableObjectBody.worldCenterOfMass;

        m_ResizerPivot.right = Vector3.Cross(m_ResizerPivot.forward, Vector3.up);

        m_ResizeDistanceReference = Vector3.Distance(m_ResizerAnchor1.position, m_ResizerAnchor2.position);

        m_LastParent = m_ResizableObject.parent;

        m_ResizerPivot.position = (m_ResizerAnchor1.position + m_ResizerAnchor2.position) / 2;

        float distanceToObj = Vector3.Distance(m_ResizableObjectBody.worldCenterOfMass, m_ResizerPivot.position);

        m_ResizerPivot.position += m_ResizerPivot.forward * distanceToObj * -1;

        m_ResizableObject.parent = m_ResizerPivot;

        m_ResizerPivot.parent = m_LastParent;
    }

    public void ExitResizeMode()
    {
        if(m_ResizableObject != null)
        {
            m_ResizableObject.parent = m_LastParent;
        }

        m_ResizerPivot.parent = null;

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

        float currentDistance = Vector3.Distance(m_ResizerAnchor1.position, m_ResizerAnchor2.position);

        float distanceDelta = currentDistance - m_ResizeDistanceReference;

        if(Mathf.Abs(distanceDelta) < 0.05f)
        {
            distanceDelta = 0;
        }

        distanceDelta *= 3;

        distanceDelta = Mathf.Min(distanceDelta, 0.01f);
        distanceDelta = Mathf.Max(distanceDelta, -0.01f);

        Vector3 currentScale = m_ResizerPivot.localScale;

        m_ResizerPivot.localScale = new Vector3(currentScale.x + distanceDelta, currentScale.y + distanceDelta, currentScale.z + distanceDelta);

    }

    public bool IsResizing()
    {
        return m_ResizableObject != null;
    }
}
