using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LanguageListener))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageImage : MonoBehaviour
{
    LanguageListener languageListener => GetComponent<LanguageListener>();
    Image image => GetComponent<Image>();

    [Header("Settings")]
    [SerializeField] Sprite englishSprite;
    [SerializeField] Sprite spanishSprite;

    private void Awake()
    {
        languageListener.changeLanguageCallback += ChangeLanguage;
    }

    void ChangeLanguage(Language language)
    {
        switch (language)
        {
            case Language.English:
                image.sprite = englishSprite;
                break;
            case Language.Spanish:
                image.sprite = spanishSprite;
                break;
        }


    }

}
