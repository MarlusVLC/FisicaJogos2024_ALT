using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float maximumHealth = 100f;
    
    [SerializeField][ReadOnly] private float currentHealth;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maximumHealth;
    }

    public void ApplyForce(Vector3 direction, float force, float damage, ForceMode mode = ForceMode.Force)
    {
        rb.AddForce(direction * force, mode);
        currentHealth -= damage;
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
