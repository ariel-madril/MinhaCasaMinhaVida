using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIItemController : MonoBehaviour
{

    Animator m_Animator;

    [SerializeField]
    UnityEvent m_ItemSelected;

    [SerializeField]
    Image m_Icon;

    [SerializeField]
    Text m_Text;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(UIItemInfo item)
    {
        m_Icon.sprite = item.m_Icon;
        m_Text.text = item.m_Text;
    }

    public void ShowItem()
    {
        m_Animator.SetTrigger("Show");
    }

    public void HideItem()
    {
        m_Animator.SetTrigger("Hide");
    }
}
