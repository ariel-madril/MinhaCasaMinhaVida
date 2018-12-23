using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : Grabbable
{
    public enum ObjectType
    {
        FLOOR,
        WALL
    }

    public ObjectType m_Type;

    public bool m_Snap = false;

    List<Renderer> m_Renderers;

    List<Material> m_DefaultMaterias;

    private void OnEnable()
    {
        m_Renderers = new List<Renderer>();

        m_DefaultMaterias = new List<Material>();

        Renderer ownRenderer = GetComponent<Renderer>();

        if (ownRenderer != null)
        {
            m_Renderers.Add(ownRenderer);
        }

        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        if(childRenderers.Length > 0)
        {
            m_Renderers.AddRange(new List<Renderer>(childRenderers));
        }

        for(int i = 0; i < m_Renderers.Count; i++)
        {
            m_DefaultMaterias.Add(m_Renderers[i].material);
        }

        Grabber.AddGrabbableObjectReference(this);
    }

    public override void ObjectGrabbed(Grabber grabber)
    {
        base.ObjectGrabbed(grabber);

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

    public void RestoreMat()
    {
        for (int i = 0; i < m_DefaultMaterias.Count; i++)
        {
            m_Renderers[i].material = m_DefaultMaterias[i];
        }
    }

    public void SetHoverMat(Material mat)
    {
        if(m_CurrentGrabber != null)
        {
            return;
        }

        foreach(Renderer rend in m_Renderers)
        {
            rend.material = mat;
        }
    }

    private void Update()
    {
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
