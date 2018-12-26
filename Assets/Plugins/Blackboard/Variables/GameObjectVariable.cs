using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectVariable", menuName = "Blackboard/Create Variable/GameObjectVariable", order = 100)]
public class GameObjectVariable : ScriptableObject
{
    public GameObject m_Value;
}
