using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            other.gameObject.GetComponent<HealthScript>().Heal(100);
            Destroy(gameObject);
        }
    }
}
