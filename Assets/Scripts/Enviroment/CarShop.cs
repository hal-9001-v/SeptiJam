using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarShop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CurvePath enterPath;
    [SerializeField] Transform carPivot;
    [SerializeField] Transform exitPosition;
    [SerializeField] Car car;


    [SerializeField] Transform platform;
    [SerializeField] Transform cameraFollower;

    [SerializeField] CinemachineVirtualCamera showcaseCamera;

    [SerializeField] CanvasGroup carshopCanvas;
    [SerializeField] InventoryUI inventoryUI;

    DistanceInteractable distanceInteractable => GetComponent<DistanceInteractable>();
    GameCamera gameCamera => FindObjectOfType<GameCamera>();

    [Header("Settings")]
    [SerializeField] [Range(1, 180)] float carRotationSpeed;
    [SerializeField] [Range(1, 180)] float passiveCarRotationSpeed;
    float carRotationSign = 1;

    [SerializeField] [Range(1, 180)] float cameraRotationSpeed;
    [SerializeField] [Range(1, 180)] float maxCameraDegrees;

    [SerializeField] float cameraDegrees = 0;

    PlayerInput input;

    [SerializeField] [Range(1, 180)] float closeRotationSpeed;
    float cameraRotationInput;
    float carRotationInput;

    [SerializeField] [Range(0, 5)] float platformElevation;
    [SerializeField] [Range(0, 100)] float platformElevationSpeed;
    Vector3 platformStartingPosition;
    Vector3 platformTargetPosition;

    [Header("Sounds")]
    public AudioClip enterWorkshopAudioClip;
    AudioSource audioSource => GetComponent<AudioSource>();

    public bool isOpen { private set; get; }

    Quaternion closeStartingRotation;
    Quaternion closeTargetRotation;

    CounterHelper rotationHelper;
    CounterHelper elevationHelper;

    bool rotatePlatform;

    private void Awake()
    {
        input = new PlayerInput();
        SetInput(input);
        input.Enable();

        distanceInteractable.enterCallback += (interactor) =>
        {
            var car = interactor.GetComponent<Car>();
            if (car)
            {
                StartWorkShop(car);
            }
        };

        platformStartingPosition = platform.position;
        platformTargetPosition = platformStartingPosition + platform.up * platformElevation;
        elevationHelper.targetTime = platformElevation / platformElevationSpeed;
    }

    private void Start()
    {
        carshopCanvas.gameObject.SetActive(false);
    }

    public void SetInput(PlayerInput input)
    {
        input.Carshop.RotateCar.performed += (ctx) =>
        {
            carRotationInput = ctx.ReadValue<float>();
        };
        input.Carshop.RotateCar.canceled += (ctx) =>
        {
            carRotationInput = 0;
        };

        input.Carshop.RotateCamera.performed += (ctx) =>
        {
            cameraRotationInput = ctx.ReadValue<float>();
        };
        input.Carshop.RotateCamera.canceled += (ctx) =>
        {
            cameraRotationInput = 0;
        };

        input.Carshop.ExitCarshop.performed += (ctx) =>
        {
            StopWorkShop();
        };
    }

    private void Update()
    {
        if (rotatePlatform)
            OpenRotations();
        else
            CloseRotation();

        PlatformElevation();
    }

    void OpenRotations()
    {
        if (cameraRotationInput != 0)
        {
            var deltaDegrees = cameraRotationInput * cameraRotationSpeed * Time.deltaTime;

            if (Mathf.Abs(cameraDegrees + deltaDegrees) < maxCameraDegrees)
            {
                cameraDegrees += deltaDegrees;
                cameraFollower.rotation = Quaternion.AngleAxis(deltaDegrees, Vector3.up) * cameraFollower.rotation;
            }
        }

        if (carRotationInput != 0)
        {
            carRotationSign = Mathf.Sign(carRotationInput);

            var deltaDegrees = carRotationInput * carRotationSpeed * Time.deltaTime;
            platform.rotation = Quaternion.AngleAxis(deltaDegrees, Vector3.up) * platform.rotation;
        }
        else
        {
            var deltaDegrees = carRotationSign * passiveCarRotationSpeed * Time.deltaTime;
            platform.rotation = Quaternion.AngleAxis(deltaDegrees, Vector3.up) * platform.rotation;
        }
    }

    void CloseRotation()
    {
        if (rotationHelper.Update(Time.deltaTime) == false)
        {
            platform.rotation = Quaternion.Lerp(closeStartingRotation, closeTargetRotation, rotationHelper.currentTime / rotationHelper.targetTime);
        }
        else
        {
            platform.rotation = closeTargetRotation;
        }
    }

    void PlatformElevation()
    {
        if (rotatePlatform)
        {
            elevationHelper.Update(Time.deltaTime);
        }
        else
        {
            elevationHelper.Update(-Time.deltaTime);
        }

        platform.transform.position = Vector3.Lerp(platformStartingPosition, platformTargetPosition, elevationHelper.currentTime / elevationHelper.targetTime);
    }

    [ContextMenu("Reset Carshop")]
    private void ResetCarShop()
    {
        //Reset Camera
        cameraFollower.rotation = Quaternion.AngleAxis(-cameraDegrees, Vector3.up) * cameraFollower.rotation;
        cameraDegrees = 0;
    }

    void StartWorkShop(Car car)
    {
        if (isOpen == false)
        {
            isOpen = true;
            rotatePlatform = false;

            FindObjectOfType<GamePause>().input.UI.Disable();

            gameCamera.UseCamera(showcaseCamera);

            enterPath.a = car.transform.position;
            enterPath.c = carPivot.position;
            enterPath.SetBInMiddleAlongDirection(car.transform.forward);


            var time = car.EnterCarshop(enterPath.curveFollower);

            StartCoroutine(CarshopEnterTimeline(time, car));
        }
    }

    [ContextMenu("Exit CarShop")]
    public void StopWorkShop()
    {
        //TODO: if it doesn't have anything, no exit

        if (!CarModificationManager.IsValidCar())
        {
            FindObjectOfType<Popup>().EnablePopup(PopupType.warning);
            return;
        }
        if (isOpen)
        {
            isOpen = false;
            rotatePlatform = false;

            gameCamera.UseDefaultCamera();

            isOpen = false;
            var toExit = exitPosition.position - platform.transform.position;
            toExit.y = 0;
            toExit.Normalize();

            closeTargetRotation = Quaternion.FromToRotation(platform.transform.forward, toExit) * platform.rotation;
            closeTargetRotation = Quaternion.FromToRotation(car.transform.forward, platform.transform.forward) * closeTargetRotation;
            closeStartingRotation = platform.rotation;

            rotationHelper.Reset();
            float closeTime = Quaternion.Angle(closeStartingRotation, closeTargetRotation) / closeRotationSpeed;
            rotationHelper.targetTime = closeTime;
            if (closeTime < 0.05f)
            {
                rotationHelper.targetTime = 1;
                rotationHelper.Update(1);
                closeStartingRotation = closeTargetRotation;
            }
            gameCamera.UseDefaultCamera();

            carshopCanvas.gameObject.SetActive(false);
            FindObjectOfType<Speedometer>().ShowUI();
            FindObjectOfType<GamePause>().input.UI.Enable();


            StartCoroutine(CarshopExitTimeline());
        }
    }

    IEnumerator CarshopEnterTimeline(float time, Car car)
    {
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(enterWorkshopAudioClip);

        rotatePlatform = true;
        car.transform.parent = carPivot;
        this.car = car;
        carshopCanvas.gameObject.SetActive(true);
        inventoryUI.OnOpen();

    }

    IEnumerator CarshopExitTimeline()
    {
        while (elevationHelper.currentTime > 0 || rotationHelper.normalizedTime < 1)
        {
            yield return null;
        }


        Debug.Log("End of carshop!");

        car.LeaveCarshop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(platform.position, exitPosition.position);
        Gizmos.DrawWireSphere(exitPosition.position, 1f);
    }

}
