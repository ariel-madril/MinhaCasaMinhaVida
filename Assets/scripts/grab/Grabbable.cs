﻿using UnityEngine;
using UnityEngine.Events;

public class GrabbableInformation
{
    public Rigidbody m_Body;
    public GrabbableObject m_GrabbableObject;

    public GrabbableInformation(Rigidbody body, GrabbableObject grabbableObject = null)
    {
        m_Body = body;
        m_GrabbableObject = grabbableObject;
    }
}

public class Grabbable : MonoBehaviour
{
    protected Grabber m_CurrentGrabber;

    public Grabber CurrentGrabber
    {
        get
        {
            return m_CurrentGrabber;
        }
    }

    public UnityEvent GrabbedEvents;
    public UnityEvent ReleasedEvents;

    protected new Transform transform;

    private void Awake()
    {
        transform = base.transform;
    }

    public virtual void ObjectGrabbed(Grabber grabber)
    {
    }

    public virtual void ObjectReleased()
    {
    }

    private void Update()
    {
    }
}