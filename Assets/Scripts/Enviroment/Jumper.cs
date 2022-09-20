using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    CollisionInteractable interactable => GetComponentInChildren<CollisionInteractable>();

    [Header("Settings")]
    [SerializeField] [Range(1, 50)] float height;



    private void Awake()
    {
        interactable.enterTriggerCallback += (interactor) =>
        {
            var player = interactor.GetComponent<PlayerMovement>();

            if (player)
            {
                player.Jump(height, true);
            }
        };
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * height);
        Gizmos.DrawSphere(transform.position + Vector3.up * height, 0.15f);

    }

}
