using Asce.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Shared
{
    public class LoadingManager : MonoBehaviourSingleton<LoadingManager>
    {
        [SerializeField] private Slider _progressSlider;

        private void OnEnable()
        {
            SceneLoader.Instance.OnLoadingProgress += UpdateProgress;
            SceneLoader.Instance.OnLoadingStarted += Show;
            SceneLoader.Instance.OnLoadingCompleted += Hide;
        }

        private void OnDisable()
        {
            SceneLoader.Instance.OnLoadingProgress -= UpdateProgress;
            SceneLoader.Instance.OnLoadingStarted -= Show;
            SceneLoader.Instance.OnLoadingCompleted -= Hide;
        }

        private void UpdateProgress(float value)
        {
            if (_progressSlider == null) return;
            _progressSlider.value = value;
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}