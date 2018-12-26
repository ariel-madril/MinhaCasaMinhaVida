using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blackboard", menuName = "Blackboard/Create Blackboard", order = 1)]
public class Blackboard : ScriptableObject
{
    public List<BlackboardData> m_Data;

    public void ClearListeners()
    {
        for (int i = 0; i < m_Data.Count; ++i)
        {
            m_Data[i].ClearListeners();
        }
    }
}
