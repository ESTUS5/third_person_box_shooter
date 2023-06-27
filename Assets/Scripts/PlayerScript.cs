using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private PlayerCameraBehaviour cameraBehaviour;
    void Awake()
    {
        cameraBehaviour = GetComponent<PlayerCameraBehaviour>();
    }

    

    
    void Update()
    {
        cameraBehaviour.MoveCamera();
        
    }

    // void OnDrawGizmos()
    // {
    //     //Gizmos.DrawSphere(SnapHit.point,0.1f);
    //     //Gizmos.DrawSphere(StepHit.point,0.1f);
    //     Gizmos.DrawLine(transform.position,transform.position + Rigidbody.velocity);
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawLine(transform.position,transform.position + MoveDirection*2f);
    //     //Gizmos.DrawLine(transform.position,GroundHit.point);
    // }
}
