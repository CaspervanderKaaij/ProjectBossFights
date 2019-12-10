using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBox : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    void OnTriggerStay(Collider other) {
        if(other.GetComponent<CharacterController>() != null){
            other.GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
        }    
    }
}
