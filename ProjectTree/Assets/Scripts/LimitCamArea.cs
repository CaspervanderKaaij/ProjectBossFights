using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitCamArea : MonoBehaviour {
    PlayerCam cam;
    [HideInInspector] public Collider col;
    void Start () {
        cam = FindObjectOfType<PlayerCam> ();
        col = GetComponent<Collider>();
    }
    void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            cam.limiter = this;
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.tag == "Player" && cam.limiter == this) {
            cam.limiter = null;
        }
    }
}