using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(0, 10)] float timeBeforeLoad = 1;
    [SerializeField] [Range(0, 10)] float timeAfterLoad = 1;
    public float progress { get; private set; }

    Coroutine loadingCoroutine;

    static LevelLoader levelLoader;

    private void Awake()
    {
        if (levelLoader == null)
        {
            levelLoader = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    public enum Levels
    {
        MainMenu = 0,
        Game = 1,
        Credits = 2
    }

    public void LoadLevel(Levels level)
    {
        if (levelLoader != this)
        {
            levelLoader.LoadLevel(level);
            return;
        }

        switch (level)
        {
            case Levels.MainMenu:
                break;
            case Levels.Game:
                break;
            case Levels.Credits:
                break;
            default:
                break;
        }

        if (loadingCoroutine != null)
            StopCoroutine(loadingCoroutine);

        loadingCoroutine = StartCoroutine(LoadAsync((int)level));
    }

    IEnumerator LoadAsync(int index)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(index);

        yield return new WaitForSeconds(timeBeforeLoad);
        while (op.isDone == false)
        {
            progress = op.progress;

            yield return null;
        }
        yield return new WaitForSeconds(timeAfterLoad);

    }

}
