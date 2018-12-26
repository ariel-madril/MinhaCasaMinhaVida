using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomVariable", menuName = "Blackboard/Create Variable/Built-in/Custom (Example)", order = 100)]
public class CustomVariable : ScriptableObject
{
    [Serializable]
    public struct MyStruct
    {
        public Color m_Color;
        public Color m_BgColor;
    }

    public int m_ValueA;
    public float m_ValueB;
    public MyStruct m_ValueC;

}
