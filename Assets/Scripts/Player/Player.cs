using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteractor))]
public class Player : MonoBehaviour
{
    Car car => FindObjectOfType<Car>();

    PlayerMovement movement => FindObjectOfType<PlayerMovement>();

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
        currentState = PlayerState.Car;

        input.Car.Enable();
        input.Character.Disable();
        Debug.Log("Enter car");
    }

    public void ExitCar()
    {
        currentState = PlayerState.Ground;

        input.Car.Disable();
        input.Character.Enable();
        Debug.Log("Exit car");
    }



}
