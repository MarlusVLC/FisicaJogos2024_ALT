using UnityEngine;
using UnityEngine.Serialization;

namespace _4___Explosions.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        [FormerlySerializedAs("forceAttenuationFactor")]
        [Tooltip("Explosion forces will be multiplied by this amount")]
        [Range(-2f, 2f)][SerializeField] private float forceMultiplier = 0.8f;
        public float ForceMultiplier => forceMultiplier;
    }
}