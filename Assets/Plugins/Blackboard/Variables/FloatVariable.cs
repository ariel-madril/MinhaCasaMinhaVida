using UnityEngine;

namespace BRSFramework.Blackboard
{
    [CreateAssetMenu(fileName = "FloatVariable", menuName = "Orchestra/Blackboard/Create Variable/Built-in/Float", order = 1)]
    public class FloatVariable : ScriptableObject
    {
        public float m_Value;
    }
}
