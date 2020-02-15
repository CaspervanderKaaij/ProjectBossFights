using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyObjectsEvent : MonoBehaviour {
    [SerializeField] GameObject[] objects;
    [SerializeField] UnityEvent ev;

    void LateUpdate () {
        bool willDoEv = true;
        for (int i = 0; i < objects.Length; i++) {
            if (objects[i] != null) {
                willDoEv = false;
            }
        }

        if (willDoEv == true) {
            Destroy (this);
            ev.Invoke ();
        }
    }

    public void DestroyMe () {
        Destroy (gameObject);
    }
}