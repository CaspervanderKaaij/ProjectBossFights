using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour {
    public Transform player;
    [Header ("Cam Stats")]
    public Vector3 offset = Vector3.zero;
    public float dontMoveDistance = 2;
    public float camSpeed = 5;
    void Start () {
        goalPos = player.position + offset;
        transform.position = goalPos;
    }

    void Update () {
        NormalCam();
    }

    Vector3 goalPos = Vector3.zero;
    void NormalCam () {
        float realSpeed = camSpeed / 10;
        realSpeed  *= Vector3.Distance (player.position, goalPos - offset) * 10;
        if (Vector3.Distance (player.position, goalPos - offset) > dontMoveDistance) {
            goalPos = Vector3.MoveTowards(goalPos,player.position + offset,Time.deltaTime * realSpeed);
        }
        transform.position = Vector3.Lerp(transform.position, goalPos,Time.deltaTime * realSpeed);
    }
}