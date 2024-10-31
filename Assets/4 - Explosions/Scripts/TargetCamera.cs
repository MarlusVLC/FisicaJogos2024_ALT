using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }
        // Move para o target suavemente
        Vector3 targetPosition = _target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 6f);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (_target == null)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_target.position, _target.position + _offset);
        Gizmos.DrawSphere(_target.position + _offset, 0.1f);
    }
#endif//UNITY_EDITOR
}
