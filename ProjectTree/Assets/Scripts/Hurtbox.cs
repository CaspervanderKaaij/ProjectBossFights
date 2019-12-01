using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtbox : MonoBehaviour {
    public int team;
    public float damage;
    [SerializeField] float activeTime = 0;
    public UnityEvent hitEv;
    void OnTriggerEnter (Collider other) {
        if (other.GetComponent<Hitbox> () != null) {
            other.GetComponent<Hitbox> ().GetHit (team, damage,this);
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