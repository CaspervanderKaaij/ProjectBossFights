using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {

    bool isShaking = false;
    float shakestr = 0.5f;
    [SerializeField] float shakeScale = 1;
    Vector3 rngRemover;
    [SerializeField] bool usePosition = false;

    void FixedUpdate () {
        if (shakeScale > 0) {
            if (isShaking == true) {
                Shaking (shakestr);
            }

        }
    }

    void StartShake (float time, float strength) {
        CancelInvoke ("StopShake");
        isShaking = true;
        shakestr = strength;
        Invoke ("StopShake", time);
        if (usePosition == true) {
            transform.localPosition -= rngRemover;
        } else {
            transform.localEulerAngles -= rngRemover;
        }
        rngRemover = Vector3.zero;
    }

    public void SmallShake () {
        StartShake (0.15f, 0.25f);
    }

    public void MediumShake () {
        StartShake (0.2f, 0.5f);
    }

    public void HardShake () {
        StartShake (0.3f, 1f);
    }

    public void CustomShake (float time, float strength) {
        StartShake (time, strength);
    }

    void StopShake () {
        isShaking = false;
        if (usePosition == true) {
            transform.localPosition -= rngRemover;
        } else {
            transform.localEulerAngles -= rngRemover;
        }
        rngRemover = Vector3.zero;
    }

    void Shaking (float str) {
        if (usePosition == false) {
            transform.eulerAngles -= rngRemover;
            str *= shakeScale;
            rngRemover = new Vector3 (Random.Range (-str, str), Random.Range (-str, str), Random.Range (-str, str));
            transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x + rngRemover.x, transform.localEulerAngles.y + rngRemover.y, rngRemover.z);
        } else {
            transform.position -= rngRemover;
            str *= shakeScale;
            rngRemover = new Vector3 (Random.Range (-str, str), Random.Range (-str, str), Random.Range (-str, str));
            transform.localPosition = new Vector3 (transform.localPosition.x + rngRemover.x, transform.localPosition.y + rngRemover.y, rngRemover.z);
        }
    }
}