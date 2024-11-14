using System;
using System.Collections.Generic;
using _4___Explosions.Scripts;
using UnityEngine;

public class ExplosionParticleManipulator : MonoBehaviour
{
    [SerializeField] private InstantExplosion explosion;
    
    private ParticleSystem ps;
    private Collider[] _targetColliders;
    private readonly List<ParticleSystem.Particle> _particlesList = new(200);
    private ParticleSystem.ColliderData _triggerColliderData;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        _targetColliders = Physics.OverlapSphere(transform.position, explosion.Radius, explosion.ObstacleLayers);
        var triggerModule = ps.trigger;
        Array.ForEach(_targetColliders, c => triggerModule.AddCollider(c));
    }
    
    private void OnParticleTrigger()
    {
        //https://docs.unity3d.com/ScriptReference/ParticleSystem.TriggerModule.AddCollider.html
        //https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleTrigger.html
        int numParticles = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, _particlesList, out _triggerColliderData);
        for (int i = 0; i < numParticles; i++)
        {
            var obstacle = _triggerColliderData.GetCollider(i, 0).GetComponent<Obstacle>();
            var particle = _particlesList[i];   
            particle.remainingLifetime *= obstacle.ForceMultiplier;
            _particlesList[i] = particle;
        }
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, _particlesList);
    }
}
