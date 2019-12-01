using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Interact : MonoBehaviour {
    public float priority = 0;
    public UnityEvent ev;
    public bool autoActivate = false;
    void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            PlayerController p = other.GetComponent<PlayerController> ();
            if (autoActivate == false) {
                p.interactables.Add (this);
            } else if(p.curState == PlayerController.State.Normal && p.isGrounded == true) {
                Activate(p);
                FindObjectOfType<Dialogue>().firstInput = false;
            } else {
                GetComponent<Collider>().enabled = false;
                GetComponent<Collider>().enabled = true;
            }
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<PlayerController> ().interactables.Remove (this);
        }
    }

    public virtual void Activate (PlayerController player) {
        ev.Invoke ();
        player.anim.SetBool ("grounded", true);
        player.anim.SetFloat ("curSpeed", 0);
        player.SetDashInvisible(false);
        player.cameraTransform.GetComponent<PlayerCam>()._enabled = true;
    }
}