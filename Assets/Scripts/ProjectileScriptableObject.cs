using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject",menuName ="")]
public class ProjectileScriptableObject : ScriptableObject
{
    // Start is called before the first frame update

    public GameObject prefab;
    public float Damage,ProjectileSpeed,Cooldown;
}
