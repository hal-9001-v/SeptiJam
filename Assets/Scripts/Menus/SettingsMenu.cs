using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [SerializeField] TextMeshProUGUI currentVolumeText;

    public Action closeCallback;

    LanguageNotifier languageNotifier => FindObjectOfType<LanguageNotifier>();

    private void Awake()
    {
        volumeUp.onClick.AddListener(() =>
        {
            ChangeVolume(true);
        });

        volumeDown.onClick.AddListener(() =>
        {
            ChangeVolume(false);
        });

        closeButton.onClick.AddListener(() =>
        {
            Close();
        });

        english.onClick.AddListener(() =>
        {
            ChangeLanguage(Language.English);
        });

        spanish.onClick.AddListener(() =>
        {
            ChangeLanguage(Language.Spanish);
        });

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
    }

    public void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        Debug.Log("Open Settings");
    }

}
