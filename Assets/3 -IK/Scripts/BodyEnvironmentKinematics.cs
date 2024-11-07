using GeneralHelpers;
using UnityEngine;

namespace _3__IK.Scripts
{
    public class BodyEnvironmentKinematics : MonoBehaviour
    {
        [SerializeField] private HorizontalSurfaceDetector forwardReference;
        [SerializeField] private HorizontalSurfaceDetector backReference;
        [SerializeField] private HorizontalSurfaceDetector rightReference;
        [SerializeField] private HorizontalSurfaceDetector leftReference;

        private float zAngle;
        private float xAngle;
        private void Update()
        {
            var rotation = transform.localRotation.eulerAngles;
            if (forwardReference.IsDetectingSurface && backReference.IsDetectingSurface)
            {
                xAngle = Vector3Extensions.ElevationAngle(forwardReference.HitInfo.point, backReference.HitInfo.point);
                Debug.Log("xAngle = " + xAngle);
                xAngle = float.IsNaN(xAngle) ? transform.rotation.eulerAngles.x : xAngle;
                rotation.x = xAngle;
            }
            
            if (leftReference.IsDetectingSurface && rightReference.IsDetectingSurface)
            {
                zAngle = Vector3Extensions.ElevationAngle(leftReference.HitInfo.point, rightReference.HitInfo.point);
                Debug.Log("zAngle = " + zAngle);
                zAngle = float.IsNaN(zAngle) ? transform.rotation.eulerAngles.z : zAngle;
                rotation.z = zAngle;
            }

            transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}