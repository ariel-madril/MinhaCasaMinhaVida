using UnityEngine;

namespace BRSFramework.Blackboard
{
    [CreateAssetMenu(fileName = "ColorVariable", menuName = "Orchestra/Blackboard/Create Variable/Built-in/Color", order = 4)]
    public class ColorVariable : ScriptableObject
    {
        public Color m_Value = Color.white;
    }
}