using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BRSFramework.Blackboard
{
    public class BlackboardController : MonoBehaviour
    {
        protected const string LOG_WARNING_KEY_NOT_FOUND = "[Blackboard] Key not found: ";

        public UnityAction OnBlackboardInitialize;

        [SerializeField]
        protected Blackboard m_Blackboard;

        protected Dictionary<int, BlackboardData> m_Data = new Dictionary<int, BlackboardData>();

        protected Dictionary<string, Coroutine> m_InvokeDelayList;
        
        protected virtual void Awake()
        {
            InitializeBlackboard();
        }

        protected virtual void OnApplicationQuit()
        {
            m_Blackboard.ClearListeners();
        }

        protected virtual void OnDestroy()
        {
        }

        public virtual void InitializeBlackboard()
        {
            m_InvokeDelayList = new Dictionary<string, Coroutine>();

            for (int i = 0; i < m_Blackboard.m_Data.Count; i++)
            {
                m_Blackboard.m_Data[i].MakeRuntimeCopy();

                int hashKey = m_Blackboard.m_Data[i].Key.GetHashCode();
                m_Data.Add(hashKey, m_Blackboard.m_Data[i]);
            }

            if (null != OnBlackboardInitialize)
            {
                OnBlackboardInitialize.Invoke();
            }
        }

        public virtual void ReloadBlackboard(string key)
        {
            if (m_Blackboard.m_Data != null)
            {
                BlackboardData data;

                if (TryGetValue_Internal(key, out data))
                {
                    data.MakeRuntimeCopy();
                }
            }            
        }

        public virtual void AddListener(string key, Action<ScriptableObject> callback)
        {
            BlackboardData data;

            if (TryGetValue_Internal(key, out data))
            {
                data.AddListener(callback);
            }
            else
            {
                Debug.LogError("Cant find " + key);
            }
        }

        public virtual void RemoveListener(string key, Action<ScriptableObject> callback)
        {
            BlackboardData data;

            if (TryGetValue_Internal(key, out data))
            {
                data.RemoveListener(callback);
            }
        }

        public virtual void Invoke(string key)
        {
            BlackboardData data;

            if (TryGetValue_Internal(key, out data))
            {
                data.Invoke();
            }
        }

        private IEnumerator InvokeDelayRoutine(string key, float delay)
        {
            BlackboardData data;
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

            float t = 0;

            while (t < delay)
            {
                t += Time.deltaTime;

                yield return waitForEndOfFrame;
            }

            if (TryGetValue_Internal(key, out data))
            {
                data.Invoke();
            }

            m_InvokeDelayList.Remove(key);
        }

        protected virtual bool TryGetValue_Internal(string key, out BlackboardData data)
        {
            int hashKey = key.GetHashCode();

            if (m_Data.TryGetValue(hashKey, out data))
            {
                data = m_Data[hashKey];
                return true;
            }
            else
            {
                Debug.LogWarning(LOG_WARNING_KEY_NOT_FOUND + key);
                return false;
            }
        }

        public virtual ScriptableObject GetValue(string key)
        {
            BlackboardData data = null;
            
            if (TryGetValue_Internal(key, out data))
            {
                return data.Value;
            }
            else
            {
                return null;
            }
        }

        public virtual void SetValue(string key, ScriptableObject obj)
        {
            int hash = key.GetHashCode();
            if (m_Data.ContainsKey(hash))
            {
                m_Data[hash].Value = obj;
            }
        }
    }
}