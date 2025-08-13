using Asce.Manager.Sounds;
using Asce.Managers;
using Asce.Managers.UIs;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviourSingleton<MainMenuManager>
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _quitButton;


    public Button PlayButton => _playButton;
    public Button NewGameButton => _newGameButton;
    public Button QuitButton => _quitButton;


    private void Start()
    {
        if (PlayButton != null) PlayButton.onClick.AddListener(PlayButton_OnClick);
        if (NewGameButton != null) NewGameButton.onClick.AddListener(NewGameButton_OnClick);
        if (QuitButton != null) QuitButton.onClick.AddListener(QuitButton_OnClick);

        AudioManager.Instance.PlayMusic("Town Theme");
    }

    private void PlayButton_OnClick()
    {
        SceneLoader.Instance.LoadScene("Game");
    }

    private void NewGameButton_OnClick()
    {
        UIAlert alert = UISystem.GetAlert();
        if (alert == null)
        {
            this.NewGame();
            return;
        }

        alert.Set(
            title: "Play new game",
            description: "Are you want to play new game?\nAll data will be lose!",
            buttonATitle: "No", 
            buttonBTitle: "Yes",
            onButtonAClick: () => { },
            onButtonBClick: NewGame
        );
    }

    private void QuitButton_OnClick()
    {
        UIAlert alert = UISystem.GetAlert();
        if (alert == null)
        {
            Application.Quit();
            return;
        }

        alert.Set(
            title: "Quit Game :(",
            description: "Are you want to quit?",
            buttonATitle: "No",
            buttonBTitle: "Yes",
            onButtonAClick: () => { },
            onButtonBClick: Application.Quit
        );
    }

    private void NewGame()
    {
        SaveLoadSystem.DeleteAllPersistentData();
        SceneLoader.Instance.LoadScene("Game");
    }
}
