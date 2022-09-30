using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(PlayerInteractor))]
public class Player : MonoBehaviour
{
    Car car => FindObjectOfType<Car>();

    PlayerMovement movement => GetComponent<PlayerMovement>();

    PlayerAnimations playerAnimations => GetComponentInChildren<PlayerAnimations>();

    Collider collider => GetComponent<Collider>();

    public bool isInCar { get; private set; }
    public enum PlayerState
    {
        Car,
        Ground
    }


    PlayerState currentState = PlayerState.Ground;

    InputComponent inputComponent => GetComponent<InputComponent>();

    public void EnterCar()
    {
        if (currentState != PlayerState.Car)
        {
            isInCar = true;
            currentState = PlayerState.Car;

            inputComponent.Input.Car.Enable();
            inputComponent.Input.Character.Disable();
            Debug.Log("Enter car");

            //playerAnimations.GetInCar();
            playerAnimations.Jump();
            car.enterCurve.pointA.position = transform.position;
            car.enterCurve.follower.t = 0;

            transform.position = car.sitPivot.position;

            collider.enabled = false;

            transform.parent = car.transform;

            movement.GetInCar();
            //movement.MoveInCurve(car.enterCurve);
            //
            car.TurnOn();
        }
    }

    public void ExitCar()
    {
        isInCar = false;

        currentState = PlayerState.Ground;

        inputComponent.Input.Car.Disable();
        inputComponent.Input.Carshop.Disable();
        inputComponent.Input.Character.Enable();

        playerAnimations.LeaveCar();

        movement.enabled = true;

        transform.parent = null;

        movement.OutOfCar();

        movement.StopMoveInCurve();
        movement.Jump(movement.jumpHeight * 2, true);

        StartCoroutine(EnableCollider(1));

        car.TurnOff();
    }

    public void BlockMovement()
    {
        inputComponent.Input.Character.Disable();
    }

    public void FreeMovement()
    {
        inputComponent.Input.Character.Enable();
    }

    public IEnumerator EnableCollider(float time)
    {
        movement.collider.enabled = false;
        yield return new WaitForSeconds(time);
        movement.collider.enabled = true;
    }
}
