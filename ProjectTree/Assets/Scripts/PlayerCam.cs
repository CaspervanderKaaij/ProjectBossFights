using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour {
    public Transform player;
    [Header ("Cam Stats")]
    public Vector3 offset = Vector3.zero;
    public Vector3 angleGoal;
    public float dontMoveDistance = 2;
    public float camSpeed = 5;
    public bool _enabled = true;
    float shakeSTR = 0;
    [HideInInspector] public RippleEffect ripple;
    void Start () {
        goalPos = player.position + offset;
        transform.position = goalPos;
        ripple = GetComponent<RippleEffect>();
        angleGoal = transform.eulerAngles;
    }

    void LateUpdate () {
        if (_enabled == true) {
            NormalCam ();
        }
        if (IsInvoking ("StopShake") == true) {
            Shake ();
        }
    }

    Vector3 goalPos = Vector3.zero;
    void NormalCam () {
        float realSpeed = camSpeed / 10;
        realSpeed *= Vector3.Distance (player.position, goalPos - offset) * 10;
        if (Vector3.Distance (player.position, goalPos - offset) > dontMoveDistance) {
            goalPos = Vector3.MoveTowards (goalPos, player.position + offset, Time.deltaTime * realSpeed);
        } else {
            goalPos = Vector3.MoveTowards (goalPos, player.position + offset, Time.deltaTime * realSpeed / 30);
        }
        transform.position = Vector3.Lerp (transform.position, goalPos, Time.deltaTime * realSpeed);


        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(angleGoal),Time.deltaTime * 5);
    }

    void Shake () {
        float str = shakeSTR;
        Vector3 rng = new Vector3 (Random.Range (-str, str), Random.Range (-str, str), Random.Range (-str, str));
        transform.position += rng * Time.deltaTime;
    }

    public void SmallShake (float time) {
        CustomShake (time, 1);
    }
    public void MediumShake (float time) {
        CustomShake (time, 10);
    }
    public void HardShake (float time) {
        CustomShake (time, 15);
    }

    public void CustomShake (float time, float strength) {
        if (shakeSTR < strength) {
            shakeSTR = strength;
            CancelInvoke ("StopShake");
            Invoke ("StopShake", time);
        }
    }

    void StopShake () {
        shakeSTR = 0;
    }
}