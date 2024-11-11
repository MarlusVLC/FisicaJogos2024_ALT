using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float maximumHealth = 100f;
    [SerializeField] private Renderer renderer;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float dmgFeedbackDuration = 0.5f;
    
    [SerializeField][ReadOnly] private float currentHealth;

    private Rigidbody rb;
    private Collider collider;
    private Color originalColor;
    private Coroutine feedbackRoutine;
    
    public float HealthPercentage => currentHealth / maximumHealth;
    public Collider Collider => collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        currentHealth = maximumHealth;
        originalColor = renderer.material.color;
        text.enabled = false;
    }

    public void ApplyForce(Vector3 direction, float force, float damage, ForceMode mode = ForceMode.Force, bool showLogs = false)
    {
        rb.AddForce(direction * force, mode);
        ApplyDamage(damage);
        if (showLogs) Debug.Log($"Final Applied Force for {name} = {force}");
        if (showLogs) Debug.Log($"Final Applied Damage for {name} = {damage}");
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        ShowHealthStatus();
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

    private IEnumerator ShowHealthStatusCoroutine()
    {
        text.text = ((int)(HealthPercentage*100f)).ToString(CultureInfo.InvariantCulture) + "%";
        text.enabled = true;
        yield return new WaitForSeconds(dmgFeedbackDuration);
        text.enabled = false;
    }

    public void ShowHealthStatus()
    {
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(ShowHealthStatusCoroutine());
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
        //TODO: Instanciar párticulas de explosão
    }
    
    

}
