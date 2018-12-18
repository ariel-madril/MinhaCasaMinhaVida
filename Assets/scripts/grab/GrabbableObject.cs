using UnityEngine;

public class GrabbableObject : Grabbable
{
    public bool m_Snap = false;

    private void OnEnable()
    {
        Grabber.AddGrabbableObjectReference(this);
    }

    public override void ObjectGrabbed(Grabber grabber)
    {
        base.ObjectGrabbed(grabber);

        if(m_CurrentGrabber != null)
        {
            m_CurrentGrabber.GrabbedObjectTakenAway();
        }

        m_CurrentGrabber = grabber;

        if (GrabbedEvents != null)
        {
            GrabbedEvents.Invoke();
        }
    }

    public override void ObjectReleased()
    {
        m_CurrentGrabber = null;
        if (ReleasedEvents != null)
        {
            ReleasedEvents.Invoke();
        }
    }

    private void Update()
    {
        if (m_Snap && m_CurrentGrabber != null)
        {
            transform.position = m_CurrentGrabber.SnapPosition.transform.position;
            transform.rotation = m_CurrentGrabber.SnapPosition.transform.rotation;
        }
    }

    private void OnDisable()
    {
        Grabber.RemoveGrabbableObjectReference(this);
    }
    private void OnDestroy()
    {
        Grabber.RemoveGrabbableObjectReference(this);
    }
}
