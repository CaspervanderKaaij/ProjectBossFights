using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikaSnekwurm : MonoBehaviour {
    [SerializeField] MikaSnekwurmPart[] parts;
    [SerializeField] float partSpeed = 10;
    List<Vector3> playerPosses = new List<Vector3> ();
    Transform player;
    [SerializeField] int delay = 50;
    void Start () {
        player = FindObjectOfType<PlayerController> ().transform;
    }

    void FixedUpdate () {
        Move ();
        UpdateParts ();
    }

    void Move () {
        playerPosses.Add (player.position + new Vector3 (0, 1.5f, 0));
        if (playerPosses.Count > delay) {
            transform.position = playerPosses[0];
            if (Vector3.Distance (playerPosses[0], playerPosses[1]) > 0.01f) {
                transform.LookAt (transform.position + (playerPosses[0] - playerPosses[1]));
                transform.Rotate (-90, 0, 0);
            }

            playerPosses.RemoveAt (0);
        }

    }

    void UpdateParts () {
        if (playerPosses.Count > 2) {
            for (int i = 0; i < parts.Length; i++) {
                parts[i].UpdateMe (partSpeed);
            }
        }
    }
}