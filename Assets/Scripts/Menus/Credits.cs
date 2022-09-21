using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button backToMenu;

    LevelLoader levelLoader => FindObjectOfType<LevelLoader>();
    EventSystem eventSystem => FindObjectOfType<EventSystem>();

    private void Awake()
    {
        backToMenu.onClick.AddListener(BackToMenu);
        eventSystem.SetSelectedGameObject(backToMenu.gameObject);
    }

    void BackToMenu()
    {
        levelLoader.LoadLevel(LevelLoader.Levels.MainMenu);
    }
}
