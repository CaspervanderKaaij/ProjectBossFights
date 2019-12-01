using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioSlider : MonoBehaviour {
    Slider s;
    [SerializeField] AudioMixer mixer;
    [SerializeField] string valueToChange = "masterVolume";

    void Start () {
        s = GetComponent<Slider> ();
    }
    public void UpdateMe () {
        if (s.value > 0.5f) {
            mixer.SetFloat (valueToChange, Mathf.Log ((float) s.value / 10f) * 20);
        } else {
            mixer.SetFloat (valueToChange, -80);

        }
    }
}