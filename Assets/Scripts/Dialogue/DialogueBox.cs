using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI textBox;

    Action<bool> endCallback;
    Coroutine typeCoroutine;

    public void StartDialogue(Dialogue dialogue, Action<bool> endCallback)
    {
        StopDialogue();

        this.endCallback = endCallback;
        typeCoroutine = StartCoroutine(TypeDialogue(dialogue, endCallback));
    }

    void StopDialogue()
    {
        if (endCallback != null)
        {
            endCallback.Invoke(false);
        }
        endCallback = null;

        StopCoroutine(typeCoroutine);

    }

    IEnumerator TypeDialogue(Dialogue dialogue, Action<bool> endCallback)
    {
        textBox.text = dialogue.text;
        textBox.maxVisibleCharacters = 0;

        while (textBox.maxVisibleCharacters < textBox.text.Length)
        {
            textBox.maxVisibleCharacters++;
            yield return new WaitForSeconds(dialogue.delay);
        }

        if (endCallback != null)
        {
            endCallback.Invoke(true);
        }

    }


}
