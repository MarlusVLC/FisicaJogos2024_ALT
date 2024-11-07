#nullable enable
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LegCoordinator : MonoBehaviour
{
    // [SerializeField] private bool relocateLegBase = true;
    // [SerializeField] private Transform legBase = null!;
    [SerializeField] private Transform intendedTarget = null!;
    [SerializeField] private float maximumTargetDistance;
    [Min(1.0f)][SerializeField] private float maximumDistanceOverload;
    [Min(0.0001f)][SerializeField] private float legMovementDuration;
    [SerializeField] private AnimationCurve legMovementCurve = null!;
    [SerializeField] private AnimationCurve tipRiseCurve = null!;
    [Header("Debug")] 
    [SerializeField] private bool enableDebug = true;
    [ReadOnly][SerializeField] private float legMovementTime;
    [ReadOnly][SerializeField] private Vector3 _currentTargetToBaseOffset;

    private ChainIKConstraint _ikConstraint = null!;
    private Transform _currentFeetTarget = null!;
    private Vector3 _lastStableFeetTarget;
    private LegsManager? _optionalManager;
    public LegGroup? OptionalGroup { private set; get; }
 
    private float CurrentTargetSqrDistance => Vector3.SqrMagnitude(intendedTarget.position - _currentFeetTarget.position);
    private bool IsWithinMovementDistance => CurrentTargetSqrDistance >= maximumTargetDistance * maximumTargetDistance; 
    private bool IsOutOfBounds => CurrentTargetSqrDistance/maximumDistanceOverload >= maximumTargetDistance * maximumTargetDistance;
    private bool ShouldStopLeg => MovementCompletion >= 0.99f;

    public bool IsMoving { private set; get; }
    public float MovementCompletion => legMovementTime / legMovementDuration;
    public Transform CurrentFeetTarget => _currentFeetTarget;

    private void Start()
    {
        _ikConstraint = GetComponentInChildren<ChainIKConstraint>();
        if (_ikConstraint == null)
        {
            Debug.LogError($"No ChainIKConstraint component found in {gameObject.name}'s children");
        }
        _currentFeetTarget = _ikConstraint.data.target;
        _currentFeetTarget.position = intendedTarget.position;
        // _currentTargetToBaseOffset = legBase.position - currentFeetTarget.position;
    }

    private void FixedUpdate()
    {
        if (ShouldStartMoving())
        {
            legMovementTime = 0f;
            _lastStableFeetTarget = _currentFeetTarget.position;
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
        _currentFeetTarget.position = tempPosition;
        // if (relocateLegBase)
        // {
        //     RelocateLegBase(y: (currentFeetTarget.position + _currentTargetToBaseOffset).y); 
        // }
    }

    private bool ShouldStartMoving()
    {
        bool should = IsWithinMovementDistance;
        if (IsOutOfBounds) return true;
        if (_optionalManager != null)
        {
            should &= _optionalManager.CanMoveLeg(this);
        }
        return should;
    }

    // private void RelocateLegBase(float? x = null, float? y = null, float? z = null)
    // {
    //     var temp = legBase.position;
    //     temp.x = x ?? temp.x;
    //     temp.y = y ?? temp.y;
    //     temp.z = z ?? temp.z;
    //     legBase.position = temp;
    // }
    
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
        Gizmos.DrawLine(_currentFeetTarget.position, intendedTarget.position); 
    }
}