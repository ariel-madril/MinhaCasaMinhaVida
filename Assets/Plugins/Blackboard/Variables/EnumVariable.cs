using System;
using UnityEngine;

namespace Orchestra.Blackboard
{
    [CreateAssetMenu(fileName = "Enum Variable", menuName = "Orchestra/Blackboard/Create Variable/Built-in/Enum", order = 5)]
    public class EnumVariable : ScriptableObject
    {
        public Enum m_Value;
    }
}