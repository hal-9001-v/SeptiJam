using UnityEngine;

public class CarTrail : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(0, 5)] float groundRaycastRange = 0.5f;
    [SerializeField] [Range(0, 0.5f)] float trailOffset = 0.1f;
    [SerializeField] LayerMask groundMask;

    TrailRenderer[] trailRendererers => GetComponentsInChildren<TrailRenderer>();
    Vector3[] originalPositions;

    public bool emit = true;

    [SerializeField] [Range(0, 1)] float updateTime = 0.2f;
    float updateElapsedTime;

    private void Awake()
    {
        originalPositions = new Vector3[trailRendererers.Length];

        for (int i = 0; i < originalPositions.Length; i++)
        {
            originalPositions[i] = trailRendererers[i].transform.localPosition;
        }
    }

    private void Update()
    {
        UpdateTrails();
    }

    void UpdateTrails()
    {
        if (emit == false) return;

        updateElapsedTime += Time.deltaTime;

        if (updateElapsedTime >= updateTime)
        {
            updateElapsedTime = 0;
            for (int i = 0; i < trailRendererers.Length; i++)
            {
                TrailRenderer trail = trailRendererers[i];

                RaycastHit hit;
                if (CheckIfGrounded(transform.TransformPoint(originalPositions[i]), out hit))
                {
                    trail.transform.position = hit.point + hit.normal * trailOffset;

                    trail.emitting = true;
                }
                else
                {
                    trail.emitting = false;
                }

            }
        }

    }

    bool CheckIfGrounded(Vector3 position, out RaycastHit hit)
    {
        if (Physics.Raycast(position, -transform.up, out hit, groundRaycastRange, groundMask))
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastRange);
    }
}
