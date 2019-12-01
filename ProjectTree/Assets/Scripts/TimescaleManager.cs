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
                SetOptioned();
                break;
            case State.Options:
                isPaused = true;
                Time.timeScale = 0;
                SetUnpaused();
                break;
            case State.Paused:
                isPaused = true;
                Time.timeScale = 0;
                SetUnpaused();
                break;
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
        yield return new WaitForSecondsRealtime (time / normalScale);
        slowmo = normalScale;
    }

    void SetPaused(){
        if(Input.GetButtonDown(pauseButton) == true){
            curState = State.Paused;
            Destroy(optionSpawned);
        }
    }

     void SetOptioned(){
        if(Input.GetButtonDown(pauseButton) == true){
            curState = State.Options;
            optionSpawned = Instantiate(optionScreen);
        }
    }

    void SetUnpaused(){
        if(Input.GetButtonDown(pauseButton) == true){
            curState = State.None;
            Destroy(optionSpawned);
            isPaused = false;
        }
    }
}