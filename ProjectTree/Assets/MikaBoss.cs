using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikaBoss : MonoBehaviour {
    Collider myHitbox;
    [SerializeField] Transform barrierPointsParent;
    void Start () {
        myHitbox = GetComponent<Collider>();
    }

    void Update () {
        UpdateBarrierActive();
    }

    void UpdateBarrierActive () {
        SetHitboxActive ((barrierPointsParent.childCount <= 0));
    }

    void SetHitboxActive (bool active) {
        bool wasActive = myHitbox.enabled;

        if (active == true && wasActive == false) {
            myHitbox.enabled = active;
        }

        if (active == false && wasActive == true) {
            myHitbox.enabled = active;
        }
    }
}