using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{

    public PlayerInput Input
    {
        get
        {
            return provider.PlayerInput;
        }
    }

    InputProvider provider;

    private void Awake()
    {
        provider = FindObjectOfType<InputProvider>();

        if (provider == false)
        {
            Debug.LogWarning("No InputProvider in scene!");
        }
    }

}
