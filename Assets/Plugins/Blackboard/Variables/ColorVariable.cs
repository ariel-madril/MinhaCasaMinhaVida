using UnityEngine;

[CreateAssetMenu(fileName = "ColorVariable", menuName = "Blackboard/Create Variable/Built-in/Color", order = 4)]
public class ColorVariable : ScriptableObject
{
    public Color m_Value = Color.white;
}
