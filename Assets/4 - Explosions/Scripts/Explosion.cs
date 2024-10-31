using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject _visual;
    [SerializeField] float _radius = 10f;
    [SerializeField] float _time = 1f;
    [SerializeField] AnimationCurve _forceAttenuationCurve;
    [SerializeField] float _maximumForce = 100f;

    private Collider[] _hitColliders;

    private void Start()
    {
        _visual.transform.localScale = Vector3.one * (_radius * 2);
        ApplyDamage();
    }
    private void ApplyDamage()
    {
        // if (Physics.OverlapSphereNonAlloc(transform.position, _radius, _hitColliders) > 0)
        // {
        //     foreach (var collider in _hitColliders)
        //     {
        //         if (collider.TryGetComponent(out Destructible destructible))
        //         {
        //             Debug.Log("Destructible");
        //             destructible.Destroy();
        //         }
        //     }
        // }
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        colliders
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
        target.ApplyForce(direction, force);
    }

    private void Update()
    {
        _time -= Time.deltaTime;
        if (_time <= 0f)
        {
            // ApplyDamage();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
