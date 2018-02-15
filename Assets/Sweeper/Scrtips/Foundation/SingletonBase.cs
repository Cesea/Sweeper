using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }
                return _instance;
            }
        }

        protected void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
