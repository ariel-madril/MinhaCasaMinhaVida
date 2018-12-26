using UnityEngine;

namespace BRSFramework.Blackboard
{
    [CreateAssetMenu(fileName = "BoolVariable", menuName = "Orchestra/Blackboard/Create Variable/Built-in/Bool", order = 2)]
    public class BoolVariable : ScriptableObject
    {
        public bool m_Value;

        public static implicit operator bool(BoolVariable b)
        {
            return b.m_Value;
        }
    }
}