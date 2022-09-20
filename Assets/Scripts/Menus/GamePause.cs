using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePause : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button backToMenuButton;

    SettingsMenu settingsMenu => FindObjectOfType<SettingsMenu>();
    LevelLoader levelLoader => FindObjectOfType<LevelLoader>();

    public Action pauseCallback;
    public Action resumeCallback;

    PlayerInput input;

    private void Awake()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(DisplaySettings);
        backToMenuButton.onClick.AddListener(GetToMainMenu);

        input = new PlayerInput();

        input.UI.Pause.performed += Pause;
        input.Enable();


        Close();

    }

    private void Start()
    {
        settingsMenu.closeCallback += Open;

    }

    // Let ctx there so it can be += and -= to avoid a nullPointer when reloading scenes. Dont make questions
    void Pause(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (pauseCallback != null)
        {
            pauseCallback.Invoke();
        }

        Time.timeScale = 0;

        Open();

    }

    void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }


    void ResumeGame()
    {
        if (resumeCallback != null)
        {
            resumeCallback.Invoke();
        }

        Time.timeScale = 1;

        Close();
    }

    void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    void DisplaySettings()
    {
        if (settingsMenu)
        {
            settingsMenu.Open();
        }

        Close();
    }

    void GetToMainMenu()
    {
        Time.timeScale = 1;
        
        input.UI.Pause.performed -= Pause;

        levelLoader.LoadLevel(LevelLoader.Levels.MainMenu);
    }


}
