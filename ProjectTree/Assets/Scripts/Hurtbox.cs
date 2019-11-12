using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public int team;
    public float damage;
    void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Hitbox>() != null){
           other.GetComponent<Hitbox>().GetHit(team,damage);
        }    
    }
}
