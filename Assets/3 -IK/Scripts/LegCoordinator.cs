#nullable enable
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LegCoordinator : MonoBehaviour
{
    [SerializeField] private bool relocateLegBase = true;
    [SerializeField] private Transform legBase = null!;
    [SerializeField] private Transform intendedTarget = null!;
    [SerializeField] private float maximumTargetDistance;
    [SerializeField] private AnimationCurve legMovementCurve = null!;
    [Min(0.0001f)][SerializeField] private float legMovementDuration;
    [SerializeField] private AnimationCurve tipRiseCurve = null!;
    [Header("Debug")] 
    [SerializeField] private bool enableDebug = true;
    [ReadOnly][SerializeField] private float legMovementTime;
    [ReadOnly][SerializeField] private Vector3 _currentTargetToBaseOffset;

    private ChainIKConstraint _ikConstraint = null!;
    private Transform currentFeetTarget = null!;
    private Vector3 _lastStableFeetTarget;
    private LegsManager? _optionalManager;
    public LegGroup? OptionalGroup { private set; get; }
 
    private float CurrentTargetSqrDistance => Vector3.SqrMagnitude(intendedTarget.position - currentFeetTarget.position);
    private bool IsWithinMovementDistance => CurrentTargetSqrDistance >= maximumTargetDistance * maximumTargetDistance;
    private bool ShouldStopLeg => MovementCompletion >= 0.99f;

    public bool IsMoving { private set; get; }
    public float MovementCompletion => legMovementTime / legMovementDuration;
    public Transform CurrentFeetTarget => currentFeetTarget;

    private void Start()
    {
        _ikConstraint = GetComponentInChildren<ChainIKConstraint>();
        if (_ikConstraint == null)
        {
            Debug.LogError($"No ChainIKConstraint component found in {gameObject.name}'s children");
        }
        currentFeetTarget = _ikConstraint.data.target;
        currentFeetTarget.position = intendedTarget.position;
        _currentTargetToBaseOffset = legBase.position - currentFeetTarget.position;
    }

    private void FixedUpdate()
    {
        if (ShouldStartMoving())
        {
            legMovementTime = 0f;
            _lastStableFeetTarget = currentFeetTarget.position;
            IsMoving = true;
        }
        
        if (ShouldStopLeg)
        {
            IsMoving = false;
            return;
        }

        legMovementTime += Time.fixedDeltaTime / legMovementDuration;

        var tempPosition = Vector3.LerpUnclamped(_lastStableFeetTarget, intendedTarget.position,
            legMovementCurve.Evaluate(MovementCompletion));
        tempPosition.y += tipRiseCurve.Evaluate(MovementCompletion); //Colocar movimento para cima
        currentFeetTarget.position = tempPosition;
        if (relocateLegBase)
        {
            RelocateLegBase(y: (currentFeetTarget.position + _currentTargetToBaseOffset).y); 
        }
    }

    private bool ShouldStartMoving()
    {
        bool should = IsWithinMovementDistance;
        if (_optionalManager != null)
        {
            should &= _optionalManager.CanMoveLeg(this);
        }
        return should;
    }

    private void RelocateLegBase(float? x = null, float? y = null, float? z = null)
    {
        var temp = legBase.position;
        temp.x = x ?? temp.x;
        temp.y = y ?? temp.y;
        temp.z = z ?? temp.z;
        legBase.position = temp;
    }
    
    public void LinkToSystem(LegsManager manager, LegGroup group)
    {
        _optionalManager = manager;
        OptionalGroup = group;
    }

    private void OnDrawGizmos()
    {
        if (enableDebug == false)
            return;
        Gizmos.color = IsWithinMovementDistance ? Color.red : Color.green;
        Gizmos.DrawLine(currentFeetTarget.position, intendedTarget.position); 
    }
}