using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    InventoryInGame inventoryGame => FindObjectOfType<InventoryInGame>();

    EventSystem eventSystem => FindObjectOfType<EventSystem>();

    public Action pauseCallback;
    public Action resumeCallback;

    Speedometer speedometer => FindObjectOfType<Speedometer>();

    public PlayerInput input;

    bool paused;
    private Popup popup;
    private void Awake()
    {
        popup = FindObjectOfType<Popup>();
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(DisplaySettings);
        backToMenuButton.onClick.AddListener(GetToMainMenu);
        
        input = new PlayerInput();

        input.UI.Pause.performed += Pause;
        input.UI.OpenInventory.performed += Inventory;
        input.UI.Back.performed += ClosePopup;
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
        
        if (paused == false && !inventoryGame.inventoryOpened)
        {
            paused = true;

            if (pauseCallback != null)
            {
                pauseCallback.Invoke();
            }
            FindObjectOfType<Speedometer>().HideUI();
            Time.timeScale = 0;

            Open();
        }
        else
        {
            ResumeGame();
        }

    }
    
    void ClosePopup(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        popup.DisablePopup();
    }


    void Open()
    {
        canvasGroup.transform.GetChild(0).gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;


        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(resumeButton.gameObject);
    }


    void ResumeGame()
    {
        if (paused)
        {
            paused = false;

            if (resumeCallback != null)
            {
                resumeCallback.Invoke();
            }
            
            if (speedometer)
                speedometer.ShowUI();

            Time.timeScale = 1;

            //Settings.close() and then this.Close(). Otherwise, settingsMenu.close will open this menu with its close callback
            settingsMenu.Close();
            Close();

            eventSystem.SetSelectedGameObject(null);
        }
    }

    void Close()
    {
        canvasGroup.transform.GetChild(0).gameObject.SetActive(false);
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
        input.UI.OpenInventory.performed -= Inventory;

        levelLoader.LoadLevel(LevelLoader.Levels.MainMenu);
    }
    void Inventory(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!paused) inventoryGame.OnSelectPressed();
    }
}
