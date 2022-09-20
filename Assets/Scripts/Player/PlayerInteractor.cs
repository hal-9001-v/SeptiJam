using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Interactor))]
public class PlayerInteractor : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] [Range(0, 10)] float range = 3;
    [SerializeField] Color color = Color.white;

    Interactor interactor => GetComponent<Interactor>();

    PlayerInput input;

    private void Awake()
    {
        input = new PlayerInput();

        input.Character.Interact.performed += (ctx) =>
        {
            interactor.InteractWithPassives(range);
        };

        input.Enable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, range);
    }


}
