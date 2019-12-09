using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtbox : MonoBehaviour {
    public int team;
    public float damage;
    [SerializeField] float activeTime = 0;
    public UnityEvent hitEv;
    public bool destroyOnHit = false;
    void OnTriggerEnter (Collider other) {
        if (other.GetComponent<Hitbox> () != null && this.enabled == true) {
            other.GetComponent<Hitbox> ().GetHit (team, damage, this);
            if (destroyOnHit == true && other.GetComponent<Hitbox> ().team != team) {
            Destroy (gameObject);
        }
        } else if (destroyOnHit == true && this.enabled == true && other.GetComponent<Hurtbox>() == false) {
            Destroy (gameObject);
        }
    }

    void OnCollisionEnter(Collision other) {
       if(other.gameObject.layer == 0 && destroyOnHit == true){
           Destroy(gameObject);
       } 
    }

    void OnEnable () {
        if (activeTime != 0) {
            Invoke ("DisableAgain", activeTime);
        }
    }

    void DisableAgain () {
        if (gameObject.activeSelf == true) {
            gameObject.SetActive (false);
        }
    }
}