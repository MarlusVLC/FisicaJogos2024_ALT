
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class LegGroup
{
    public LegCoordinator[] legs;

    public bool IsAnyLegMoving() => legs.Any(leg => leg.IsMoving);
}