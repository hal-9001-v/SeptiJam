using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataFileManager;

public enum ControllerMode
{
    Player = 0,
    Car = 1
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController => GetComponent<CharacterController>();
    CheckPointTracker checkPointTracker => GetComponent<CheckPointTracker>();

    GameCamera gameCamera => FindObjectOfType<GameCamera>();

    [Header("References")]

    [Header("Settings")]
    [SerializeField] [Range(1, 20)] float speed = 10;
    [SerializeField] [Range(1, 20)] float gravity = 10;

    [SerializeField] [Range(1, 20)] float jumpHeight = 10;

    [SerializeField] [Range(0.1f, 2)] float groundRaycastRange = 2;

    [SerializeField] [Range(1, 360)] float rotationSpeed = 90f;

    [SerializeField] LayerMask groundMask;

    Quaternion startingRotation;
    Quaternion targetRotation;

    //Input
    PlayerInput input;
    public ControllerMode controllerMode;

    bool axisInUse;
    Vector2 movementInput;

    bool isGrounded;
    Vector3 floorUp;

    float ySpeed = 0;

    float rotationTime;
    float rotationElapsedTime;

    //Car
    [SerializeField] Car carReference;

    bool respawning;
    [SerializeField] [Range(0.1f, 5)] float respawnTime = 2;
    float respawnElapsedTime;

    public float respawnProgress
    {
        get
        {
            if (respawning == false) return 0;
            return respawnElapsedTime / respawnTime;
        }
    }

    private void Awake()
    {
        input = new PlayerInput();
        
        input.Character.MovementAxis.performed += (axis) =>
        {
            movementInput = axis.ReadValue<Vector2>();
            axisInUse = true;
        };

        input.Character.MovementAxis.canceled += (axis) =>
        {
            movementInput = Vector2.zero;
            axisInUse = false;
        };

        input.Character.Jump.performed += (ctx) =>
        {
            Jump(jumpHeight, false);
        };

        input.Character.Respawn.performed += (ctx) =>
        {
            respawning = true;
        };

        input.Character.Respawn.canceled += (ctx) =>
        {
            respawning = false;

        };

        input.Enable();

        SetTargetRotation(transform.rotation, 1);
    }

    // Update is called once per frame
    void Update()
    {

        if (controllerMode == ControllerMode.Player)
        {
            //PLAYER INPUT
            UpdateGrounded();

            Vector3 velocity = Vector3.zero;

            velocity += GravityVelocity();
            velocity += AxisMovementVelocity();

            characterController.Move(velocity * Time.deltaTime);

            LerpRotation();

            RespawnCountdown();
        }
        else
        {
            if (carReference)
            {
                //CAR INPUT
                carReference.Input.Steer = movementInput.x;
                carReference.Input.Forward = movementInput.y;
            }
        }


    }

    void RespawnCountdown()
    {
        if (respawning)
        {
            respawnElapsedTime += Time.deltaTime;

            if (respawnElapsedTime > respawnTime)
            {
                respawnElapsedTime = 0;
                respawning = false;

                checkPointTracker.Respawn();
            }
        }
    }

    Vector3 AxisMovementVelocity()
    {
        if (axisInUse)
        {
            var direction = gameCamera.InputDirection(movementInput, floorUp);

            var horizontalDirection = direction;
            horizontalDirection.y = 0;

            SetTargetRotation(Quaternion.LookRotation(horizontalDirection, Vector3.up), 0.25f);

            return direction * speed;
        }

        return Vector3.zero;

    }
    Vector3 GravityVelocity()
    {
        if (isGrounded && ySpeed < 0)
        {
            ySpeed = 0;
        }
        else
        {
            ySpeed -= gravity * Time.deltaTime;
        }

        return Vector3.up * ySpeed;
    }

    void SetTargetRotation(Quaternion target, float lerpFactor)
    {
        targetRotation = Quaternion.Lerp(transform.rotation, target, lerpFactor);
        startingRotation = transform.rotation;

        rotationTime = Quaternion.Angle(startingRotation, targetRotation) / rotationSpeed;
        rotationElapsedTime = 0;

        if (rotationTime < 0.01f)
        {
            rotationTime = 1;
            rotationElapsedTime = 1;
            startingRotation = target;
        }
    }

    void LerpRotation()
    {
        if (rotationElapsedTime > rotationTime)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            rotationElapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, rotationElapsedTime / rotationTime);
        }
    }

    void UpdateGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var raycastHit, groundRaycastRange, groundMask))
        {
            floorUp = raycastHit.normal;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            floorUp = Vector3.up;
        }
    }

    public void Jump(float height, bool force)
    {

        if ((isGrounded && ySpeed <= 0) || force)
        {
            ySpeed = Mathf.Pow(2 * gravity * height, 0.5f);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastRange);
        Gizmos.DrawSphere(transform.position + Vector3.down * groundRaycastRange, 0.15f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * jumpHeight);
        Gizmos.DrawSphere(transform.position + Vector3.up * jumpHeight, 0.15f);
    }

}
