using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Enum Variable", menuName = "Blackboard/Create Variable/Built-in/Enum", order = 5)]
public class EnumVariable : ScriptableObject
{
    public Enum m_Value;
}
