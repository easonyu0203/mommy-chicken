using System;
using UnityEngine;

namespace Utility
{
    public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.Find(typeof(T).Name)?.GetComponent<T>();
                }

                return _instance;
            }
        }

        private static T _instance;

        protected virtual void Awake()
        {
            _instance = this as T;
        }
    }
}