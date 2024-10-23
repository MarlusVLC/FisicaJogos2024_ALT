using System;
using System.Linq;
using UnityEngine;

public class LegsManager : MonoBehaviour
{
    [SerializeField] private LegGroup[] legGroups;

    private void Start()
    {
        AddLegToSystem();
    }

    private void AddLegToSystem()
    {
        foreach (var group in legGroups)
        {
            foreach (var leg in @group.legs)
            {
                leg.LinkToSystem(this, @group);
            }
        }
    }

    public bool CanMoveLeg(LegCoordinator leg)
    {
        foreach (var legGroup in legGroups)
        {
            if (leg.OptionalGroup == legGroup)
            {
                return !AreOtherLegsMoving(legGroup);
            }
        }
        throw new MissingMemberException("Leg must be contained within one manager's groups");
    }

    private bool AreOtherLegsMoving(LegGroup entryGroup)
    {
        foreach (var legGroup in legGroups)
        {
            if (entryGroup == legGroup)
                continue;
            if (legGroup.IsAnyLegMoving())
                return true;
        }
        return false;
    }
    
    public Vector3 GetFeetMediumPos()
    {
        var sum = legGroups.Aggregate(Vector3.zero, (current, leg) => current + leg.GetFeetMediumPos());
        return sum / legGroups.Length;
    }
}