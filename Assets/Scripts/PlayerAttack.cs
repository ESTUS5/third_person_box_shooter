using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform playerTargetTransform;
    private Stats playerStats;
    private Stat normalDamage,chargedDamage,normalSpeed,chargedSpeed,normalCooldown,chargedCooldown;
    void Start()
    {
        playerStats = GetComponent<Stats>();
        
        normalDamage = playerStats.GetStat("NormalDamage");
        normalSpeed = playerStats.GetStat("NormalDamageSpeed");
        normalCooldown = playerStats.GetStat("NormalDamageCooldown");
        
        chargedDamage = playerStats.GetStat("ChargedDamage");
        chargedSpeed = playerStats.GetStat("ChargedDamageSpeed");
        chargedCooldown = playerStats.GetStat("ChargedDamageCooldown");
        
        if(playerTargetTransform == null)
        {
            playerTargetTransform = transform.Find("/Player/PlayerTarget");
        }
    }

    [SerializeField] private GameObject ProjectileNormal;
    [SerializeField] private GameObject ProjectileCharged;
    public float lastNormalShotTime {private set;get;}
    public float lastChargedShotTime {private set;get;}

    void Update()
    {
        lastNormalShotTime -= Time.deltaTime;
        lastChargedShotTime -= Time.deltaTime;
    }

    void OnFireNormal()
    {
        if(lastNormalShotTime < 0)
        {
            lastNormalShotTime = normalCooldown.GetValue();
            Shot(ProjectileNormal,(int)normalDamage.GetValue(),(int)normalSpeed.GetValue());
        }
    }

    void OnFireCharged()
    {
        if(lastChargedShotTime < 0)
        {
            lastChargedShotTime = chargedCooldown.GetValue();
            Shot(ProjectileCharged,(int)chargedDamage.GetValue(),(int)chargedSpeed.GetValue());
        }
    }
    [SerializeField] private Vector3 bulletOriginOffset = new Vector3(0,0.25f,0);
    [SerializeField] private LayerMask bulletLayerMask;
    private float maxDistance = 100f;
    void Shot(GameObject prefab,int projectileDamage,int projectileSpeed)
    {
            GameObject instance = Instantiate(prefab,transform.position + bulletOriginOffset,transform.rotation);
            
            if(!instance.TryGetComponent<Rigidbody>(out Rigidbody _projectileRigidbody))
                instance.AddComponent<Rigidbody>();

            instance.GetComponent<ProjectileScript>().projectileDamage = projectileDamage;

            float x = Screen.width * 0.5f;
            float y = Screen.height * 0.5f;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
            RaycastHit hit;
            Vector3 hitPoint;
            if(Physics.Raycast(ray,out hit,maxDistance,bulletLayerMask))
                hitPoint = hit.point;
            else
                hitPoint = ray.direction;
            instance.GetComponent<Rigidbody>().AddForce((hit.point-instance.transform.position).normalized * projectileSpeed,ForceMode.Impulse);
    }
    
    
}
