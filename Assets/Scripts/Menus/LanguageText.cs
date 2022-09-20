using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LanguageListener))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageText : MonoBehaviour
{
    LanguageListener languageListener => GetComponent<LanguageListener>();
    TextMeshProUGUI textMesh => GetComponent<TextMeshProUGUI>();

    [Header("Settings")]
    [SerializeField] [TextArea(1, 2)] string englishText;
    [SerializeField] [TextArea(1, 2)] string spanishText;

    private void Awake()
    {
        languageListener.changeLanguageCallback += ChangeLanguage;
    }

    void ChangeLanguage(Language language)
    {
        switch (language)
        {
            case Language.English:
                textMesh.text = englishText;
                break;
            case Language.Spanish:
                textMesh.text = spanishText;
                break;
        }



    }

}
