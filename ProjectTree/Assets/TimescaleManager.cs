using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleManager : MonoBehaviour {
    public bool isPaused = false;
    public float normalScale = 1;
    bool isSlowMotion = false;
    float slowmo = 0.1f;

    void Start () {
        Time.timeScale = normalScale;
        isPaused = false;
        slowmo = normalScale;
    }
    void LateUpdate () {
        //see the else ifs, as a priority system
        if (isPaused == true) {
            Time.fixedDeltaTime = 100;
            Time.timeScale = 0;
        } else {
            Time.fixedDeltaTime = Mathf.Max(0.01f, Time.deltaTime);
            Time.timeScale = slowmo;
        }
    }

    public void SlowMo (float time, float scale) {
        if (slowmo > scale) {
            StopCoroutine ("SlowMotion");
            StartCoroutine (SlowMotion (time, scale));
        }
    }

    IEnumerator SlowMotion (float time, float scale) {
        slowmo = scale;
        yield return new WaitForSecondsRealtime (time);
        slowmo = normalScale;
    }
}