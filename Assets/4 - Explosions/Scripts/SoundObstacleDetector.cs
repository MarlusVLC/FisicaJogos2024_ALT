using System.Linq;
using GeneralHelpers;
using UnityEngine;

namespace _4___Explosions.Scripts
{
    public class SoundObstacleDetector : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private AnimationCurve soundAttenuationCurve;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask obstacleLayers;
        [SerializeField] private float lowPassCutOffRate;

        private AudioSource audioSource;
        private AudioListener listener;
        private AudioLowPassFilter lowPassFilter;
        private float maxCutoffFrequency;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            lowPassFilter = GetComponent<AudioLowPassFilter>();
            maxCutoffFrequency = lowPassFilter.cutoffFrequency; 
            audioSource.PlayOneShot(clip);
        }

        private void Start()
        {
            listener = SceneRefs.Instance.AudioListener;
        }

        private void Update()
        {
            if (audioSource.isPlaying)
                ExecuteSound();
        }

        private void ExecuteSound()
        {
            float totalDiffractionEffect = 1f;
            
            ModulatedRay ray = new ModulatedRay(transform.position, listener.transform.position);
            Vector3 direction = ray.direction;
            float distance = ray.length;
            float horizonReach =
                Mathf.InverseLerp(0, radius,
                    distance); //How close, percentually, is target from explosion *LISTENABLE* radius
            float volume = soundAttenuationCurve.Evaluate(horizonReach);

            RaycastHit[] forwardObstacles = Physics.RaycastAll(transform.position, direction, distance, obstacleLayers);
            forwardObstacles = forwardObstacles
                .Where(o => o.transform.TryGetComponent(out Obstacle obstacle)
                            && o.transform.GetInstanceID() != listener.transform.GetInstanceID())
                .OrderBy(o => o.distance).ToArray();

            if (forwardObstacles.Length == 0)
            {
                audioSource.volume = volume;
                return;
            }

            RaycastHit[] inverseHits =
                Physics.RaycastAll(listener.transform.position, -direction, distance,
                    obstacleLayers); //Length = forwardHits.Length - 1 
            inverseHits = inverseHits.Where(o => o.transform.TryGetComponent(out Obstacle obstacle)
                                                 && o.transform.GetInstanceID() != listener.transform.GetInstanceID())
                .OrderBy(h => h.distance).ToArray();


            if (forwardObstacles.Length != inverseHits.Length)
            {
                Debug.LogException(new UnityException(
                    "Forward and inverse explosion raycasts should have the same number of hits on the target."));
                Debug.LogWarning(
                    $"Forward raycasts quantity = {forwardObstacles.Length} | Inverse quantity = {inverseHits.Length}");
            }

            var cutOffFrequency = maxCutoffFrequency;
            for (int i = 0; i < forwardObstacles.Length; i++)
            {
                var hit = forwardObstacles[i];
                int inverseIndex = inverseHits.Length - 1 - i;
                var inverseHit = inverseHits[inverseIndex];

                if (hit.colliderInstanceID != inverseHit.colliderInstanceID)
                {
                    Debug.LogException(
                        new UnityException("Colliders should be detected in the same object in the inverse order!"));
                }

                if (hit.collider.TryGetComponent(out Obstacle obstacle) == false)
                    continue;
                var obstacleSpaceCovering = Vector3.Distance(hit.point, inverseHit.point);
                float frequencyReduction = obstacle.ForceMultiplier / Mathf.Max(obstacleSpaceCovering, 1);
                Debug.Log(frequencyReduction);
                cutOffFrequency -= (1-frequencyReduction) * lowPassCutOffRate;
                volume *= frequencyReduction;
            }
            ApplyLowPassFilter(cutOffFrequency);
            audioSource.volume = totalDiffractionEffect;
        }

        private void ApplyLowPassFilter(float cutOffFrequency)
        {
            lowPassFilter.cutoffFrequency = Mathf.Clamp(cutOffFrequency, 500, maxCutoffFrequency);
        }
    }
}