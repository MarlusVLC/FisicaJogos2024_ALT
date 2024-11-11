using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneralHelpers
{
    public struct ModulatedRay
    {
        public Vector3 origin;
        public Vector3 direction;
        public float length;
        public float intensity;
        public bool isLastRay;

        public ModulatedRay(Vector3 origin, Vector3 direction, float length, bool isLastRay = false)
        {
            this.origin = origin;
            this.direction = direction;
            this.length = length;
            intensity = 1;
            this.isLastRay = isLastRay;
        }

        public ModulatedRay(Vector3 origin, Vector3 endPoint, bool isLastRay = false)
        {
            this.origin = origin;
            var offset = endPoint - origin;
            direction = offset.normalized;
            length = offset.magnitude;
            intensity = 1;
            this.isLastRay = isLastRay;
        }

        public Vector3 EndPoint
        {
            set
            {
                var offset = value - origin;
                direction = offset.normalized;
                length = offset.magnitude;
            }
            get => origin + direction * length;
        }
        
        public static void DebugInteractionPath(IEnumerable<ModulatedRay> rays, Color? dotColor = null, Color? targetColor = null)
        {
            foreach (var ray in rays)
            {
                if (ray.isLastRay == false)
                {
                    Gizmos.color = dotColor ?? Color.red;
                    Gizmos.DrawSphere(ray.EndPoint, 1f * ray.intensity);
                }
                else
                {
                    Gizmos.color = targetColor ?? Color.blue;
                    Gizmos.DrawSphere(ray.EndPoint, 1f * ray.intensity);
                }
                Gizmos.DrawLine(ray.origin, ray.EndPoint);
            }
        }
    }
}