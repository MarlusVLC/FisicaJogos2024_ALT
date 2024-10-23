using System;
using System.Linq;
using UnityEngine;

public class BodyInverseKinematics : MonoBehaviour
{
    [SerializeField] private Transform[] sources;
    [SerializeField] private Vector3 repositionOffset;

    private void FixedUpdate()
    {
        transform.position = sources.Aggregate(Vector3.zero, (current, source) => current + source.position) / sources.Length + repositionOffset;
    }
}