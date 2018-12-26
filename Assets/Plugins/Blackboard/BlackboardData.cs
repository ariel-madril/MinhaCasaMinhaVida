using System;
using System.Collections.Generic;
using UnityEngine;

namespace BRSFramework.Blackboard
{
    [Serializable]
    public class BlackboardData
    {
        [SerializeField]
        private string m_Key;

        [SerializeField]
        private ScriptableObject m_Value;

        private ScriptableObject m_ValueCopy;

        [SerializeField]
        private List<Action<ScriptableObject>> m_Listeners = new List<Action<ScriptableObject>>();

        private const int kListCopyPreallocSize = 4;

        private List<Action<ScriptableObject>> m_ListCopy = new List<Action<ScriptableObject>>(kListCopyPreallocSize);

#if UNITY_EDITOR
        [HideInInspector]
        public BlackboardGroup m_Group;

        public const string kNoGroupOption = "Groupless";
        
        [HideInInspector]
        public bool m_ExpandedValue;

        public string GroupName
        {
            get
            {
                return m_Group ? m_Group.Name : kNoGroupOption;
            }
        }
#endif

        public string Key
        {
            get
            {
                return m_Key;
            }
            set
            {
                m_Key = value;
            }
        }

        public ScriptableObject Value
        {
            get
            {
                if (Application.isPlaying)
                {
                    return m_ValueCopy;
                }
                return m_Value;
            }

            set
            {
                if (Application.isPlaying)
                {
                    m_ValueCopy = value;
                }
                else
                {
                    m_Value = value;
                }
            }
        }
                
        public void MakeRuntimeCopy()
        {
            m_ValueCopy = m_Value != null ? ScriptableObject.Instantiate(m_Value) : null;
        }

        public virtual void AddListener(Action<ScriptableObject> listener)
        {
            m_Listeners.Insert(0, listener);
        }

        public virtual void RemoveListener(Action<ScriptableObject> listener)
        {
            m_Listeners.Remove(listener);
        }

        public virtual void ClearListeners()
        {
            m_Listeners.Clear();
        }

        public virtual void Invoke()
        {
            m_ListCopy.AddRange(m_Listeners);
            
            for (int i = 0; i < m_ListCopy.Count; ++i)
            {
                m_ListCopy[i].Invoke(Value);
            }

            m_ListCopy.Clear();
        }

        public List<UnityEngine.Object> GetOwnerListeners()
        {
            List<UnityEngine.Object> listeners = new List<UnityEngine.Object>();

            UnityEngine.Object targetObject = null;

            for (int i = 0; i < m_Listeners.Count; i++)
            {
                targetObject = m_Listeners[i].Target as UnityEngine.Object;

                if (null != targetObject)
                {
                    listeners.Add(targetObject);
                }
            }

            return listeners;
        }
    }
}