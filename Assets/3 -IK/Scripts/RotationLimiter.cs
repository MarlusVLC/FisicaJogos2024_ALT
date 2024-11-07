using UnityEngine;

namespace _3__IK.Scripts
{
    public class RotationLimiter : MonoBehaviour
    {
        [SerializeField] private Vector3 minAngles;
        [SerializeField] private Vector3 maxAngles;

        private void FixedUpdate()
        {
            var rotation = transform.localRotation.eulerAngles;
            if (maxAngles.x < rotation.x || minAngles.x > rotation.x)
            {
                Debug.Log("X = " + rotation.x);
            }
            // rotation.x = Mathf.Clamp(rotation.x, minAngles.x, maxAngles.x);
            // rotation.y = Mathf.Clamp(rotation.y, minAngles.y, maxAngles.y);
            // rotation.z = Mathf.Clamp(rotation.z, minAngles.z, maxAngles.z);
            // transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}