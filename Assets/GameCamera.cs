using Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public CinemachineVirtualCamera defaultCamera;

    private void Start()
    {
        UseDefaultCamera();
    }

    /// <summary>
    /// Get Direction in World coordinates applying the camera rotation and the desired normal. 
    /// </summary>
    /// <param name="axisInput"></param>
    /// <param name="up"></param>
    /// <returns></returns>
    public Vector3 InputDirection(Vector2 axisInput, Vector3 up)
    {
        var input = new Vector3(axisInput.x, 0, axisInput.y).normalized;

        return Quaternion.FromToRotation(transform.up, up) * transform.rotation * input;
    }
    
    public Vector3 InputDirectionUnNormalized(Vector2 axisInput, Vector3 up)
    {        
        var input = new Vector3(axisInput.x, 0, axisInput.y);

        return Quaternion.FromToRotation(transform.up, up) * transform.rotation * input;
    }
    

    /// <summary>
    /// Get direction in World coordinates applying the camera rotation with World.up as Up
    /// </summary>
    /// <param name="axisInput"></param>
    /// <returns></returns>
    public Vector3 InputDirection(Vector2 axisInput)
    {
        return InputDirection(axisInput, Vector3.up);
    }

    public void UseCamera(CinemachineVirtualCamera cameraToUse)
    {
        foreach (var camera in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            camera.enabled = false;
        }
        cameraToUse.enabled = true;
    }

    public void UseDefaultCamera()
    {
        UseCamera(defaultCamera);
    }
}
