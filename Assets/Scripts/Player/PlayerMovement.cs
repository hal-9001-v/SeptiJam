using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static DataFileManager;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController => GetComponent<CharacterController>();
    public CheckPointTracker checkPointTracker;

    GameCamera gameCamera => FindObjectOfType<GameCamera>();

    public Collider collider => GetComponent<Collider>();

    [Header("References")]
    [SerializeField] Transform model;
    [Header("Settings")]
    [SerializeField] [Range(1, 20)] float speed = 10;

    [SerializeField] [Range(1, 20)] float gravity = 10;

    [Range(1, 20)] public float jumpHeight = 10;

    [SerializeField] [Range(0.1f, 10)] float groundRaycastRange = 2;

    [SerializeField] [Range(1, 360)] float rotationSpeed = 90f;

    [SerializeField] LayerMask groundMask;

    Quaternion startingRotation;
    Quaternion targetRotation;

    bool axisInUse;
    Vector2 movementInput;

    bool isGrounded;
    Vector3 floorUp;

    float ySpeed = 0;

    float rotationTime;
    float rotationElapsedTime;

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

    PlayerAnimations playerAnimations => FindObjectOfType<PlayerAnimations>();

    bool inCar;

    private void Awake()
    {
        SetTargetRotation(transform.rotation, 1);
    }

    private void Start()
    {
        FindObjectOfType<Popup>().EnablePopup(PopupType.triptico);
    }

    public void SetInput(PlayerInput input)
    {
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

        input.Character.Jump.performed += (ctx) => { Jump(jumpHeight, false); };

        input.Character.Respawn.performed += (ctx) => { respawning = true; };

    }

    // Update is called once per frame
    void Update()
    {
        if (inCar == false)
        {
            if (movingInCurve == false)
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
                path.follower.UpdateTimeWithDistance(speed * Time.deltaTime);
                transform.position = path.follower.transform.position;

            }
        }
    }


    public void GetInCar()
    {
        inCar = true;
        characterController.enabled = false;
    }

    public void OutOfCar()
    {
        inCar = false;
        characterController.enabled = true;
    }

    public void Move(Vector3 vel)
    {
        characterController.Move(vel);
    }

    CurvePath path;
    bool movingInCurve = false;
    public void MoveInCurve(CurvePath curve)
    {
        movingInCurve = true;
        path = curve;
    }

    public void StopMoveInCurve()
    {
        movingInCurve = false;
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
            playerAnimations.SetWalkSpeed(movementInput.magnitude);
            playerAnimations.StartWalking();

            var direction = gameCamera.InputDirectionUnNormalized(movementInput, floorUp);

            var horizontalDirection = direction;
            horizontalDirection.y = 0;

            SetTargetRotation(Quaternion.LookRotation(horizontalDirection, Vector3.up), 0.25f);

            return direction * speed;
        }
        else
        {
            playerAnimations.SetWalkSpeed(0);
            playerAnimations.StopWalking();
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
        targetRotation = Quaternion.Lerp(model.transform.rotation, target, lerpFactor);
        startingRotation = model.transform.rotation;

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
            model.transform.rotation = targetRotation;
        }
        else
        {
            rotationElapsedTime += Time.deltaTime;
            model.transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, rotationElapsedTime / rotationTime);
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

        playerAnimations.SetGroundBool(isGrounded);
    }

    public void Jump(float height, bool force)
    {
        if ((isGrounded && ySpeed <= 0) || force)
        {
            ySpeed = Mathf.Pow(2 * gravity * height, 0.5f);
            playerAnimations.Jump();
        }

    }

    public void Jump()
    {
        Jump(jumpHeight, true);

        playerAnimations.Jump();
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