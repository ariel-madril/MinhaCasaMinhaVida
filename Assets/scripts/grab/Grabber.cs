using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class Grabber : MonoBehaviour
{
    public enum PlayerHand
    {
        LEFT,
        RIGHT
    }
    bool m_RefreshGrabAmount = true;

    Rigidbody m_Body;

    private static Dictionary<GameObject, GrabbableInformation> m_GrabbableObjectDic;
    public static Dictionary<GameObject, GrabbableInformation> GrabbableObjectsDic
    {
        get
        {
            if (m_GrabbableObjectDic == null)
            {
                m_GrabbableObjectDic = new Dictionary<GameObject, GrabbableInformation>();
            }
            return m_GrabbableObjectDic;
        }
    }

    private static Dictionary<GameObject, GrabbableInformation> m_GrabbableUIDic;
    public static Dictionary<GameObject, GrabbableInformation> GrabbableUIDic
    {
        get
        {
            if (m_GrabbableUIDic == null)
            {
                m_GrabbableUIDic = new Dictionary<GameObject, GrabbableInformation>();
            }
            return m_GrabbableUIDic;
        }
    }

    GrabbableInformation m_CurrentGrabbedObject;

    [SerializeField]
    GameObject m_Anchor;

    TrackedPoseDriver m_TrackDriver;
    public GameObject Anchor
    {
        get
        {
            return m_Anchor;
        }
    }

    [SerializeField]
    GameObject m_SnapPosition;

    public GameObject SnapPosition
    {
        get
        {
            return m_SnapPosition;
        }
    }

    private GameObject m_HandModel;

    [HideInInspector]
    public Animator m_Animator;

    [SerializeField]
    PlayerHand m_PlayerHand;

    [SerializeField]
    bool m_PotatoPresence = true;

    public PlayerHand Hand
    {
        get
        {
            return m_PlayerHand;
        }
    }

    List<GameObject> m_GrabbableObjectsCandidates;
    public List<GameObject> m_GrabbableUICandidates;

    new Transform transform;

    FixedJoint m_Joint;

    public static void AddGrabbableObjectReference(Grabbable grabbable)
    {
        GrabbableObjectsDic.Add(grabbable.gameObject, new GrabbableInformation(grabbable.GetComponent<Rigidbody>(), grabbable as GrabbableObject));
    }

    public static void RemoveGrabbableObjectReference(Grabbable grabbable)
    {
        GrabbableObjectsDic.Remove(grabbable.gameObject);
    }

    private void Awake()
    {
        SetHandModel();
        m_Body = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start()
    {
        transform = base.transform;

        m_GrabbableObjectsCandidates = new List<GameObject>();
        m_GrabbableUICandidates = new List<GameObject>();

        m_Joint = GetComponent<FixedJoint>();
    }

    public void SetHandModel()
    {
        if(m_Animator == null)
        {
            m_Animator = gameObject.GetComponent<Animator>();

            if(m_Animator == null)
            {
                m_Animator = gameObject.GetComponentInChildren<Animator>();
            }

            if(m_Animator != null)
            {
                m_RefreshGrabAmount = ContainsParam("GrabAmount");
            }
        }

        if(m_HandModel != null)
        {
            return;
        }

        SkinnedMeshRenderer rendererObject = GetComponent<SkinnedMeshRenderer>();
        if(rendererObject == null)
        {
            rendererObject = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        
        if(rendererObject != null)
        {
            m_HandModel = rendererObject.gameObject;
        }
        else
        {
            m_HandModel = gameObject;
        }
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        string button = m_PlayerHand == PlayerHand.LEFT ? "LeftTriggerTouch" : "RightTriggerTouch";

        if (m_CurrentGrabbedObject == null && Input.GetButtonDown(button))
        {
            HandleGrab();
        }

        if (m_CurrentGrabbedObject != null && Input.GetButtonUp(button))
        {
            HandleRelease();
        }

        UpdateAnimator();
	}

    public bool ContainsParam(string parameter)
    {
        foreach (AnimatorControllerParameter param in m_Animator.parameters)
        {
            if (param.name == parameter)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateAnimator()
    {
        if(!m_RefreshGrabAmount)
        {
            return;
        }

        string axis = m_PlayerHand == PlayerHand.LEFT ? "LeftTriggerSqueeze" : "RightTriggerSqueeze";
        m_Animator.SetFloat("GrabAmount", Input.GetAxis(axis));
    }

    public void GrabbedObjectTakenAway()
    {
        m_CurrentGrabbedObject = null;
        m_Joint.connectedBody = null;

        if (m_PotatoPresence)
        {
            m_HandModel.SetActive(true);
        }
    }

    public void GrabObject(GameObject objectToGrab)
    {
        GrabbedObjectTakenAway();
        PerformGrab(objectToGrab);
    }

    void HandleGrab()
    {
        GameObject grabbable = GetClosestGrabbableObject();

        if(grabbable != null)
        {
            PerformGrab(grabbable);
        }
    }

    void PerformGrab(GameObject grabbable)
    {
        GrabbableInformation grabbableObj;
        if (GrabbableObjectsDic.TryGetValue(grabbable, out grabbableObj))
        {
            grabbableObj.m_GrabbableObject.ObjectGrabbed(this);
            m_CurrentGrabbedObject = grabbableObj;
            m_Joint.connectedBody = grabbableObj.m_Body;

            if (m_PotatoPresence)
            {
                m_HandModel.SetActive(false);
            }
        }
    }

    void HandleRelease()
    {
        if (m_CurrentGrabbedObject != null)
        {
            m_CurrentGrabbedObject.m_GrabbableObject.ObjectReleased();
            m_Joint.connectedBody = null;

            m_CurrentGrabbedObject.m_Body.velocity = m_Body.velocity;
            m_CurrentGrabbedObject.m_Body.angularVelocity = m_Body.angularVelocity;

            m_CurrentGrabbedObject = null;
            m_HandModel.SetActive(true);
        }
    }

    public void SetGrabberVisibility(bool visible)
    {
        m_HandModel.SetActive(visible);
    }

    GameObject GetClosestGrabbableObject()
    {
        float closestDistance = float.PositiveInfinity;

        GameObject closestGrabbable = null;

        for(int i = 0; i < m_GrabbableObjectsCandidates.Count; i++)
        {
            float dist = (transform.position - m_GrabbableObjectsCandidates[i].transform.position).sqrMagnitude;

            if(dist < closestDistance)
            {
                closestDistance = dist;
                closestGrabbable = m_GrabbableObjectsCandidates[i];
            }
        }

        return closestGrabbable;
    } 
    
    private void OnTriggerEnter(Collider other)
    {
        GrabbableInformation grabbable;
        if(GrabbableObjectsDic.TryGetValue(other.gameObject, out grabbable))
        {
            m_GrabbableObjectsCandidates.Add(other.gameObject);
        }
        else if (GrabbableUIDic.TryGetValue(other.gameObject, out grabbable))
        {
            m_GrabbableUICandidates.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_GrabbableObjectsCandidates.Contains(other.gameObject))
        {
            m_GrabbableObjectsCandidates.Remove(other.gameObject);
        }
        else if(m_GrabbableUICandidates.Contains(other.gameObject))
        { 
            m_GrabbableUICandidates.Remove(other.gameObject);
        }
    }
}
