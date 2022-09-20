using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanguageListener : MonoBehaviour
{
    public Language language { get; private set; }

    [SerializeField] UnityEvent changeLanguageEvent;
    public Action<Language> changeLanguageCallback;

    private void Start()
    {
        SetLanguage(FindObjectOfType<LanguageNotifier>().CurrentLanguage);
    }

    public void SetLanguage(Language language)
    {
        this.language = language;

        changeLanguageEvent.Invoke();

        if (changeLanguageCallback != null)
        {
            changeLanguageCallback.Invoke(language);
        }
    }
}
