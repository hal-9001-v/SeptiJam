using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
public class CarShop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CurvePath enterPath;
    [SerializeField] Transform carPivot;
    [SerializeField] Transform exitPosition;


    [SerializeField] Transform platform;
    [SerializeField] Transform cameraFollower;

    [SerializeField] CinemachineVirtualCamera showcaseCamera;

    [SerializeField] CanvasGroup carshopCanvas;
    [SerializeField] InventoryUI inventoryUI;

    [Header("Settings")]
    [SerializeField] [Range(1, 180)] float carRotationSpeed;
    [SerializeField] [Range(1, 180)] float passiveCarRotationSpeed;
    float carRotationSign = 1;

    [SerializeField] [Range(1, 180)] float cameraRotationSpeed;
    [SerializeField] [Range(1, 180)] float maxCameraDegrees;

    [SerializeField] float cameraDegrees = 0;


    [SerializeField] [Range(1, 180)] float closeRotationSpeed;
    float cameraRotationInput;
    float carRotationInput;

    [SerializeField] [Range(0, 5)] float platformElevation;
    [SerializeField] [Range(0, 100)] float platformElevationTime;


    [SerializeField] [Range(1, 20)] float enterSpeed;
    [Header("Sounds")]
    public SoundInfo enterSound;
    public SoundInfo exitSound;

    public string currentState;

    DistanceInteractable distanceInteractable => GetComponent<DistanceInteractable>();
    InputComponent inputComponent => GetComponent<InputComponent>();
    GameCamera gameCamera => FindObjectOfType<GameCamera>();
    Car car => FindObjectOfType<Car>();
    Player player => FindObjectOfType<Player>();


    Vector3 platformStartingPosition;
    Vector3 platformTargetPosition;

    public bool isOpen { private set; get; }

    Quaternion closeStartingRotation;
    Quaternion closeTargetRotation;

    CounterHelper rotationHelper;
    CounterHelper elevationHelper;

    FSMachine machine;

    bool carInWorkshop;
    bool playerInWorkshop;

    bool carIsValid;

    private void Awake()
    {
        distanceInteractable.enterCallback += (interactor) =>
        {

            var player = interactor.GetComponent<PlayerInteractor>();
            if (player)
            {
                playerInWorkshop = true;
            }

            var car = interactor.GetComponent<Car>();
            if (car)
            {
                carInWorkshop = true;
            }

            if (carInWorkshop && playerInWorkshop)
            {
                StartWorkShop();
            }
        };

        distanceInteractable.exitCallback += (interactor) =>
        {
            if (interactor.GetComponent<Car>())
            {
                carInWorkshop = false;
            }

            if (interactor.GetComponent<PlayerInteractor>())
            {
                playerInWorkshop = false;
            }
        };

        platformStartingPosition = platform.position;
        platformTargetPosition = platformStartingPosition + platform.up * platformElevation;
        elevationHelper.targetTime = platformElevationTime;

        CreateMachine();
    }

    private void Start()
    {
        SetInput(inputComponent.Input);
        carshopCanvas.gameObject.SetActive(false);

        enterSound.Initialize(gameObject);
        exitSound.Initialize(gameObject);
    }

    void CreateMachine()
    {
        FSMState idle = new FSMState("Idle", () =>
        {
            return true;
        }, () => { });

        FSMState enterCar = new FSMState("Enter",
          () =>
          {
              if (isOpen)
              {
                  player.BlockMovement();
                  return true;
              }

              return false;
          }, () =>
          {

          });

        FSMState goingUp = new FSMState("goingUp",
             () =>
             {
                 if (enterPath.follower.t == 1)
                 {
                     enterSound.Play();
                     car.transform.parent = carPivot;

                     OpenUI();
                     return true;
                 }

                 return false;
             },
            () =>
            {
                PlatformElevation(true);
                OpenRotations();
            });

        FSMState goingDown = new FSMState("goingDown");
        goingDown.SetCondition(() =>
        {
            if (isOpen == false)
            {
                goingDown.readyForNext = false;
                return true;
            }

            return false;
        });
        goingDown.SetAction(() =>
        {
            PlatformElevation(false);
            CloseRotation();

            if (elevationHelper.normalizedTime == 0 && rotationHelper.normalizedTime >= 1)
            {
                goingDown.readyForNext = true;

                if (carIsValid)
                {
                    car.LeaveCarshop();
                }
                else
                {
                    if (player.isInCar)
                        player.ExitCar();
                    else
                        player.FreeMovement();
                }

                player.FreeMovement();

            }
        });


        idle.children.Add(enterCar);
        enterCar.children.Add(goingUp);
        goingUp.children.Add(goingDown);
        goingDown.children.Add(idle);

        machine = new FSMachine(idle, false);
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
        machine.Update();

        currentState = machine.currentState.name;

        /*if (rotatePlatform)
            OpenRotations();
        else
            CloseRotation();
        */
    }

    void OpenRotations()
    {
        //Camera
        if (cameraRotationInput != 0)
        {
            var deltaDegrees = cameraRotationInput * cameraRotationSpeed * Time.deltaTime;

            if (Mathf.Abs(cameraDegrees + deltaDegrees) < maxCameraDegrees)
            {
                cameraDegrees += deltaDegrees;
                cameraFollower.rotation = Quaternion.AngleAxis(deltaDegrees, Vector3.up) * cameraFollower.rotation;
            }
        }

        //Platform
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

    void PlatformElevation(bool up)
    {
        if (up)
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

    void StartWorkShop()
    {
        if (isOpen == false)
        {
            //input.Carshop.Enable();
            isOpen = true;

            FindObjectOfType<GamePause>().input.UI.Disable();

            gameCamera.UseCamera(showcaseCamera);

            enterPath.a = car.transform.position;
            enterPath.c = carPivot.position;
            enterPath.SetBInMiddleAlongDirection(car.transform.forward);

            car.EnterCarshop(enterPath.follower);
            enterPath.follower.t = 0;

            enterPath.follower.SetAutoUpdate(enterSpeed);

        }
    }

    [ContextMenu("Exit CarShop")]
    public void StopWorkShop()
    {
        if (isOpen)
        {
            isOpen = false;

            carIsValid = CarModificationManager.IsValidCar();

            if (carIsValid == false)
            {
                FindObjectOfType<Popup>().EnablePopup(PopupType.warning);
            }
            gameCamera.UseDefaultCamera();
            SetPlatformForExit();

            HideUI();
        }
    }

    void SetPlatformForExit()
    {
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

        enterSound.Play();
        exitSound.PlayScheduled(enterSound.audioClip.length * 0.9f);

    }

    void OpenUI()
    {
        carshopCanvas.gameObject.SetActive(true);
        inventoryUI.OnOpen();

    }

    void HideUI()
    {
        carshopCanvas.gameObject.SetActive(false);
        FindObjectOfType<Speedometer>().ShowUI();
        FindObjectOfType<GamePause>().input.UI.Enable();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(platform.position, exitPosition.position);
        Gizmos.DrawWireSphere(exitPosition.position, 1f);
    }

}
