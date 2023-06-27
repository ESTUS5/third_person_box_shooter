using UnityEngine;

[CreateAssetMenu(fileName = "HealthScriptableObject",menuName ="")]
public class HealthScriptableObject : ScriptableObject
{
    public int maxHealth;
    public int currentHealth;
    public float maxSpeed;
}
