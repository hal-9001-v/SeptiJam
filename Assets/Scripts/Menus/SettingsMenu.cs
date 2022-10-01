using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button closeButton;

    [SerializeField] Button volumeUp;
    [SerializeField] Button volumeDown;

    [SerializeField] Button english;
    [SerializeField] Button spanish;

    [SerializeField] Slider currentVolume;

    public Action closeCallback;

    LanguageNotifier languageNotifier => FindObjectOfType<LanguageNotifier>();
    EventSystem eventSystem => FindObjectOfType<EventSystem>();

    [SerializeField] SoundInfo buttonSound;

    private void Awake()
    {
        volumeUp.onClick.AddListener(() =>
        {
            ChangeVolume(true);
            buttonSound.Play();
        });

        volumeDown.onClick.AddListener(() =>
        {
            ChangeVolume(false);
            buttonSound.Play();
        });

        closeButton.onClick.AddListener(() =>
        {
            Close();
            buttonSound.Play();
        });

        english.onClick.AddListener(() =>
        {
            ChangeLanguage(Language.English);
            buttonSound.Play();

        });

        spanish.onClick.AddListener(() =>
        {
            ChangeLanguage(Language.Spanish);
            buttonSound.Play();
        });

    }

    private void Start()
    {
        buttonSound.Initialize(gameObject);

        Close();

    }

    void ChangeLanguage(Language language)
    {
        languageNotifier.NotifyLanguage(language);

    }

    void ChangeVolume(bool up)
    {

        if (up)
        {
            Debug.Log("Volume Up");
        }
        else
        {
            Debug.Log("Volume Down");
        }
    }

    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        if (closeCallback != null)
        {
            closeCallback.Invoke();
        }

        buttonSound.Play();
    }

    public void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(volumeDown.gameObject);
    }

}
