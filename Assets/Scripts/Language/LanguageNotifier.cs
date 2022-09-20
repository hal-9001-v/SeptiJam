using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageNotifier : MonoBehaviour
{
    public Language CurrentLanguage
    {
        get
        {
            if (languageNotifier == this)
                return currentLanguage;

            return languageNotifier.CurrentLanguage;
        }
    }

    Language currentLanguage;

    static LanguageNotifier languageNotifier;

    private void Awake()
    {
        if (languageNotifier == null)
        {
            languageNotifier = this;
            transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void NotifyLanguage(Language language)
    {
        currentLanguage = language;
        foreach (var listener in FindObjectsOfType<LanguageListener>())
        {
            listener.SetLanguage(language);
        }
    }
}
