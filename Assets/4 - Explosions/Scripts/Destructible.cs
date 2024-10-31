using UnityEngine;

public class Destructible : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyForce(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
        
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
        //TODO: Instanciar párticulas de explosão
    }
    
    

}
