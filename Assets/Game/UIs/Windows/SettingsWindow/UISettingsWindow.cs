using Asce.Managers;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UISettingsWindow : UIWindow
    {
        [Header("Out Game")]
        [SerializeField] protected Button _exitGameButton;
        [SerializeField] protected Button _backMenuButton;

        protected override void Start()
        {
            base.Start();
            if (_exitGameButton != null) _exitGameButton.onClick.AddListener(ExitGameButton_OnClick);
            if (_backMenuButton != null) _backMenuButton.onClick.AddListener(BackMenuButton_OnClick);

            _ = this.Load();
        }

        private async Task Load()
        {
            await SaveLoads.SaveLoadManager.Instance.WaitUntilLoadedAsync();
            this.Hide(); // Auto Hide
        }

        private void ExitGameButton_OnClick()
        {
            Game.SaveLoads.SaveLoadManager.Instance.SaveAll();
            Application.Quit();
        }
        
        private void BackMenuButton_OnClick()
        {
            Game.SaveLoads.SaveLoadManager.Instance.SaveAll();
            SceneLoader.Instance.LoadScene("MainMenu", doneDelay: 0.5f);
        }
    }
}
