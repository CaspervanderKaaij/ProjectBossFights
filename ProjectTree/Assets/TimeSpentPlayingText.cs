using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpentPlayingText : MonoBehaviour {
    Text txt;
    float savedTime = 0;
    void Start () {
        txt = GetComponent<Text> ();
        savedTime = SaveSystem.LoadStuff ().timeSpentPlaying;
    }

    void Update () {
        float totalTime = Time.unscaledTime + savedTime;
        int hours = (int)((totalTime/3600)%24);
        string minSec = string.Format("{0}:{1:00}", (int)totalTime / 60, (int)totalTime % 60);
        txt.text = hours + ":" + minSec;
    }
}