using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : LandAction {
    [Header ("Important: for this to work, this object needs an empty parent witch scale 1,1,1")]
    [SerializeField] PlayerController player;

    void Start () {
        player = FindObjectOfType<PlayerController> ();
    }

    public override void Activate () {
        player = FindObjectOfType<PlayerController> ();
        player.angleGoal -= lastY - transform.eulerAngles.y;

        if (player.transform.parent == null) {
            GameObject g = new GameObject ();
            g.transform.position = transform.position;
            g.transform.eulerAngles = transform.eulerAngles;
            g.AddComponent<FixedUpdateParenter> ().parent = transform;
            player.transform.SetParent (g.transform);
        }

    }

    public override void End () {
        GameObject g = player.transform.parent.gameObject;
        player.transform.SetParent (null);
        Destroy (g);

    }

    float lastY;
    void LateUpdate () {
        lastY = transform.eulerAngles.y;
    }

}