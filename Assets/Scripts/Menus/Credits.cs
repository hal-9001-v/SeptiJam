using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button backToMenu;

    LevelLoader levelLoader => FindObjectOfType<LevelLoader>();

    private void Awake()
    {
        backToMenu.onClick.AddListener(BackToMenu);
    }

    void BackToMenu()
    {
        levelLoader.LoadLevel(LevelLoader.Levels.MainMenu);
    }
}
