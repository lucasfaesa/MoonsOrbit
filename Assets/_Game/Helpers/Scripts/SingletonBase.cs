using UnityEngine;

namespace Helpers
{
    public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                    
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                //This uses the as keyword in C#, which attempts to cast this (the current object) to the type T.
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
        
            else if(_instance != this)
                Destroy(this.gameObject);
            
        }
    
    }
}

