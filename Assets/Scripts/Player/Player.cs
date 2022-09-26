using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteractor))]
public class Player : MonoBehaviour
{
    Car car => FindObjectOfType<Car>();

    PlayerMovement movement => GetComponent<PlayerMovement>();

    PlayerAnimations playerAnimations => GetComponentInChildren<PlayerAnimations>();

    Collider collider => GetComponent<Collider>();
    public enum PlayerState
    {
        Car,
        Ground
    }


    PlayerState currentState = PlayerState.Ground;

    PlayerInput input;

    private void Awake()
    {
        input = new PlayerInput();

        car.SetInput(input);
        movement.SetInput(input);

        input.Enable();

        ExitCar();
    }

    public void EnterCar()
    {
        if (currentState != PlayerState.Car)
        {
            currentState = PlayerState.Car;

            input.Car.Enable();
            input.Character.Disable();
            Debug.Log("Enter car");

            //playerAnimations.GetInCar();
            playerAnimations.Jump();
            car.enterCurve.pointA.position = transform.position;
            car.enterCurve.follower.t = 0;

            collider.enabled = false;
            //movement.MoveInCurve(car.enterCurve);   
        }
    }

    public void ExitCar()
    {
        currentState = PlayerState.Ground;

        input.Car.Disable();
        input.Character.Enable();
        Debug.Log("Exit car");

        playerAnimations.LeaveCar();

        collider.enabled = true;
        movement.enabled = true;
        movement.StopMoveInCurve();
        movement.Jump();
    }



}
