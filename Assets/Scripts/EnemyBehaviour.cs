using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Color FullHealth = new Vector4(0.3802f,1,0,1);
    [SerializeField] private Color LowHealth = new Vector4(1,0.1001f,0,1);

    [SerializeField] private HealthScript Health;
    [SerializeField] private Material Material;
    [SerializeField] private ProjectileScriptableObject Projectile;
    [SerializeField] private Rigidbody rigidbody;
    private float _currentHealthColor;
    private Stats stat;
    private float maxHealth;
    private float maxSpeed;

    void Start()
    {
        stat = GetComponent<Stats>();
        maxHealth = stat.GetStat("MaxHealth").GetValue();
        maxSpeed = stat.GetStat("MaxSpeed").GetValue();
        Health = GetComponent<HealthScript>();
        Material = GetComponent<Renderer>().material;
        rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        Wander();
        if(AttackTarget == null)
        {
            Detect();
        }
    }
    void Update()
    {
        
        RotateAtTarget();
        Timer();
        if(AttackTarget != null)
        {
            if(CooldownTime < 0)
            {
                Attack(AttackTarget.position);
            }
        }
        ChangeMaterialColor();
    }
    void Timer()
    {
        CooldownTime -= Time.deltaTime;
    }
    void RotateAtTarget()
    {
        if(AttackTarget != null)
        {
            transform.LookAt(AttackTarget,Vector3.up);
            return;
        }
        if(newTarget != null)
        {
            transform.LookAt(newTarget + transform.position,Vector3.up);
        }
    }
    [SerializeField] private LayerMask GroundMask;
    float randomZ;
    float randomX;
    public float TimeTillNextTarget = 2f;
    public float NewTargetCooldown = 2f;
    public float DistanceWander = 1f;
    public Vector3 newTarget;
    [SerializeField]private float acceleration = 1.4f;
    

    void Wander()
    {
        if(maxSpeed <= 0) return;
        if(newTarget == transform.position || TimeTillNextTarget < 0)
        {
            Vector3 distanceToTarget = transform.forward * DistanceWander;
            newTarget = Quaternion.Euler(0,Random.Range(0,360),0) * transform.forward * 4f + distanceToTarget;
            TimeTillNextTarget = Random.value * 5f;
        }
        else
        {
            TimeTillNextTarget -= Time.deltaTime;
        }

        float targetSpeed = maxSpeed; 
        float speedDifference = targetSpeed - rigidbody.velocity.magnitude;
        Vector3 MoveDirection = newTarget.normalized;
            //Perpendicular to surface
        RaycastHit GroundHit;
        Physics.Raycast(transform.position,(-transform.up),out GroundHit,1f,GroundMask);
        MoveDirection = Vector3.ProjectOnPlane(MoveDirection,GroundHit.normal).normalized;

        //rigidbody.AddForce(MoveDirection * maxSpeed,ForceMode.Force);
        
            
            //float acceleration = Mathf.Abs(MoveInput.magnitude) > 0.1f ? Acceleration : Deacceleration;
            
            //Makes turns faster;
        float currentSpeed = speedDifference * acceleration;
        rigidbody.velocity += (MoveDirection * currentSpeed* Time.deltaTime);
        Debug.Log(rigidbody.velocity.magnitude);
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity,maxSpeed);
        
    }
    [Header("Attack")]
    [SerializeField] private Transform AttackTarget;
    [SerializeField] private float CooldownTime;
    void Attack(Vector3 attackTargetposition)
    {
        GameObject instance = Instantiate(Projectile.prefab,transform.position,transform.rotation);
            
        if(!instance.TryGetComponent<Rigidbody>(out Rigidbody _projectileRigidbody))
            instance.AddComponent<Rigidbody>();
            
        instance.GetComponent<ProjectileScript>().projectileDamage = (int)Projectile.Damage;

        Vector3 direction = attackTargetposition - transform.position;
        instance.GetComponent<Rigidbody>().AddForce(direction.normalized * Projectile.ProjectileSpeed,ForceMode.Impulse);
        
        CooldownTime = Projectile.Cooldown;
    }
    [Header("Detect")]
    [SerializeField] private int NumberOfRays = 15;
    [SerializeField] private float DetectionAngle = 60f;
    [SerializeField] private float DetectionDistance = 5f;
    [SerializeField] private LayerMask Mask;
    void Detect()
    {
        float angleBetweenRays = DetectionAngle / NumberOfRays;
        Vector3 currentRay = Quaternion.Euler(0,-DetectionAngle*0.5f,0) * transform.forward;
        for(int i = 0; i < NumberOfRays; i++) {
            Debug.DrawLine(transform.position,currentRay*DetectionDistance+ transform.position);
            if(Physics.Raycast(transform.position,currentRay,out RaycastHit targetHit,DetectionDistance,Mask))
            {
                Debug.DrawLine(transform.position,targetHit.point);
                if(targetHit.transform.tag == "Player")
                {
                    Debug.Log("Found Target");
                    AttackTarget = targetHit.rigidbody.transform;
                    Debug.Log(AttackTarget);
                }
            }
            currentRay = Quaternion.Euler(0,angleBetweenRays,0) * currentRay;
        }
    }
    void ChangeMaterialColor()
    {
        _currentHealthColor = (maxHealth - Health.currentHealth) / maxHealth; // or * 0.002 if health is 500 and wont be changed;
        Material.color = Color.Lerp(FullHealth,LowHealth,_currentHealthColor);
    }
    private Vector3 collided;
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "PlayerProjectile")
        {
            
            Vector3 direction = new Vector3(other.transform.position.x,transform.position.y,other.transform.position.z);
            //newTarget = direction;
            newTarget = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
        }
    }
    void OnDrawGizmos()
    {
        
        Gizmos.DrawSphere(collided,0.2f);
    }
        
}