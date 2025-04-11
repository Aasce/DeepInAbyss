using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asce.Managers
{
    public class SceneLoader : DontDestroyOnLoadSingleton<SceneLoader>
    {
        public event Action<float> OnLoadingProgress;
        public event Action OnLoadingStarted;
        public event Action OnLoadingCompleted;

        /// <summary>
        ///     Load scene by name
        /// </summary>
        public void LoadScene(string sceneName, bool useLoadingScene = true, float doneDelay = 1f)
        {
            if (useLoadingScene)
            {
                StartCoroutine(LoadSceneWithLoading(sceneName, doneDelay));
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        private IEnumerator LoadSceneWithLoading(string targetScene, float doneDelay = 1f)
        {
            OnLoadingStarted?.Invoke();

            SceneManager.LoadScene("Loading"); // Load Loading Scene
            yield return null; // Wait 1 frame for Loading Scene showing

            // Start load scene target
            AsyncOperation async = SceneManager.LoadSceneAsync(targetScene);
            async.allowSceneActivation = false;

            // Progess
            while (async.progress < 0.9f)
            {
                OnLoadingProgress?.Invoke(async.progress);
                yield return null;
            }

            OnLoadingProgress?.Invoke(1);
            yield return new WaitForSeconds(doneDelay);

            async.allowSceneActivation = true;
            while (!async.isDone)
            {
                yield return null;
            }

            OnLoadingCompleted?.Invoke();
        }

        /// <summary>
        ///     Load scene by index
        /// </summary>
        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}