using System.Collections;
using UnityEngine;

namespace Asce.Managers
{
    public class BootstrapManager : MonoBehaviourSingleton<BootstrapManager>
    {
        protected override void Awake()
        {
            base.Awake();

            GameManager.CreateInstance();
            SceneLoader.CreateInstance();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(.5f);
            SceneLoader.Instance.LoadScene("MainMenu", doneDelay: 0.5f);
        }
    }
}