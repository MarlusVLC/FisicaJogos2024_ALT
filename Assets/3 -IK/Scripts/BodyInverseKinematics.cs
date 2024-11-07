using System;
using System.Collections.Generic;
using System.Linq;
using GeneralHelpers;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BodyInverseKinematics : MonoBehaviour
{
    [SerializeField] private TransformDefiningSource[] sources;
    [SerializeField] private Vector3 repositionOffset;
    [SerializeField] private bool useXRotation, useZRotation;

    private void FixedUpdate()
    {
        // transform.position = sources.Aggregate(Vector3.zero, (current, source) => current + source.position) / sources.Length + repositionOffset;
        var targetPos = Vector3.zero;
        var targetCount = Vector3Int.zero;
        foreach (var source in sources)
        {
            if (source.useAxis.x)
            {
                targetPos.x += source.transform.position.x;
                targetCount.x++;
            }
            if (source.useAxis.y)
            {
                targetPos.y += source.transform.position.y;
                targetCount.y++;
            }
            if (source.useAxis.z)
            {
                targetPos.z += source.transform.position.z;
                targetCount.z++;
            }
        }
        targetPos.x = targetCount.x == 0 ? transform.position.x : targetPos.x/targetCount.x;
        targetPos.y = targetCount.y == 0 ? transform.position.y : targetPos.y/targetCount.y;
        targetPos.z = targetCount.z == 0 ? transform.position.z : targetPos.z/targetCount.z;
        transform.position = targetPos + repositionOffset;
        
        // CheckSourcePositions();
        // DefineRotationThoughSourceElevation();
    }

    [ContextMenu("Check Source Positions")]
    private void CheckSourcePositions()
    {
        var leftSources = new List<Vector3>();
        var rightSources = new List<Vector3>();
        var forwardSources = new List<Vector3>();
        var backSources = new List<Vector3>();
        Debug.Log($"This body rotation is {transform.rotation.eulerAngles}");
        foreach (var source in sources)
        {
            var localPosition = transform.InverseTransformPoint(source.transform.position);
            Debug.Log($"{source.name}'s position relative to this is {localPosition}");
            if (localPosition.x > 0)
            {
                rightSources.Add(source.transform.position);
                Debug.Log($"{source.name} is RIGHT to {name}");
            }
            else
            {
                leftSources.Add(source.transform.position);
                Debug.Log($"{source.name} is LEFT to {name}");
            }

            if (localPosition.z > 0)
            {
                forwardSources.Add(source.transform.position);
                Debug.Log($"{source.name} is FORWARD to {name}");
            }
            else
            {
                backSources.Add(source.transform.position);
                Debug.Log($"{source.name} is BACK to {name}");
            }
        }
        var leftHeight = leftSources.Average();
        Debug.Log($"There's {leftSources.Count} sources to the LEFT and it's mean height is {leftHeight}");
        var rightHeight = rightSources.Average();
        Debug.Log($"There's {rightSources.Count} sources to the RIGHT  and it's mean height is {rightHeight}");
        var forwardHeight = forwardSources.Average();
        Debug.Log($"There's {forwardSources.Count} sources to the FORWARD and it's mean height is {forwardHeight}");
        var backHeight = backSources.Average();
        Debug.Log($"There's {backSources.Count} sources to the BACK and it's mean height is {backHeight}");
        
        Debug.Log("Difference between LEFT and RIGHT is = " + (leftHeight - rightHeight));
        Debug.Log("Difference between FORWARD and BACK is = " + (forwardHeight - backHeight));

        var zAngle = Vector3Extensions.ElevationAngle(leftHeight, rightHeight);
        var xAngle = Vector3Extensions.ElevationAngle(forwardHeight, backHeight);
        Debug.Log("Elevation between LEFT and RIGHT is = " + (zAngle));
        Debug.Log("Elevation between FORWARD and BACK is = " + (xAngle));
        
        Debug.Log("zAngle = " + zAngle);
        Debug.Log("xAngle = " + xAngle);
        zAngle = float.IsNaN(zAngle) || !useZRotation ? transform.rotation.eulerAngles.z : zAngle;
        xAngle = float.IsNaN(xAngle) || !useXRotation ? transform.rotation.eulerAngles.x : xAngle;
        transform.localRotation = Quaternion.Euler(xAngle, transform.localRotation.eulerAngles.y, zAngle);
    }
    
      private void DefineRotationThoughSourceElevation()
    { 
        var leftSources = new List<Vector3>();
        var rightSources = new List<Vector3>();
        var forwardSources = new List<Vector3>();
        var backSources = new List<Vector3>();
        foreach (var source in sources)
        {
            var localPosition = transform.InverseTransformPoint(source.transform.position);
            if (localPosition.x > 0)
            {
                rightSources.Add(source.transform.position);
            }
            else
            {
                leftSources.Add(source.transform.position);
            }

            if (localPosition.z > 0)
            {
                forwardSources.Add(source.transform.position);
            }
            else
            {
                backSources.Add(source.transform.position);
            }
        }
        var leftMean = leftSources.Average();
        var rightMean = rightSources.Average();
        var forwardMean = forwardSources.Average();
        var backMean = backSources.Average();

        var zAngle = Vector3Extensions.ElevationAngle(leftMean, rightMean);
        var xAngle = Vector3Extensions.ElevationAngle(forwardMean, backMean);
        
        Debug.Log("zAngle = " + zAngle);
        Debug.Log("xAngle = " + xAngle);
        zAngle = float.IsNaN(zAngle) || !useZRotation ? transform.rotation.eulerAngles.z : zAngle;
        xAngle = float.IsNaN(xAngle) || !useXRotation ? transform.rotation.eulerAngles.x : xAngle;
        transform.localRotation = Quaternion.Euler(xAngle, transform.localRotation.eulerAngles.y, zAngle);
    }
}

[Serializable]
public class TransformDefiningSource
{
    public string name;
    public Transform transform;
    public Vector3Bool useAxis;
}