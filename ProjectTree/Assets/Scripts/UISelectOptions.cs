using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

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

    void AntiAlias () {
        if (Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode != PostProcessLayer.Antialiasing.None) {
            Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode = PostProcessLayer.Antialiasing.None;
        } else {
            Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        }

        SaveStuff data = SaveSystem.LoadStuff ();
        data.antiAlias = (Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode != PostProcessLayer.Antialiasing.None);
        SaveSystem.Save (data);
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

        SaveStuff data = SaveSystem.LoadStuff ();
        data.resolution = curRes;
        SaveSystem.Save (data);
    }

    int curQuality = 0;
    void QualityLevel () {
        curQuality = (int) Mathf.Repeat (curQuality + 1, QualitySettings.names.Length);
        QualitySettings.SetQualityLevel (curQuality);
        qualityText.text = QualitySettings.names[curQuality];

        SaveStuff data = SaveSystem.LoadStuff ();
        data.quality = curQuality;
        SaveSystem.Save (data);
    }

    bool isNightcore = false;
    void Nightcore () {
        isNightcore = !SaveSystem.LoadStuff().nightcore;
        if (isNightcore == true) {
            mixer.SetFloat ("nightcore", 1.25f);
            if (FindObjectOfType<TimescaleManager> () != null) {
                FindObjectOfType<TimescaleManager> ().normalScale = 1.5f;
                FindObjectOfType<TimescaleManager> ().UpdateScale();
            }
        } else {
            mixer.SetFloat ("nightcore", 1);
            if (FindObjectOfType<TimescaleManager> () != null) {
                FindObjectOfType<TimescaleManager> ().normalScale = 1f;
                FindObjectOfType<TimescaleManager> ().UpdateScale();
            }
        }

        SaveStuff data = SaveSystem.LoadStuff ();
        data.nightcore = isNightcore;
        SaveSystem.Save (data);
    }
    void WindowMode () {
        if (Screen.fullScreenMode == FullScreenMode.Windowed) {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        SaveStuff data = SaveSystem.LoadStuff ();
        data.windowMode = (Screen.fullScreenMode == FullScreenMode.Windowed);
        SaveSystem.Save (data);
    }

    void Start () {
        switch (option) {

            case Option.Quality:
                qualityText.text = QualitySettings.names[curQuality];
                break;
            case Option.Resolution:
            curRes = SaveSystem.LoadStuff().resolution;
                switch (curRes) {
                    case 0:
                        resText.text = "720p";
                        break;
                    case 1:
                        resText.text = "1080p";
                        break;
                    case 2:
                        resText.text = "4K";
                        break;
                }
                break;
        }
    }
}