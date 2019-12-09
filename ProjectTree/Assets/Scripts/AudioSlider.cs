using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioSlider : MonoBehaviour {
    Slider s;
    [SerializeField] AudioMixer mixer;
    public string valueToChange = "masterVolume";

    void Start () {
        s = GetComponent<Slider> ();

        SaveStuff stuff = SaveSystem.LoadStuff ();
        switch (valueToChange) {
            case "masterVolume":
                s.value = stuff.mainVolume;
                break;
            case "sfxVolume":
                s.value = stuff.sfxVolume;
                break;
            case "voiceVolume":
                s.value = stuff.voiceVolume;
                break;
            case "musicVolume":
                s.value = stuff.musicVolume;
                break;
        }

        UpdateMe();
    }
    public void UpdateMe () {
        if (s.value > 0.5f) {
            mixer.SetFloat (valueToChange, Mathf.Log ((float) s.value / 10f) * 20);
        } else {
            mixer.SetFloat (valueToChange, -80);
        }
        SaveStuff stuff = SaveSystem.LoadStuff ();
        //stuff.mainVolume = s.value;
        switch (valueToChange) {
            case "masterVolume":
                stuff.mainVolume = s.value;
                break;
            case "sfxVolume":
                stuff.sfxVolume = s.value;
                break;
            case "voiceVolume":
                stuff.voiceVolume = s.value;
                break;
            case "musicVolume":
                stuff.musicVolume = s.value;
                break;
        }
        SaveSystem.Save (stuff);
    }
}