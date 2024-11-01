using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _4___Explosions.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class Explosion : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayers;
    [SerializeField] private LayerMask _obstacleLayers;
    [SerializeField] private GameObject _visual;
    [SerializeField] float _radius = 10f;
    [SerializeField] float _time = 1f;
    [SerializeField] AnimationCurve _forceAttenuationCurve;
    [SerializeField] float _maximumForce = 100f;
    [SerializeField] AnimationCurve _damageAttenuationCurve;
    [SerializeField] float _maximumDamage = 100f;
    
    

    private Collider[] _hitColliders = new Collider[5000];

    private void Start()
    {
        _visual.transform.localScale = Vector3.one * (_radius * 2);
        ApplyDamage();
    }
    private void ApplyDamage()
    {
        // var colliders = new Collider[100];
        // var size = Physics.OverlapSphereNonAlloc(transform.position, _radius, colliders, targetLayers);
        // Debug.Log(size);
        // Debug.Log(colliders[0].TryGetComponent(out Destructible dest));
        // ApplyForce(dest);
        
        _hitColliders = Physics.OverlapSphere(transform.position, _radius, _targetLayers);
        // var size = Physics.OverlapSphereNonAlloc(transform.position, _radius, _hitColliders, targetLayers);
        _hitColliders
            .Select(c => c.GetComponent<Destructible>())
            .Where(c => c != null)
            .ToList()
            .ForEach(ApplyForce)
            ;
    }

    private void ApplyForce(Destructible target)
    {
        Vector3 offset = target.transform.position - transform.position;
        Vector3 direction = offset.normalized;
        float distance = offset.magnitude;
        float horizonReach = Mathf.InverseLerp(0, _radius, distance); //How close, percentually, is target from explosion radius
        Debug.Log($"Horizon Reach for {target.name} = {horizonReach}");
        float force = _forceAttenuationCurve.Evaluate(horizonReach) * _maximumForce;
        Debug.Log($"Applied Force for {target.name} = {force}");
        float damage = _damageAttenuationCurve.Evaluate(horizonReach) * _maximumDamage;
        Debug.Log($"Applied Damage for {target.name} = {damage}");

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, _obstacleLayers);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent(out Obstacle obstacle) == false)
                    continue;
                force *= obstacle.ForceAttenuation;
                damage += obstacle.ForceAttenuation;
            }
        }
        
        target.ApplyForce(direction, force, damage, ForceMode.Impulse);
    }

    private void Update()
    {
        _time -= Time.deltaTime;
        if (_time <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
