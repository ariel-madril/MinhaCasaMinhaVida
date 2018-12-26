using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGroup", menuName = "Blackboard/Create Group", order = 0)]
public class BlackboardGroup : ScriptableObject
{
    public string Name
    {
        get
        {
            return this.name;
        }
    }
}
