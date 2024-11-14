using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform _object;

    void Update()
    {
        if (_object == null)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(_object, transform.position, transform.rotation);
        }
    }
}
