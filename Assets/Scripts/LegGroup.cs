
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class LegGroup
{
    public LegCoordinator[] legs;

    public bool IsAnyLegMoving() => legs.Any(leg => leg.IsMoving);
    public Vector3 GetFeetMediumPos()
    {
        var sum = legs.Aggregate(Vector3.zero, (current, leg) => current + leg.CurrentFeetTarget.position);
        return sum / legs.Length;
    }
    
}