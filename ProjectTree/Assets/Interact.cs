using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Interact : MonoBehaviour {
    public float priority = 0;
    public UnityEvent ev;
    void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<PlayerController>().interactables.Add(this);
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<PlayerController>().interactables.Remove(this);
        }
    }

    public virtual void Activate(PlayerController player){
        ev.Invoke();
    }
}