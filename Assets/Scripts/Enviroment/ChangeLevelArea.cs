using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionInteractable))]
public class ChangeLevelArea : MonoBehaviour
{
    CollisionInteractable interactable => GetComponent<CollisionInteractable>();

    LevelLoader loader => FindObjectOfType<LevelLoader>();

    private void Awake()
    {
        interactable.enterTriggerCallback += (interactor) =>
        {
            var player = interactable.GetComponent<PlayerMovement>();

            if (player)
                ChangeLevel();
        };
    }

    void ChangeLevel()
    {
        loader.LoadLevel(LevelLoader.Levels.Credits);
    }

}
