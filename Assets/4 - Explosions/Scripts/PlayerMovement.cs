using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Vector2 _input;
    private Vector3 _velocity;
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_controller == null)
        {
            return;
        }

        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            Jump();
        }

        // Olha para a velocity
        LookTowardVelocity(Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        if (_controller == null)
        {
            return;
        }

        if (_controller.isGrounded && _velocity.y < 0f)
        {
            _velocity.y = 0f;
        }

        // Input
        Vector2 normalizedInput = _input.sqrMagnitude > 1f ? _input.normalized : _input;
        _velocity = new Vector3(normalizedInput.x * _speed, _velocity.y, normalizedInput.y * _speed);
        // Gravidade
        _velocity.y += Time.fixedDeltaTime * -9.81f;
        // Aplica os valores
        _controller.Move(_velocity * Time.fixedDeltaTime);      
    }

    private void LookTowardVelocity(float delta)
    {
        Vector3 velocityXZ = new Vector3(_velocity.x, 0f, _velocity.z);
        if (velocityXZ.sqrMagnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocityXZ, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, delta * 6f);
        }
    }

    private void Jump()
    {
        _velocity.y = 5f;
    }
}
