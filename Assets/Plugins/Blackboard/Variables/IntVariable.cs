using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "Blackboard/Create Variable/Built-in/Integer", order = 0)]
public class IntVariable : ScriptableObject
{
    public int m_Value;

    public static implicit operator int(IntVariable var)
    {
        return var.m_Value;
    }
}
