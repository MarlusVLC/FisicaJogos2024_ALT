using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float _initialVelocity = 5f;
    [SerializeField] private float _time = 3f;
    [SerializeField] private GameObject _explosion;

    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rb.velocity = transform.forward * _initialVelocity;
    }

    private void FixedUpdate()
    {
        _time -= Time.fixedDeltaTime;
        if (_time <= 0f)
        {
            if (_explosion != null)
            {
                Instantiate(_explosion, transform.position, _explosion.transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
