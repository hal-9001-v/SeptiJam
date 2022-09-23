using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button exitButton;

    SettingsMenu settingsMenu => FindObjectOfType<SettingsMenu>();
    LevelLoader levelLoader => FindObjectOfType<LevelLoader>();
    EventSystem eventSystem => FindObjectOfType<EventSystem>();

    private void Awake()
    {
        playButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(DisplaySettings);
        exitButton.onClick.AddListener(ExitGame);

    }

    private void Start()
    {
        settingsMenu.closeCallback += Open;
        Open();
    }

    void StartGame()
    {
        levelLoader.LoadLevel(LevelLoader.Levels.Game);
    }

    void DisplaySettings()
    {
        if (settingsMenu)
        {
            settingsMenu.Open();
            Close();
        }
    }

    void ExitGame()
    {
        Application.Quit();
    }

    void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(playButton.gameObject);
    }


}
