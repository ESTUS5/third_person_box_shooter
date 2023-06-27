using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    [SerializeField] private Color FullHealth = new Vector4(0.3802f,1,0,1);
    [SerializeField] private Color LowHealth = new Vector4(1,0.1001f,0,1);

    [SerializeField] private HealthScript Health;
    [SerializeField] private Material Material;
    [SerializeField] private ProjectileScriptableObject Projectile;
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private HealthScriptableObject NpcStats;
    private float _currentHealthColor;
    private Stats stat;
    private float maxHealth;
    void Start()
    {
        stat = GetComponent<Stats>();
        maxHealth = stat.GetStat("MaxHealth").GetValue();
        Health = GetComponent<HealthScript>();
        Material = GetComponent<Renderer>().material;
    }
    void LateUpdate()
    {
        Wander();
        RotateAtTarget();
        Timer();
        if(AttackTarget != null)
        {
            if(CooldownTime < 0)
            {
                Attack(AttackTarget.position);
            }
        }
        else
        {
            Detect();
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
    public float DistanceWander = 3f;
    public Vector3 newTarget;
    

    void Wander()
    {
        if(NpcStats.maxSpeed <= 0) return;
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

        //var angleToRotate = Target.transform.localRotation.eulerAngles.y;
            //var forwardVec = Quaternion.AngleAxis(angleToRotate,Vector3.up) * transform.forward;
            //var rightVec = Quaternion.AngleAxis(angleToRotate,Vector3.up) * transform.right;
            //MoveDirection = forwardVec  * MoveInput.y + rightVec * MoveInput.x;
        Vector3 MoveDirection = newTarget.normalized;
            //Perpendicular to surface
        RaycastHit GroundHit;
        Physics.Raycast(transform.position,(-transform.up),out GroundHit,1f,GroundMask);
        MoveDirection = Vector3.ProjectOnPlane(MoveDirection,GroundHit.normal).normalized;
            float targetSpeed = MoveDirection.magnitude * NpcStats.maxSpeed;
            
            //float acceleration = Mathf.Abs(MoveInput.magnitude) > 0.1f ? Acceleration : Deacceleration;
            //float VelocityDot = Vector3.Dot(Rigidbody.velocity,MoveDirection);
            //float CurrentHorizontalSpeed = speedDifference * Acceleration * AccelerationFactor.Evaluate(VelocityDot);
        //Rigidbody.velocity += (MoveDirection * speedDifference * Time.deltaTime);
        Rigidbody.AddForce(MoveDirection * NpcStats.maxSpeed,ForceMode.Force);
        Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity,NpcStats.maxSpeed);
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
