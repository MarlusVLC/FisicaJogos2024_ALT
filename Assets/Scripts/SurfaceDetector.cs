using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    [SerializeField] private Vector3 repositionOffset;
    [SerializeField] private Vector3 detectionOffset;
    [SerializeField] private LayerMask surfaceMask;
    [SerializeField] private float maxDetectionDistance = 1f;
    [Header("Debug")] 
    [SerializeField] private bool enableDebug = true;

    private bool isDetectingSurface = false;

    private void FixedUpdate()
    {
        isDetectingSurface = Physics.Raycast(transform.position + detectionOffset, Vector3.down, out var hitInfo, maxDetectionDistance,
            surfaceMask);
        if (isDetectingSurface)
        {
            transform.position = hitInfo.point + repositionOffset;
        }
    }

    private void OnDrawGizmos()
    {
        if (enableDebug == false)
            return;
        Gizmos.color = isDetectingSurface ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position + detectionOffset, Vector3.down * maxDetectionDistance); 
    }
}
