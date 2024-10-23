#nullable enable
using UnityEngine;

public class LegCoordinator : MonoBehaviour
{
    [SerializeField] private Transform currentFeetTarget = null!;
    [SerializeField] private Transform intendedTarget = null!;
    [SerializeField] private float maximumTargetDistance;
    [SerializeField] private AnimationCurve legMovementCurve = null!;
    [Min(0.0001f)][SerializeField] private float legMovementDuration;
    [SerializeField] private AnimationCurve tipRiseCurve = null!;
    [Header("Debug")] 
    [SerializeField] private bool enableDebug = true;
    [Space]
    [SerializeField] private float legMovementTime;

    private Vector3 _lastStableFeetTarget;
    private LegsManager? _optionalManager;
    public LegGroup? OptionalGroup { private set; get; }
 
    private float CurrentTargetSqrDistance => Vector3.SqrMagnitude(intendedTarget.position - currentFeetTarget.position);
    private bool IsWithinMovementDistance => CurrentTargetSqrDistance >= maximumTargetDistance * maximumTargetDistance;
    private bool ShouldStopLeg => MovementCompletion >= 0.99f;
    
    public bool IsMoving { private set; get; }
    public float MovementCompletion => legMovementTime / legMovementDuration;

    private void Start()
    {
        currentFeetTarget.position = intendedTarget.position;
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
    

    private void OnDrawGizmos()
    {
        if (enableDebug == false)
            return;
        Gizmos.color = IsWithinMovementDistance ? Color.red : Color.green;
        Gizmos.DrawLine(currentFeetTarget.position, intendedTarget.position); 
    }

    public void LinkToSystem(LegsManager manager, LegGroup group)
    {
        _optionalManager = manager;
        OptionalGroup = group;

    }
}