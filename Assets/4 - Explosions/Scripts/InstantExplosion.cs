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
        if (showLogs) Debug.Log($"RAY START for {target.name}");
        ModulatedRay ray = new ModulatedRay(transform.position, target.transform.position);
        Vector3 direction = ray.direction;
        float distance = ray.length;
        float horizonReach = Mathf.InverseLerp(0, radius, distance); //How close, percentually, is target from explosion radius
        if (showLogs) Debug.Log($"Horizon Reach for {target.name} = {horizonReach}");
        float force = forceAttenuationCurve.Evaluate(horizonReach) * maximumForce;
        if (showLogs) Debug.Log($"Initial Applied Force for {target.name} = {force}");
        float damage = damageAttenuationCurve.Evaluate(horizonReach) * maximumDamage;
        if (showLogs) Debug.Log($"Initial Applied Damage for {target.name} = {damage}");
        
        //Hipotese: No caminho do raycast há uma parede e duas caixas. O alvo é a ultima caixa. A parede e as duas caixas estão no 'obstacleLayer'
        //Tenta detectar obstáculos
        //Vai do centro da explosão até a ultima caixa. Detecta parede, caixa e alvo.
        RaycastHit[] forwardObstacles = Physics.RaycastAll(transform.position, direction, distance, obstacleLayers); //Length = 3
        // forwardObstacles = forwardObstacles.Where(h => h.collider.TryGetComponent(out Obstacle obstacle)).ToArray();
        if (showLogs) Debug.Log($"Forward Hits for {target.name} = {forwardObstacles.Length}");
        
        //case 0 não deve ser possível pq destructible obrigatoriamente precisa ter um collider
        switch (forwardObstacles.Length)
        {
            //Se nenhum obstáculo for detectado, adicione uma linha direta da Origem ao Alvo para a coleção de debug
            //Se forwardHits tem apenas 1 item, este item é automaticamente o alvo. E automaticamente inverseHits está vazio (o centro não é detectado, por não conter collider!)
            //Essa condição não é atendida na hipotése
            case 1:
            {
                ray.isLastRay = true;
                if (showForceRays) ray.intensity = force / maximumForce; _debugForceRays.Add(ray);
                if (showDamageRays) ray.intensity = damage / maximumDamage; _debugDamageRays.Add(ray);
                target.ApplyForce(direction, force, damage, ForceMode.Impulse, showLogs);
                return;
            }
            //Se inverseHits for maior que 0, infere-se que forwardHits é maior que 1
            case > 1:
                //Ordena forwardHits e tira o alvo à fim de deixar os dois com a mesma quantidade 
                //Poderemos utilizar, depois, as referÊncias para o centro e o alvo sem tê-los nesses arrays.
                forwardObstacles = forwardObstacles.OrderBy(h => h.distance).Take(forwardObstacles.Length - 1).ToArray();
                //forwardHits e inverseHits não contém centro nem alvo
                break;
        }
        
        //Vai do alvo até o centro da explosão. Detecta a caixa e o obstáculo. NÃO DETECTA ELE MESMO!
        RaycastHit[] inverseHits = Physics.RaycastAll(target.transform.position, -direction, distance, obstacleLayers); //Length = forwardHits.Length - 1 
        inverseHits = inverseHits.OrderBy(h => h.distance).ToArray();
        if (showLogs) Debug.Log($"Inverse Hits for {target.name} = {inverseHits.Length}");
        
        //Tratamento de erro para garantir que o ajuste foi feito adequadamente
        if (forwardObstacles.Length != inverseHits.Length)
        {
            Debug.LogException(new UnityException(
                "Forward and inverse explosion raycasts should have the same number of hits on the target."));
            Debug.LogWarning($"Forward raycasts quantity = {forwardObstacles.Length} | Inverse quantity = {inverseHits.Length}");
        }
        
        for (int i = 0; i < forwardObstacles.Length; i++)
        {
            var hit = forwardObstacles[i];
            if (showLogs) Debug.LogWarning($"Forward Index #{i} - ID = {hit.colliderInstanceID}");
            int inverseIndex = inverseHits.Length - 1 - i;
            var inverseHit = inverseHits[inverseIndex];
            if (showLogs) Debug.LogWarning($"Inverse Index #{inverseIndex} - ID = {inverseHit.colliderInstanceID}");
            if (hit.colliderInstanceID != inverseHit.colliderInstanceID)
            {
                Debug.LogException(new UnityException("Colliders should be detected in the same object in the inverse order!"));
            }
            if (hit.collider.TryGetComponent(out Obstacle obstacle) == false)
                continue;
            if (showLogs) Debug.Log($"{hit.colliderInstanceID} IS AN OBSTACLE");
            var obstacleSpaceCovering = Vector3.Distance(hit.point, inverseHit.point);
            if (showLogs) Debug.Log("Obstacle length = " + obstacleSpaceCovering);
            float finalMultiplier = obstacle.ForceMultiplier / (obstacleSpaceCovering + 1);
            // float finalMultiplier = obstacle.ForceMultiplier / Mathf.Exp(obstacleSpaceCovering);
            if (showLogs) Debug.Log("Force Multiplier = " + finalMultiplier); 
            force *= finalMultiplier;
            damage *= finalMultiplier;
            
            //Adiciona raios para debug
            if (!showDamageRays && !showForceRays)
                continue;
            var debugRay = new ModulatedRay(
                i == 0 ? transform.position : inverseHit.point, 
                i == forwardObstacles.Length - 1 ? target.transform.position : hit.point, 
                i == forwardObstacles.Length - 1);
            if (showForceRays)
            {
                debugRay.intensity = force / maximumForce; 
                _debugForceRays.Add(debugRay);
            }
            if (showDamageRays)
            {
                debugRay.intensity = damage / maximumDamage; 
                _debugDamageRays.Add(debugRay);
            }
            //[Origin, forward[0],        inverse[^1], forward[1],            inverse[^2], forward[2],         inverse[0], target]
        }
        
        target.ApplyForce(direction, force, damage, ForceMode.Impulse, showLogs);
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
        if (showDamageRays) ModulatedRay.DebugInteractionPath(_debugDamageRays, Color.magenta, Color.red);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}
