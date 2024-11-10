using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float maximumHealth = 100f;
    [SerializeField] private Renderer renderer;
    
    [SerializeField][ReadOnly] private float currentHealth;

    private Rigidbody rb;
    private Collider collider;
    private Color originalColor;
    public float HealthPercentage => currentHealth / maximumHealth;
    public Collider Collider => collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        currentHealth = maximumHealth;
        originalColor = renderer.material.color;
    }

    public void ApplyForce(Vector3 direction, float force, float damage, ForceMode mode = ForceMode.Force, bool showLogs = false)
    {
        rb.AddForce(direction * force, mode);
        ApplyDamage(damage);
        if (showLogs) Debug.Log($"Final Applied Force for {name} = {force}");
        if (showLogs) Debug.Log($"Final Applied Damage for {name} = {damage}");
        if (showLogs) Debug.Log($"RAY END for {name}");
        if (showLogs) Debug.Log($"---------------------");
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        var bodyColor = renderer.material.color;
        bodyColor.r = HealthPercentage * originalColor.r;
        bodyColor.b = HealthPercentage * originalColor.b;
        bodyColor.g = HealthPercentage * originalColor.g;
        renderer.material.color = bodyColor;
        if (currentHealth <= 0)
        {
            Destroy();
        }
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
        //TODO: Instanciar párticulas de explosão
    }
    
    

}
