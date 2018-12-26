using System;
using UnityEngine;

namespace BRSFramework.Blackboard
{
    [CreateAssetMenu(fileName = "CustomVariable", menuName = "Orchestra/Blackboard/Create Variable/Built-in/Custom (Example)", order = 100)]
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
}