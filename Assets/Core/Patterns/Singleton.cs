using UnityEngine;

namespace Asce.Managers
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public static T CreateInstance()
        {
            if (_instance == null)
            {
                GameObject singletonObj = new GameObject(typeof(T).Name, typeof(T));
                _instance = singletonObj.GetComponent<T>();
            }
            return _instance;
        }
    }
}