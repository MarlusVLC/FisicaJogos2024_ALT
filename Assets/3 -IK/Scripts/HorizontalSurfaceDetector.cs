using UnityEngine;

public class HorizontalSurfaceDetector : MonoBehaviour
{
    [SerializeField] private float detectionOffsetY = 10.0f;
    [SerializeField] private float repositionOffsetY = 0.5f;
    [SerializeField] private float maxDetectionDistance = 20.0f;
    [SerializeField] private LayerMask surfaceMask;
    [Header("Debug")] 
    [SerializeField] private bool enableDebug = true;

    private bool isDetectingSurface = false;
    private Vector3 detectionOffset;
    private Vector3 repositionOffset;
    
    public bool IsDetectingSurface => isDetectingSurface;
    public RaycastHit HitInfo;

    private void Awake()
    {
        detectionOffset = new Vector3(0.0f, detectionOffsetY, 0.0f);
        repositionOffset = new Vector3(0.0f, repositionOffsetY, 0.0f);
    }

    private void FixedUpdate()
    {
        if (enableDebug)
        {
            detectionOffset = new Vector3(0.0f, detectionOffsetY, 0.0f);
            repositionOffset = new Vector3(0.0f, repositionOffsetY, 0.0f);
        }
        isDetectingSurface = Physics.Raycast(transform.position + detectionOffset, Vector3.down, out HitInfo, maxDetectionDistance,
            surfaceMask);
        if (isDetectingSurface)
        {
            transform.position = HitInfo.point + repositionOffset;
        }
    }

    private void OnDrawGizmos()
    {
        if (enableDebug == false && Application.isPlaying == false)
            return;
        Gizmos.color = isDetectingSurface ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position + detectionOffset, Vector3.down * maxDetectionDistance); 
    }
}
