using UnityEngine;

namespace Orchestra.Blackboard
{
    [CreateAssetMenu(fileName = "StringVariable", menuName = "Orchestra/Blackboard/Create Variable/Built-in/String", order = 3)]
    public class StringVariable : ScriptableObject
    {
        public string m_Value;
    }
}