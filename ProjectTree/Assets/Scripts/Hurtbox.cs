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
        Hitbox hBox = other.GetComponent<Hitbox> ();
        if (hBox != null && this.enabled == true) {
            hBox.GetHit (team, damage, this);
            if (destroyOnHit == true && hBox.team != team && hBox.IsFriend(team) == false) {
                Destroy (gameObject);
            }
        } else if (destroyOnHit == true && this.enabled == true && other.GetComponent<Hurtbox> () == false && other.gameObject.layer == 0) {
            Destroy (gameObject);
        }
    }

    void OnCollisionEnter (Collision other) {
        if (other.gameObject.layer == 0 && destroyOnHit == true) {
            Destroy (gameObject);
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