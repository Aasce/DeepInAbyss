using Asce.Managers;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviorSingleton<MainMenuManager>
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;


    public Button PlayButton => _playButton;
    public Button SettingsButton => _settingsButton;
    public Button QuitButton => _quitButton;


    private void Start()
    {
        if (PlayButton != null) PlayButton.onClick.AddListener(PlayButton_OnClick);
        if (SettingsButton != null) SettingsButton.onClick.AddListener(SettingsButton_OnClick);
        if (QuitButton != null) QuitButton.onClick.AddListener(QuitButton_OnClick);

    }

    private void PlayButton_OnClick()
    {
        SceneLoader.Instance.LoadScene("Game");
    }

    private void SettingsButton_OnClick()
    {

    }

    private void QuitButton_OnClick()
    {
        Application.Quit();
    }
}
