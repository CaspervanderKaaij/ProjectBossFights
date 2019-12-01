using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UISelectOptions : UISelect {
    public enum Option {
        Resolution,
        WindowMode,
        AntiAlias,
        Quality,
        Nightcore
    }
    public Option option = Option.AntiAlias;
    [ConditionalField ("option", false, Option.Resolution)] public Text resText;
    [ConditionalField ("option", false, Option.Quality)] public Text qualityText;
     [ConditionalField ("option", false, Option.Nightcore)] public AudioMixer mixer;
    public override void Activate () {
        switch (option) {
            case Option.AntiAlias:
                AntiAlias ();
                break;
            case Option.Nightcore:
                Nightcore ();
                break;
            case Option.Quality:
                QualityLevel ();
                break;
            case Option.Resolution:
                Res ();
                break;
            case Option.WindowMode:
                WindowMode ();
                break;
        }
    }

    void Start () {
        if (option == Option.Resolution) {
            curRes = (int) Mathf.Repeat ((float) curRes - 1, 3f);
            Res ();
        }
        if (option == Option.Quality) {
            curQuality = QualitySettings.GetQualityLevel ();
            qualityText.text = QualitySettings.names[curQuality];
        }
    }

    void AntiAlias () {
        if (Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode != PostProcessLayer.Antialiasing.None) {
            Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode = PostProcessLayer.Antialiasing.None;
        } else {
            Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        }
    }

    int curRes = 0; //for saving, set 0 to the saved resolution id int.
    void Res () {
        curRes = (int) Mathf.Repeat ((float) curRes + 1, 3f);
        switch (curRes) {
            case 0:
                Screen.SetResolution (1280, 720, Screen.fullScreenMode, 60);
                resText.text = "720p";
                break;
            case 1:
                Screen.SetResolution (1920, 1080, Screen.fullScreenMode, 60);
                resText.text = "1080p";
                break;
            case 2:
                Screen.SetResolution (3840, 2160, Screen.fullScreenMode, 60);
                resText.text = "4K";
                break;
        }
    }

    int curQuality = 0;
    void QualityLevel () {
        curQuality = (int) Mathf.Repeat (curQuality + 1, QualitySettings.names.Length);
        QualitySettings.SetQualityLevel (curQuality);
        qualityText.text = QualitySettings.names[curQuality];
    }

    bool isNightcore = false;
    void Nightcore () {
        isNightcore = !isNightcore;
        if(isNightcore == true){
            mixer.SetFloat("nightcore",1.25f);
            if(FindObjectOfType<TimescaleManager>() != null){
                 FindObjectOfType<TimescaleManager> ().normalScale = 1.5f;
            }
        } else {
            mixer.SetFloat("nightcore",1);
            if(FindObjectOfType<TimescaleManager>() != null){
                 FindObjectOfType<TimescaleManager> ().normalScale = 1f;
            }
        }
    }
    void WindowMode () {
        if (Screen.fullScreenMode == FullScreenMode.Windowed) {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}