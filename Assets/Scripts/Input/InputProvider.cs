using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{

    static InputProvider instance;

    public PlayerInput PlayerInput
    {
        get
        {
            if (this == instance)
            {
                return playerInput;
            }
            else
                return instance.PlayerInput;
        }
    }

    PlayerInput playerInput;

    private void Awake()
    {
        if (instance)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            playerInput = new PlayerInput();
            EnableInput();
        }
    }

    public void DisableInput()
    {
        PlayerInput.Disable();
    }

    public void EnableInput()
    {
        PlayerInput.Enable();
    }


}
