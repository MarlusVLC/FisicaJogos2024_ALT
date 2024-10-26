using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BodyInverseKinematics : MonoBehaviour
{
    [SerializeField] private TransformDefiningSource[] sources;
    [SerializeField] private Vector3 repositionOffset;

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
    }
}

[Serializable]
public class TransformDefiningSource
{
    public string name;
    public Transform transform;
    public Vector3Bool useAxis;
}