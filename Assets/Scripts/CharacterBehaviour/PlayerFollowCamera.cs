using UnityEngine;
using Cinemachine;

public class PlayerFollowCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] CinemachineVirtualCamera camera;
    [SerializeField] Transform follow;

    [SerializeField] [Range(0, 10)] float mouseRotationSpeed = 3;
    [SerializeField] [Range(1, 180)] float rotationSpeed = 120;

    [SerializeField] [Range(1, 200)] float minDistance;

    GameCamera gameCamera => FindObjectOfType<GameCamera>();

    bool usingDolly;

    public CinemachineSmoothPath path;

    Vector3 closestPathPoint;

    float cameraInput;

    public void SetInput(PlayerInput input)
    {

        input.Camera.MouseRotateCamera.performed += (ctx) =>
        {
            RotateCamera(ctx.ReadValue<float>() * mouseRotationSpeed);

        };

        input.Camera.RotateCamera.performed += (ctx) =>
        {
            cameraInput = ctx.ReadValue<float>();
        };

        input.Camera.RotateCamera.canceled += (ctx) =>
        {
            cameraInput = 0;
        };
    }

    void RotateCamera(float direction)
    {
        follow.transform.rotation = Quaternion.AngleAxis(direction * Time.deltaTime, Vector3.up) * follow.transform.rotation;
    }

    void Update()
    {
        closestPathPoint = path.EvaluatePosition(path.FindClosestPoint(transform.position, 0, -1, -1));
        CheckDistance();
    }

    private void LateUpdate()
    {
        RotateCamera(cameraInput * rotationSpeed);
    }
    void CheckDistance()
    {
        if (Vector3.Distance(closestPathPoint, transform.position) > minDistance)
        {
            SetFollowCamera();
            usingDolly = false;
        }
        else
        {
            gameCamera.UseDefaultCamera();
            usingDolly = true;
        }

    }

    void SetFollowCamera()
    {
        gameCamera.UseCamera(camera);

        if (usingDolly)
        {
            var defaultForward = gameCamera.defaultCamera.transform.forward;
            defaultForward.y = 0;
            defaultForward.Normalize();

            follow.rotation = Quaternion.LookRotation(defaultForward, Vector3.up);
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, closestPathPoint);

        Gizmos.color = Color.blue;
        var toCamera = closestPathPoint - transform.position;
        Gizmos.DrawLine(transform.position, transform.position + toCamera.normalized * minDistance);
        Gizmos.DrawSphere(transform.position + toCamera.normalized * minDistance, 1);
    }


}
