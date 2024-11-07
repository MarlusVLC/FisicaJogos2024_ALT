using UnityEngine;

namespace GeneralHelpers
{
    public static class Vector3Extensions
    {
        public static float ElevationAngle(Vector3 from, Vector3 to)
        {
            // Height difference (y)
            float deltaY = to.y - from.y;
            
            // Horizontal distance between points (in XZ plane)
            to.y = from.y = 0;
            float deltaXZ = Vector3.Distance(from, to);
            
            // Calculate elevation angle (in radians), then convert it to degrees
            return  Mathf.Atan2(deltaY, deltaXZ) * Mathf.Rad2Deg;
        }
    }
}