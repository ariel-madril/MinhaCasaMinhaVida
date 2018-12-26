using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BRSFramework.Blackboard
{
    [CreateAssetMenu(fileName = "NewGroup", menuName = "Orchestra/Blackboard/Create Group", order = 0)]
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
} 
