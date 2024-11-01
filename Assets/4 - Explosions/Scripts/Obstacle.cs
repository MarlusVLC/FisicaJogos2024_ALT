using UnityEngine;

namespace _4___Explosions.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        [Tooltip("Explosion forces will be multiplied by this amount")]
        [Range(0.01f, 2f)][SerializeField] private float forceAttenuationFactor;
        public float ForceAttenuation => forceAttenuationFactor;
    }
}