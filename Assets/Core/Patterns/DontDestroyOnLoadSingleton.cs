using UnityEngine;

namespace Asce.Managers
{
    public abstract class DontDestroyOnLoadSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            if (Instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
