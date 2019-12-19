using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActiveOnPriorityMusic : MonoBehaviour {
    MusicManager manager;
    public UnityEvent trueEv;
    public UnityEvent falseEv;
    bool wasTrue = false;
    [SerializeField] bool doSetActive = false;
    void Start () {
        manager = FindObjectOfType<MusicManager> ();
        if (manager != null) {
            if (manager.priorityMusic == null) {
                wasTrue = true;
            } else {
                wasTrue = false;
            }
            InvokeRepeating ("Updator", 0, 0.2f);
        } else {
            Destroy (this);
        }
    }

    void Updator () {
        if (wasTrue != (manager.priorityMusic != null)) {
            if (doSetActive == true) {
                gameObject.SetActive ((manager.priorityMusic != null));
            }
            if (wasTrue == true) {
                falseEv.Invoke ();
            } else {
                trueEv.Invoke ();
            }
            wasTrue = (bool) (manager.priorityMusic != null);
        }
    }
}