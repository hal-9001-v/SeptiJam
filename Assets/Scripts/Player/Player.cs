using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteractor))]
public class Player : MonoBehaviour
{
    Car car => FindObjectOfType<Car>();

    PlayerMovement movement => GetComponent<PlayerMovement>();

    PlayerAnimations playerAnimations => GetComponentInChildren<PlayerAnimations>();

    PlayerFollowCamera cameraFollow => GetComponentInChildren<PlayerFollowCamera>();

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
        cameraFollow.SetInput(input);
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
        currentState = PlayerState.Ground;

        input.Car.Disable();
        input.Carshop.Disable();
        input.Character.Enable();
        Debug.Log("Exit car");

        playerAnimations.LeaveCar();

        movement.enabled = true;

        transform.parent = null;

        movement.OutOfCar();

        movement.StopMoveInCurve();
        movement.Jump(movement.jumpHeight * 2, true);

        StartCoroutine(EnableCollider(1));

        car.TurnOff();
    }



    public IEnumerator EnableCollider(float time)
    {
        movement.collider.enabled = false;
        yield return new WaitForSeconds(time);
        movement.collider.enabled = true;
    }
}
