using Asce.Manager.Sounds;
using Asce.Managers;
using Asce.Managers.UIs;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs
{
    public class UISettingsWindow : UIWindow
    {
        [Header("Sounds")]
        [SerializeField] protected Slider _masterSlider;
        [SerializeField] protected Slider _musicSlider;
        [SerializeField] protected Slider _sfxSlider;

        [Header("Out Game")]
        [SerializeField] protected Button _exitGameButton;
        [SerializeField] protected Button _backMenuButton;

        protected override void Start()
        {
            base.Start();
            this.LoadSoundSliders();
            if (_masterSlider != null) _masterSlider.onValueChanged.AddListener(MasterSlider_OnValueChanged);
            if (_musicSlider != null) _musicSlider.onValueChanged.AddListener(MusicSlider_OnValueChanged);
            if (_sfxSlider != null) _sfxSlider.onValueChanged.AddListener(SFXSlider_OnValueChanged);

            if (_exitGameButton != null) _exitGameButton.onClick.AddListener(ExitGameButton_OnClick);
            if (_backMenuButton != null) _backMenuButton.onClick.AddListener(BackMenuButton_OnClick);

            _ = this.Load();
        }

        private async Task Load()
        {
            await SaveLoads.SaveLoadManager.Instance.WaitUntilLoadedAsync();
            this.Hide(); // Auto Hide
        }

        private void LoadSoundSliders()
        {
            if (AudioManager.Instance.Settings == null) return;

            if (_masterSlider != null) _masterSlider.value = AudioManager.Instance.Settings.MasterVolume;
            if (_musicSlider != null) _musicSlider.value = AudioManager.Instance.Settings.MusicVolume;
            if (_sfxSlider != null) _sfxSlider.value = AudioManager.Instance.Settings.SFXVolume;
        }

        private void MasterSlider_OnValueChanged(float value)
        {
            if (AudioManager.Instance.Settings == null) return;
            AudioManager.Instance.Settings.MasterVolume = value;
            AudioManager.Instance.ApplySettings();
        }

        private void MusicSlider_OnValueChanged(float value)
        {
            if (AudioManager.Instance.Settings == null) return;
            AudioManager.Instance.Settings.MusicVolume = value;
            AudioManager.Instance.ApplySettings();
        }

        private void SFXSlider_OnValueChanged(float value)
        {
            if (AudioManager.Instance.Settings == null) return;
            AudioManager.Instance.Settings.SFXVolume = value;
            AudioManager.Instance.ApplySettings();
        }

        private void ExitGameButton_OnClick()
        {
            UIAlert alert = UISystem.GetAlert();
            if (alert == null)
            {
                QuitGame();
                return;
            }

            alert.Set(
                title: "Quit Game :(",
                description: "Are you want to quit?",
                buttonATitle: "No",
                buttonBTitle: "Yes",
                onButtonAClick: () => { },
                onButtonBClick: QuitGame
            );
        }
        
        private void BackMenuButton_OnClick()
        {

            UIAlert alert = UISystem.GetAlert();
            if (alert == null)
            {
                BackMenu();
                return;
            }

            alert.Set(
                title: "Back Menu",
                description: "Are you want to back Menu?",
                buttonATitle: "No",
                buttonBTitle: "Yes",
                onButtonAClick: () => { },
                onButtonBClick: BackMenu
            );
        }

        private void QuitGame()
        {
            Game.SaveLoads.SaveLoadManager.Instance.SaveAll();
            Application.Quit();
        }

        private void BackMenu()
        {
            Game.SaveLoads.SaveLoadManager.Instance.SaveAll();
            SceneLoader.Instance.LoadScene("MainMenu", doneDelay: 0.5f);
        }
    }
}
