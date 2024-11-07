using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float maximumHealth = 100f;
    [SerializeField] private Renderer renderer;
    
    [SerializeField][ReadOnly] private float currentHealth;

    private Rigidbody rb;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maximumHealth;
        originalColor = renderer.material.color;
    }

    public void ApplyForce(Vector3 direction, float force, float damage, ForceMode mode = ForceMode.Force)
    {
        rb.AddForce(direction * force, mode);
        ApplyDamage(damage);
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        var bodyColor = renderer.material.color;
        bodyColor.r = (currentHealth / maximumHealth) * originalColor.r;
        bodyColor.b = (currentHealth / maximumHealth) * originalColor.b;
        bodyColor.g = (currentHealth / maximumHealth) * originalColor.g;
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
