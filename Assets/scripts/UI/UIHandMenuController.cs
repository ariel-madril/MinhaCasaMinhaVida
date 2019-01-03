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
        GameObject newItemInstance = GameObject.Instantiate(m_UIMenuItem, m_UIPivot.transform);
        newItemInstance.transform.localPosition = new Vector3(newItemInstance.transform.localPosition.x, newItemInstance.transform.localPosition.y, 0);
        UIItemController newItem = newItemInstance.GetComponent<UIItemController>();
        m_MenuItens.Add(newItem);
        newItem.Setup(item);
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
