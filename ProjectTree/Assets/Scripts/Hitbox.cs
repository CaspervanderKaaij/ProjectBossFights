using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent (typeof (Rigidbody))]

public class Hitbox : MonoBehaviour {
    public uint team = 0;
    [SerializeField] uint[] friends;
    public float hp = 100;
    public UnityEvent hitEv;
    public UnityEvent deathEv;
    public GameObject destroyOnDeath;
    public float destroyDelay = 0;

    public void GetHit (int otherTeam, float damage, Hurtbox hBox) {
        if (otherTeam != team && hp > 0 && this.enabled == true && IsInvoking ("Invincible") == false && hBox.damage != 0 && IsFriend (otherTeam) == false) {
            hBox.hitEv.Invoke ();
            hp -= damage;
            Invoke ("Invincible", 0);
            if (hp > 0) {
                hitEv.Invoke ();
            } else {
                deathEv.Invoke ();
                if (destroyOnDeath != null) {
                    Destroy (destroyOnDeath, destroyDelay);
                }
            }
        }
    }

    public bool IsFriend (int otherTeam) {
        bool toReturn = false;
        if (friends != null) {
            for (int i = 0; i < friends.Length; i++) {
                if (friends[i] == otherTeam) {
                    toReturn = true;
                }
            }
        }
        return toReturn;
    }

    void Invincible () {

    }
}