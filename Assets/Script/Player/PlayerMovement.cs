using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController => GetComponent<CharacterController>();

    [Header("References")]
    [SerializeField] Camera camera;

    [Header("Settings")]
    [SerializeField] [Range(1, 20)] float speed = 10;
    [SerializeField] [Range(1, 20)] float gravity = 10;

    [SerializeField] [Range(5, 20)] float jumpHeight = 10;

    [SerializeField] [Range(0.1f, 2)] float groundRaycastRange = 2;
    [SerializeField] [Range(0.1f, 5)] float groundUpRaycastRange = 5;

    [SerializeField] [Range(1, 360)] float rotationSpeed = 90f;

    [SerializeField] LayerMask groundMask;

    Quaternion startingRotation;
    Quaternion targetRotation;

    //Input
    PlayerInput input;

    bool axisInUse;
    Vector2 movementInput;

    bool isGrounded;
    Vector3 floorUp;

    public float ySpeed = 0;

    float rotationTime;
    float rotationElapsedTime;

    private void Awake()
    {
        if (camera == false)
        {
            camera = FindObjectOfType<Camera>();
            Debug.LogWarning("No camera assigned in PlayerMovement: " + name + " !");
        }

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
            Jump(jumpHeight);
        };

        input.Enable();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateGrounded();

        Vector3 velocity = Vector3.zero;

        velocity += GravityVelocity();
        velocity += AxisMovementVelocity();

        characterController.Move(velocity * Time.deltaTime);

        LerpRotation();
    }


    Vector3 AxisMovementVelocity()
    {
        if (axisInUse)
        {
            var axisInput = new Vector3(movementInput.x, 0, movementInput.y).normalized;

            var cameraForward = camera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            var movementRotation = Quaternion.FromToRotation(camera.transform.up, floorUp) * camera.transform.rotation;
            var direction = movementRotation * axisInput;

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

            if (Physics.Raycast(transform.position, Vector3.down, out raycastHit, groundUpRaycastRange, groundMask))
            {
                floorUp = raycastHit.normal;
            }
            else
            {
                floorUp = Vector3.up;
            }
        }
    }

    void Jump(float height)
    {

        if (isGrounded && ySpeed <= 0)
        {
            ySpeed = Mathf.Pow(2 * gravity * height, 0.5f);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastRange);
        Gizmos.DrawSphere(transform.position + Vector3.down * groundRaycastRange, 0.15f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundUpRaycastRange);
        Gizmos.DrawSphere(transform.position + Vector3.down * groundUpRaycastRange, 0.15f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * jumpHeight);
        Gizmos.DrawSphere(transform.position + Vector3.up * jumpHeight, 0.15f);
    }

}
