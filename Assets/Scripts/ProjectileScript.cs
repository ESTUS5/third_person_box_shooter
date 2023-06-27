using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    private HealthScript hitObjectHealth;
    public int projectileDamage;
    void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag != transform.tag)
        {
            
        }
        if(other.gameObject.TryGetComponent<HealthScript>(out HealthScript hitObjectHealth))
                hitObjectHealth.TakeDamge(projectileDamage);
        Debug.Log("Hit");
        Destroy(gameObject);
    }
}