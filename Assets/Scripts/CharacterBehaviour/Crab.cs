using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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

    PlayerMovement player => FindObjectOfType<PlayerMovement>();

    [SerializeField] FSMachine machine;

    Animator animator => GetComponentInChildren<Animator>();
    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    const string AnimatorWalk = "Walking";

    Vector3 targetPosition;

    Spawnable spawnable;

    float idleElapsedTime;
    float idleTargetTime = 5;


    public string currentState;

    private void Awake()
    {
        CreateMachine();
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


        idle.children.Add(flee);
        idle.children.Add(wander);

        goForObject.children.Add(flee);
        goForObject.children.Add(takeObjectToLaid);
        goForObject.children.Add(stopGoingForObject);

        takeObjectToLaid.children.Add(flee);
        takeObjectToLaid.children.Add(stayInLaid);


        stopGoingForObject.children.Add(wander);

        stayInLaid.children.Add(idle);

        wander.children.Add(flee);
        wander.children.Add(goForObject);
        wander.children.Add(idle);
        

        machine = new FSMachine(idle);
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

        agent.SetDestination(targetPosition);
        agent.speed = speed;


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
