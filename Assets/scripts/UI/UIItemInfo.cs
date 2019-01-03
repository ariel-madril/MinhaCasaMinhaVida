using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIItemInfo", menuName = "MCMV/UI/Item", order = 100)]
public class UIItemInfo : ScriptableObject
{
    public Sprite m_Icon;
    public string m_Text;
    public List<UIItemInfo> m_SubItens;
}
