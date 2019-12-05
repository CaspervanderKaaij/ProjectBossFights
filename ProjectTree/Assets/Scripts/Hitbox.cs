using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent (typeof (Rigidbody))]

public class Hitbox : MonoBehaviour {
    public uint team = 0;
    public float hp = 100;
    public UnityEvent hitEv;
    public UnityEvent deathEv;
    public GameObject destroyOnDeath;
    public float destroyDelay = 0;

    public void GetHit (int otherTeam, float damage,Hurtbox hBox) {
        if (otherTeam != team && hp > 0) {
            hBox.hitEv.Invoke();
            hp -= damage;
            if (hp > 0) {
                hitEv.Invoke ();
            } else {
                deathEv.Invoke ();
                if (destroyOnDeath != null) {
                    Destroy (destroyOnDeath,destroyDelay);
                }
            }
        }
    }
}