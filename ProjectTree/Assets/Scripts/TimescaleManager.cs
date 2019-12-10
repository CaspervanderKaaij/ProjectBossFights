using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleManager : MonoBehaviour {
    public bool isPaused = false;
    public float normalScale = 1;
    bool isSlowMotion = false;
    float slowmo = 0.1f;
    public enum State {
        None,
        Paused,
        Options
    }
    public State curState = State.None;
    [SerializeField] string pauseButton = "Pause";
    [Header ("OptionScreen")]
    [SerializeField] GameObject optionScreen;
    GameObject optionSpawned;

    void Start () {
        if (SaveSystem.LoadStuff ().nightcore == true) {
            normalScale = 1.5f;
        }
        Time.timeScale = normalScale;
        isPaused = false;
        slowmo = normalScale;
    }
    void LateUpdate () {
        switch (curState) {
            case State.None:
                //see the else ifs, as a priority system
                if (isPaused == true) {
                    Time.fixedDeltaTime = 100;
                    Time.timeScale = 0;
                } else {
                    Time.fixedDeltaTime = Mathf.Max (0.01f, Time.deltaTime);
                    Time.timeScale = slowmo;
                }
                //SetPaused();
                SetOptioned ();
                break;
            case State.Options:
                isPaused = true;
                Time.timeScale = 0;
                SetUnpaused ();
                break;
            case State.Paused:
                isPaused = true;
                Time.timeScale = 0;
                SetUnpaused ();
                break;
        }
        if (noPauseTime > 0) {
            noPauseTime -= Time.unscaledDeltaTime;
        }
    }

    public void UpdateScale () {
        slowmo = normalScale;
    }

    public void SlowMo (float time, float scale) {
        if (slowmo > scale) {
            StopCoroutine ("SlowMotion");
            StartCoroutine (SlowMotion (time, scale));
        }
    }

    IEnumerator SlowMotion (float time, float scale) {
        slowmo = scale;
        yield return new WaitForSecondsRealtime (time / normalScale);
        slowmo = normalScale;
    }

    public void SlowMo (float time, float scale, float delay) {
        if (slowmo > scale) {
            StopCoroutine ("SlowMotion");
            StartCoroutine (SlowMotion (time, scale, delay));
        }
    }

    IEnumerator SlowMotion (float time, float scale, float delay) {
        yield return new WaitForSecondsRealtime (delay / normalScale);
        slowmo = scale;
        yield return new WaitForSecondsRealtime (time / normalScale);
        slowmo = normalScale;
    }

    float noPauseWait = 0.15f;
    void SetPaused () {
        if (Input.GetButtonDown (pauseButton) == true && noPauseTime <= 0) {
            noPauseTime = noPauseWait;
            curState = State.Paused;
            Destroy (optionSpawned);
        }
    }
    float noPauseTime = 0;

    void SetOptioned () {
        if (Input.GetButtonDown (pauseButton) == true && noPauseTime <= 0) {
            noPauseTime = noPauseWait;
            curState = State.Options;
            optionSpawned = Instantiate (optionScreen);
        }
    }

    void SetUnpaused () {
        if (Input.GetButtonDown (pauseButton) == true && noPauseTime <= 0) {
            noPauseTime = noPauseWait;
            curState = State.None;
            Destroy (optionSpawned);
            isPaused = false;
        }
    }
}