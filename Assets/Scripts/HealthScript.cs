using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
    public bool damageTaken {get; private set;}
    public float currentHealth {get; private set;}
    private float maxHealth;
    private Stats playerStat;
    void Start()
    {
        playerStat = GetComponent<Stats>();
        maxHealth = playerStat.GetStat("MaxHealth").GetValue();
        currentHealth = maxHealth;
    }
    void Update()
    {
        if(maxHealth != playerStat.GetStat("MaxHealth").GetValue())
        {
            currentHealth += playerStat.GetStat("MaxHealth").GetValue()-maxHealth;
            maxHealth = playerStat.GetStat("MaxHealth").GetValue();
        }
        currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void TakeDamge(int damage)
    {
        currentHealth -= damage;
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
    }
}
