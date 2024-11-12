using System;
using System.Collections.Generic;
using System.Linq;
using _4___Explosions.Scripts;
using GeneralHelpers;
using UnityEngine;

public class InstantExplosion : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private GameObject visual;
    [SerializeField] float radius = 10f;
    [SerializeField] float time = 1f;
    [SerializeField] AnimationCurve forceAttenuationCurve;
    [SerializeField] float maximumForce = 100f;
    [SerializeField] AnimationCurve damageAttenuationCurve;
    [SerializeField] float maximumDamage = 100f;
    [Space] 
    [SerializeField] private bool showLogs;
    [SerializeField] private bool showForceRays;
    [SerializeField] private bool showDamageRays;
    [SerializeField] private bool showObstacleSpheres;

    private Collider[] _hitColliders = new Collider[5000];
    private readonly List<ModulatedRay> _debugForceRays = new();
    private readonly List<ModulatedRay> _debugDamageRays = new();
    
    private void Start()
    {
        visual.transform.localScale = Vector3.one * (radius * 2);
        ApplyDamage();
    }
    private void ApplyDamage()
    {
        _hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayers);
        // var size = Physics.OverlapSphereNonAlloc(transform.position, radius, _hitColliders, targetLayers);
        _hitColliders
            .Select(c => c.GetComponent<Destructible>())
            .Where(c => c != null)
            .ToList()
            .ForEach(ApplyForce)
            ;
    }

    private void ApplyForce(Destructible target)
    {
        ModulatedRay ray = new ModulatedRay(transform.position, target.transform.position);
        Vector3 direction = ray.direction;
        float distance = ray.length;
        float horizonReach = Mathf.InverseLerp(0, radius, distance); //How close, percentually, is target from explosion radius
        float force = forceAttenuationCurve.Evaluate(horizonReach) * maximumForce;
        float damage = damageAttenuationCurve.Evaluate(horizonReach) * maximumDamage;
        
        RaycastHit[] forwardObstacles = Physics.RaycastAll(transform.position, direction, distance, obstacleLayers);
        forwardObstacles = forwardObstacles
            .Where(o => o.transform.TryGetComponent(out Obstacle obstacle) 
                        && o.transform.GetInstanceID() != target.transform.GetInstanceID())
            .OrderBy(o => o.distance).ToArray();

        if (forwardObstacles.Length == 0)
        {
            TryAddDebugRays(transform.position, target.transform.position, true, force, damage);
            target.ApplyForce(direction, force, damage, ForceMode.Impulse, showLogs);
            return;
        }

        RaycastHit[] inverseHits = Physics.RaycastAll(target.transform.position, -direction, distance, obstacleLayers); //Length = forwardHits.Length - 1 
        inverseHits = inverseHits.Where(o =>  o.transform.TryGetComponent(out Obstacle obstacle) 
                                              && o.transform.GetInstanceID() != target.transform.GetInstanceID())
            .OrderBy(h => h.distance).ToArray();

        
        if (forwardObstacles.Length != inverseHits.Length)
        {
            Debug.LogException(new UnityException(
                "Forward and inverse explosion raycasts should have the same number of hits on the target."));
            Debug.LogWarning($"Forward raycasts quantity = {forwardObstacles.Length} | Inverse quantity = {inverseHits.Length}");
        }
        
        for (int i = 0; i < forwardObstacles.Length; i++)
        {
            // 0 1 2
            var hit = forwardObstacles[i]; // 0 1 2
            int inverseIndex = inverseHits.Length - 1 - i;// 2 1 0  0 1 2
            var inverseHit = inverseHits[inverseIndex];
            
            if (hit.colliderInstanceID != inverseHit.colliderInstanceID)
            {
                Debug.LogException(new UnityException("Colliders should be detected in the same object in the inverse order!"));
            }
            if (i+1 < forwardObstacles.Length && showObstacleSpheres)
            {
                TryAddDebugRays(inverseHit.point, forwardObstacles[i+1].point, false, force, damage);
            }
            if (hit.collider.TryGetComponent(out Obstacle obstacle) == false)
                continue;
            var obstacleSpaceCovering = Vector3.Distance(hit.point, inverseHit.point);
            float finalMultiplier = obstacle.ForceMultiplier / (obstacleSpaceCovering + 1);
            force *= finalMultiplier;
            damage *= finalMultiplier;
            
            if (i == 0 && showObstacleSpheres)
                TryAddDebugRays(transform.position, hit.point, false, force, damage);
            if (inverseIndex == 0)
            {
                TryAddDebugRays(inverseHit.point, target.transform.position, true, force, damage);
            }
            //[Origin, forward[0],        inverse[^1], forward[1],            inverse[^2], forward[2],         inverse[0], target]
        }
        target.ApplyForce(direction, force, damage, ForceMode.Impulse, showLogs);
    }

    private void TryAddDebugRays(Vector3 origin, Vector3 endPoint, bool isLastRay, float force, float damage)
    {
        //Adiciona raios para debug
        if (!showDamageRays && !showForceRays)
            return;
        if (showForceRays)
        {
            var debugRay = new ModulatedRay(origin, endPoint, isLastRay)
            { intensity = force / maximumForce };
            _debugForceRays.Add(debugRay);
        }
        if (showDamageRays)
        {
            var debugRay = new ModulatedRay(origin, endPoint, isLastRay)
            { intensity = damage / maximumDamage };
            _debugDamageRays.Add(debugRay);
        }
    }

    private void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0f)
        {
            Destroy(visual);
        }

        if (time <= -2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    { 
        if (showForceRays) ModulatedRay.DebugInteractionPath(_debugForceRays, Color.yellow, Color.green); 
        if (showDamageRays) ModulatedRay.DebugInteractionPath(_debugDamageRays, Color.magenta, Color.cyan);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}
