using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_UIMenuItem;

    private List<UIItemController> m_MenuItens;

    [SerializeField]
    private GameObject m_UIPivot;

    [SerializeField]
    private float m_MenuRadius = 3;

    [SerializeField]
    private List<float> m_DegreesBetweenItens;

    Coroutine ShowHideItensRoutine;

    // Start is called before the first frame update
    void Start()
    {
        m_MenuItens = new List<UIItemController>();
        BlackboardController.Instance.AddListener("AddMenuItem", AddMenuItem);
        BlackboardController.Instance.AddListener("ShowMenuItens", ShowMenuItens);
    }

    private void AddMenuItem(ScriptableObject obj)
    {
        UIItemInfo itemInfo = BlackboardController.Instance.GetValue("AddMenuItem") as UIItemInfo;
        SetupMenuItem(itemInfo);
    }

    public void SetupMenuItem(UIItemInfo item)
    {
        GameObject newItemInstance = GameObject.Instantiate(m_UIMenuItem);
        UIItemController newItem = newItemInstance.GetComponent<UIItemController>();
        m_MenuItens.Add(newItem);
        newItem.Setup(item);
        newItem.transform.parent = m_UIPivot.transform;
    }

    public void ShowMenuItens(ScriptableObject obj)
    {
        if(ShowHideItensRoutine != null)
        {
            StopCoroutine(ShowHideItensRoutine);
        }
        ShowHideItensRoutine = StartCoroutine(PerformShowItens());
    }
    
    IEnumerator PerformShowItens()
    {
        foreach(UIItemController item in m_MenuItens)
        {
            item.ShowItem();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void HideMenuItens()
    {
        if (ShowHideItensRoutine != null)
        {
            StopCoroutine(ShowHideItensRoutine);
        }
        ShowHideItensRoutine = StartCoroutine(PerformHideItens());
    }

    IEnumerator PerformHideItens()
    {
        foreach (UIItemController item in m_MenuItens)
        {
            item.HideItem();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDestroy()
    {
        BlackboardController.Instance.RemoveListener("AddMenuItem", AddMenuItem);
        BlackboardController.Instance.RemoveListener("ShowMenuItens", ShowMenuItens);
    }
}
