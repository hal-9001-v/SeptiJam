using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CollisionInteractable))]
public class Crab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;


    public CrabEnvinroment environment;

    [Header("Settings")]
    [SerializeField] [Range(0.05f, 10)] float speed;
    [SerializeField] [Range(0.05f, 10)] float fleeSpeed;

    [SerializeField] [Range(1, 10)] float fleeRange;

    [Header("Launch")]
    [SerializeField] float launchSpeed = 15;
    [SerializeField] float rotationSpeed = 15;
    [SerializeField] [Range(0, 90)] float angle = 45;

    [SerializeField] float respawnTime = 10;


    Rigidbody modelRigidbody => GetComponentInChildren<Rigidbody>();
    PlayerMovement player => FindObjectOfType<PlayerMovement>();
    const string AnimatorWalk = "Walking";
    Animator animator => GetComponentInChildren<Animator>();
    CollisionInteractable interactable => GetComponent<CollisionInteractable>();

    //NavMesh
    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    Vector3 targetPosition;

    

    //FSM variables
    FSMachine machine;
    float idleElapsedTime;
    float idleTargetTime = 5;
    bool launched;
    
    //Object taken by crab
    Spawnable spawnable;

    public string currentState;
    CounterHelper airCounter;

    //Restore Model
    Vector3 startingPosition;
    Quaternion startingRotation;

    private void Awake()
    {
        interactable.enterTriggerCallback += Launch;

        startingPosition = animator.transform.localPosition;
        startingRotation = animator.transform.localRotation;
    }

    private void Start()
    {
        CreateMachine();
    }

    void Launch(Interactor interactor)
    {
        var car = interactor.GetComponent<PlayerMovement>();

        if (car)
        {
            launched = true;

            var forward = transform.position - car.transform.position;
            forward.y = 0;
            forward.Normalize();

            modelRigidbody.isKinematic = false;
            var lookRotation = Quaternion.LookRotation(forward, Vector3.up);

            var direction = Quaternion.AngleAxis(-angle, Vector3.right) * Vector3.forward;
            direction = lookRotation * direction;

            modelRigidbody.AddForce(direction * launchSpeed, ForceMode.VelocityChange);

            direction = Vector3.right;
            direction = lookRotation * direction;
            modelRigidbody.AddTorque(direction * rotationSpeed, ForceMode.VelocityChange);

        }


    }

    void CreateMachine()
    {
        FSMState idle = new FSMState("Idle", () =>
        {
            if (HorizontalDistanceToPoint(targetPosition) < 0.5f)
            {
                animator.SetBool(AnimatorWalk, false);
                return true;
            }
            return false;
        }, () =>
        {

        });

        FSMState goForObject = new FSMState("GoForObject", () =>
        {

            if (environment.objectReady)
            {
                spawnable = environment.GetRandomObject();
                animator.SetBool(AnimatorWalk, true);

                Vector3 objectPosition = spawnable.transform.position;
                StartMovingToPoint(objectPosition, speed);
                return true;
            }

            return false;
        }, () =>
        {

        });

        FSMState takeObjectToLaid = new FSMState("Take Object to Laid", () =>
        {

            if (HorizontalDistanceToPoint(targetPosition) < 0.3f)
            {
                StartMovingToPoint(environment.GetRandomLair().position, speed);
                environment.RetrieveObject(spawnable);

                spawnable.transform.parent = rightHand;
                spawnable.transform.localPosition = Vector3.zero;

                animator.SetBool(AnimatorWalk, true);


                return true;
            }
            return false;

        }, () =>
        {
        }
        );

        FSMState flee = new FSMState("Flee", () =>
        {

            if (Vector3.Distance(transform.position, player.transform.position) < fleeRange)
            {
                StartMovingToPoint(environment.GetRandomLair().position, fleeSpeed);
                animator.SetBool(AnimatorWalk, true);

                return true;
            }
            return false;

        }, () =>
        {
        }
        );

        FSMState stayInLaid = new FSMState("Stay in Laid", () =>
        {
            if (HorizontalDistanceToPoint(targetPosition) < 0.5f)
            {
                if (spawnable)
                {
                    environment.AddObject(spawnable);
                    spawnable = null;
                }
                animator.SetBool(AnimatorWalk, false);

                return true;
            }

            return false;
        }, () => { });

        FSMState wander = new FSMState("Wander", () =>
        {
            if (UpdateCounter())
            {
                StartMovingToPoint(environment.GetRandomPosition(), speed);
                animator.SetBool(AnimatorWalk, true);

                return true;
            }
            return false;
        },
        () => { });

        FSMState stopGoingForObject = new FSMState("Stop Going for object", () =>
        {
            if (spawnable && environment.IsSpawnableReady(spawnable) == false)
            {
                spawnable = null;
                StartMovingToPoint(transform.position, speed);
                animator.SetBool(AnimatorWalk, false);
                return true;
            }

            return false;

        }, () =>
        {

        });

        FSMState air = new FSMState("Launched", () =>
        {
            if (launched)
            {
                airCounter.targetTime = respawnTime;
                airCounter.Reset();

                agent.enabled = false;

                return true;
            }

            return false;
        }, () =>
        {

            if (airCounter.Update(Time.deltaTime))
            {
                launched = false;
            }
        });

        FSMState spawn = new FSMState("Spawn", () =>
        {
            if (launched == false)
            {
                modelRigidbody.isKinematic = true;
                modelRigidbody.velocity = Vector3.zero;

                RestoreModel();

                agent.enabled = true;
                agent.Warp(environment.GetRandomLair().position);

                if (spawnable)
                {
                    environment.AddObject(spawnable);
                    spawnable = null;
                }

                return true;
            }
            return false;
        }, () => { });

        spawn.children.Add(idle);
        spawn.children.Add(wander);

        idle.children.Add(flee);
        idle.children.Add(wander);
        idle.children.Add(air);

        goForObject.children.Add(flee);
        goForObject.children.Add(takeObjectToLaid);
        goForObject.children.Add(stopGoingForObject);
        goForObject.children.Add(air);

        takeObjectToLaid.children.Add(flee);
        takeObjectToLaid.children.Add(stayInLaid);
        takeObjectToLaid.children.Add(air);


        stopGoingForObject.children.Add(wander);
        stopGoingForObject.children.Add(air);

        stayInLaid.children.Add(idle);
        stayInLaid.children.Add(wander);

        wander.children.Add(flee);
        wander.children.Add(goForObject);
        wander.children.Add(idle);
        wander.children.Add(air);

        flee.children.Add(air);
        flee.children.Add(stayInLaid);

        air.children.Add(spawn);


        machine = new FSMachine(spawn, true);
    }

    private void Update()
    {
        machine.Update();
        currentState = machine.currentState.name;
    }

    void StartMovingToPoint(Vector3 point, float speed)
    {
        targetPosition = point;

        targetPosition.y = transform.position.y;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosition);
            agent.speed = speed;
        }
    }

    void RestoreModel()
    {
        animator.transform.localPosition = startingPosition;
        animator.transform.localRotation = startingRotation;

    }

    bool UpdateCounter()
    {
        idleElapsedTime += Time.deltaTime;

        if (idleElapsedTime > idleTargetTime)
        {
            idleElapsedTime = 0;

            idleTargetTime = Random.Range(2, 10);

            return true;
        }

        return false;
    }

    float HorizontalDistanceToPoint(Vector3 point)
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(point.x, point.z));
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, fleeRange);
    }
}
