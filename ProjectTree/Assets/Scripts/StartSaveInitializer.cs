using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class StartSaveInitializer : MonoBehaviour {
    PlayerController player;
    SaveStuff data;
    void OnEnable () {
        player = FindObjectOfType<PlayerController> ();
        data = SaveSystem.LoadStuff ();

        //settings
        SetAntiAlias();
        SetQuality();
        SetResolution();
        SetWindowMode();
        //modes
        SetNightcore ();

        DontDestroyOnLoad(gameObject);
        print(Application.persistentDataPath);
    }

    //settings
    void SetResolution () {
        switch (data.resolution) {
            case 0:
                Screen.SetResolution (1280, 720, Screen.fullScreenMode, 60);
                break;
            case 1:
                Screen.SetResolution (1920, 1080, Screen.fullScreenMode, 60);
                break;
            case 2:
                Screen.SetResolution (3840, 2160, Screen.fullScreenMode, 60);
                break;
        }
    }

    void SetWindowMode () {
        if (data.windowMode == true) {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    void SetAntiAlias () {
        if (data.antiAlias == true) {
            Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        } else {
            Camera.main.GetComponent<PostProcessLayer> ().antialiasingMode = PostProcessLayer.Antialiasing.None;
        }
    }

    void SetQuality () {
        QualitySettings.SetQualityLevel (data.quality);
    }

    //modes

    void SetNightcore () {
        if (data.nightcore == true) {
            if (FindObjectOfType<TimescaleManager> () != null) {
                FindObjectOfType<TimescaleManager> ().normalScale = 1.5f;
            }
            AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
            mixer.SetFloat ("nightcore", 1.25f);
        } else {
            if (FindObjectOfType<TimescaleManager> () != null) {
                FindObjectOfType<TimescaleManager> ().normalScale = 1;
            }
            AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
            mixer.SetFloat ("nightcore", 1f);
        }
    }

}