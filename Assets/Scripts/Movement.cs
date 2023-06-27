using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody Rigidbody;
    public Camera MainCamera {private set; get;}
    [SerializeField] public GameObject Target;
    private Stats playerStats;
    [SerializeField] private Stat maxSpeed;
    void Awake()
    {
        if(TryGetComponent<PlayerInput>(out PlayerInput inputComponent))
        {
        }
        else
        {
            Debug.LogError("NO PLAYER INPUT");
        }
        if(TryGetComponent<Rigidbody>(out Rigidbody rigidbodyComponent))
        {
            Rigidbody = rigidbodyComponent;
        }
        else
        {
            Debug.LogError("NO Rigidbody");
        }
        Target = GameObject.Find("/Player/PlayerTarget");
        playerStats = GetComponent<Stats>();
        maxSpeed = playerStats.GetStat("MaxSpeed");
    }
    void Update()
    {
        OnMoveRotatePlayer();
    }
    void FixedUpdate()
    {
        if(IsJumping) return;
        if(CheckGrounded())
        {
            CalculateMove();
            StepOnObstacle();
            DetectEdge();
        }
        else
        {
            SnapToGround();
        }
    }
    #region Ground Raycast
    [Header("Ground Check")]
    [SerializeField] private float DistanceToGround;
    [SerializeField] private LayerMask Mask; 
    private RaycastHit GroundHit;
    [SerializeField] private bool IsGrounded = false;
    bool CheckGrounded()
    {
        return Physics.Raycast(transform.position,(-transform.up),out GroundHit,DistanceToGround,Mask);
    }
    #endregion
    #region  StepDetection
    [Header("Step Detection")]
    [SerializeField] private float FeetHeight = 0.5f;
    [SerializeField] private float StepHeight = 0.25f;
    [SerializeField] private float Step;
    private RaycastHit StepHit,FeetHit;
    [SerializeField] private float DistanceToStep=0.1f;
    [SerializeField] private float StepAngle = 50f;
    [SerializeField] private bool IsSteping = false;
    [SerializeField] private float RotateAngle = 45f;
    private Vector3 stepRay,stepRay2;
    void StepOnObstacle()
    {
        if(MoveInput.sqrMagnitude <= 0f) return;
        Vector3 feetHeight = transform.position + Vector3.down*FeetHeight;
        stepRay = feetHeight;
        if(Physics.Raycast(feetHeight,MoveDirection,out FeetHit,DistanceToStep,Mask))
        {
            //ignore slopes
            if(Vector3.Angle(FeetHit.normal,transform.up) < StepAngle) return;
            //is obstacle climbable
            Vector3 stepHeight = transform.position + Vector3.down*StepHeight;
            stepRay2 = stepHeight;
            if(!Physics.Raycast(stepHeight,MoveDirection,out StepHit,DistanceToStep+0.1f,Mask))
            {
                transform.position -= new Vector3(0,-Step,0);
                IsGrounded = true;
                return;
            }
        }
        // ray x degrees to the left
        Vector3 leftRay = Quaternion.Euler(0, -RotateAngle, 0) * MoveDirection;
        if(Physics.Raycast(feetHeight,leftRay,out FeetHit,DistanceToStep,Mask))
        {
            //ignore slopes
            if(Vector3.Angle(FeetHit.normal,transform.up) < StepAngle) return;
            //is obstacle climbable
            Vector3 stepHeight = transform.position + Vector3.down*StepHeight;
            if(!Physics.Raycast(stepHeight,leftRay,out StepHit,DistanceToStep+0.1f,Mask))
            {
                transform.position -= new Vector3(0,-Step,0);
                IsGrounded = true;
                return;
            }
        } 
        // ray -x degrees to the right
        Vector3 rightRay = Quaternion.Euler(0, RotateAngle, 0) * MoveDirection;
        if(Physics.Raycast(feetHeight,rightRay,out FeetHit,DistanceToStep,Mask))
        {
            //ignore slopes
            if(Vector3.Angle(FeetHit.normal,transform.up) < StepAngle) return;
            //is obstacle climbable
            Vector3 stepHeight = transform.position + Vector3.down*StepHeight;
            if(!Physics.Raycast(stepHeight,rightRay,out StepHit,DistanceToStep+0.1f,Mask))
            {
                transform.position -= new Vector3(0,-Step,0);
                IsGrounded = true;
                return;
            }
        } 
    }
    #endregion


    #region Snap to ground
    [Header("Snap to Ground")]
    [SerializeField] private float SnapDistance = 0.6f;
    [SerializeField] private float SphereSize = 0.1f;
    RaycastHit SnapHit;
    void SnapToGround()
    {

        if(!Physics.SphereCast(transform.position,SphereSize,(-transform.up),out SnapHit,SnapDistance,Mask)) return;
        float SnapForce = SnapHit.distance ;
        Rigidbody.velocity -= Vector3.up * SnapForce;
    }
    #endregion
    #region Edge Detect
    [Header("Edge Detection")]
    [SerializeField] private float EdgeDetectionDistance = 2f;
    void DetectEdge()
    {
        
        Vector3 direction = (MoveDirection - transform.up);
        if(!Physics.Raycast(transform.position,direction,EdgeDetectionDistance)) 
        {
            Rigidbody.velocity = Vector3.zero;
            return;
        }
        Vector3 rightRay = Quaternion.Euler(0, -RotateAngle, 0) * direction;
        if(!Physics.Raycast(transform.position,rightRay,EdgeDetectionDistance))
        {
            Rigidbody.velocity = Vector3.zero;
            return;
        }
        Vector3 leftRay = Quaternion.Euler(0, RotateAngle, 0) * direction;
        if(!Physics.Raycast(transform.position,leftRay,EdgeDetectionDistance))
        {
            Rigidbody.velocity = Vector3.zero;
            return;
        }
    }
    #endregion
    #region Rotate
    void OnMoveRotatePlayer()
    {
        if(MoveInput.sqrMagnitude > 0.1f)
        {
            float rotationY = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, Target.transform.rotation.eulerAngles.y, ref RotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
        }
    }
    #endregion

    #region Move Settings
    [Header("Move Settings")]
    [SerializeField] private Vector2 MoveInput;
    [SerializeField] private Vector3 MoveDirection;
    [SerializeField] private float CurrentSpeed;
    [SerializeField] private float Acceleration = 2f;
    [SerializeField] private float RotationVelocity = 0.5f;
    [SerializeField] private float RotationSmoothTime = 0.5f;
    [SerializeField] private AnimationCurve AccelerationFactor;
    //Move the player
    Vector3 GetMoveDirection()
    {
        //Rotate with target camera
        float angleToRotate = Target.transform.localRotation.eulerAngles.y;
        Vector3 forwardVec = Quaternion.AngleAxis(angleToRotate,Vector3.up) * transform.forward;
        Vector3 rightVec = Quaternion.AngleAxis(angleToRotate,Vector3.up) * transform.right;
        Vector3 moveDirection = forwardVec  * MoveInput.y + rightVec * MoveInput.x;
        //Perpendicular to surface
        return Vector3.ProjectOnPlane(moveDirection,GroundHit.normal).normalized;
    }
    void CalculateMove()
    {
        

        if(MoveInput.sqrMagnitude > 0.1f)
        {
            MoveDirection = GetMoveDirection();

            float targetSpeed = maxSpeed.GetValue(); 
            float speedDifference = targetSpeed - Rigidbody.velocity.magnitude;
            //float acceleration = Mathf.Abs(MoveInput.magnitude) > 0.1f ? Acceleration : Deacceleration;
            
            //Makes turns faster;
            float VelocityDot = Vector3.Dot(Rigidbody.velocity,MoveDirection);
            CurrentSpeed = speedDifference * Acceleration * AccelerationFactor.Evaluate(VelocityDot);
            Rigidbody.velocity += (MoveDirection * CurrentSpeed* Time.deltaTime);
        }
        else
        {
            //Deaccelerate
            Rigidbody.velocity = Rigidbody.velocity * 0.90f;
        }
    }
    // void ApplyForce(Vector3 force)
    // {
    //     Rigidbody.AddForce(force,ForceMode.Force);
    // }
    #endregion
    #region Jump
    [Header("Jump")]
    [SerializeField] private bool IsJumping = false;
    [SerializeField] private float JumpForce = 5f;
    void OnJump()
    {
        if(!IsGrounded) return;
        IsJumping = true;
        Rigidbody.AddForce(MoveDirection + Vector3.up * JumpForce,ForceMode.VelocityChange);
    }
    #endregion
    #region Input settings
    void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }
    #endregion
        #region Collision
    void OnCollisionEnter(Collision other)
    {
        IsGrounded = true;
        IsJumping = false;
        //if(CheckGrounded()) IsJumping = false;
    }
    void OnCollisionStay(Collision other)
    {
        IsGrounded = true;
    }
    void OnCollisionExit(Collision other)
    {
        IsGrounded = false;
    }

    #endregion

    void OnDrawGizmos()
    {
        Debug.DrawRay(stepRay, MoveDirection);
        Debug.DrawRay(stepRay2, MoveDirection);
    }
}
