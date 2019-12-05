using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public enum State {
        Damage,
        Deflectable
    }
    public State curState = State.Damage;
    [SerializeField] Vector3 moveV3;
    [SerializeField] bool destroyOnHit;
    PlayerController player;
    Hurtbox hBox;
    SphereCollider col;
    float baseRadius;
    void Start () {
        player = FindObjectOfType<PlayerController> ();
        hBox = GetComponent<Hurtbox> ();
        hBox.destroyOnHit = destroyOnHit;
        col = GetComponent<SphereCollider> ();
        baseRadius = col.radius;
    }

    void Update () {
        SetState ();
        Move ();
    }

    void Move () {
        transform.Translate (moveV3 * Time.deltaTime);
    }

    void SetState () {
          if (player.curState != PlayerController.State.Parry) {
            curState = State.Damage;
            hBox.enabled = true;
            col.radius = baseRadius;
        } else {
            curState = State.Deflectable;
            hBox.enabled = false;
            col.radius = baseRadius * 4;
        }
    }

    void OnTriggerEnter (Collider other) {
        if (other.tag == "Player" && curState == State.Deflectable) {
            hBox.enabled = false;
            transform.forward = player.transform.forward;
            hBox.team = 0;
            moveV3 *= 2;
            player.ParrySuccess();
        }
    }
}