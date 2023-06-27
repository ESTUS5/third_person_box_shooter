using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [SerializeField] Vector3 StartPosition,EndPosition;
    [SerializeField] bool Perpetuate = true;
    [SerializeField] private float CurrentStep = 0,LastStep = 1;
    [SerializeField] private float LerpSpeed = 1f;
    [SerializeField]private AnimationCurve curve;
   
    void FixedUpdate()
    {
        if(CurrentStep == LastStep && Perpetuate)
        {
            LastStep = LastStep == 0 ? 1 : 0;
        }
        
        CurrentStep = Mathf.MoveTowards(CurrentStep,LastStep,LerpSpeed*Time.deltaTime);
        transform.position = Vector3.Lerp(StartPosition,EndPosition,curve.Evaluate(CurrentStep));
    }

    private void OnCollisionEnter(Collision other) {
        if(other.collider.tag == "Player")
        {
            other.transform.parent = transform;
        }
    }
    private void OnCollisionExit(Collision other) {
        if(other.collider.tag == "Player")
        {
            other.transform.parent = null;
        }
    }
}
