using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    AudioSource source;
    public float volumeGoal = 1;
    public AudioClip normalMusic;
    public AudioClip priorityMusic;
    float normalVolume = 0;

    public AudioClip priorityAudioTest;
    void Start () {
        DontDestroyOnLoad (gameObject);
        source = GetComponent<AudioSource> ();

        //these are the two function to use
        //FadeToNewMusic(newMusic);
        //StopMusic(0.4f);

        normalVolume = 0;
        SetVolumeGoal (1);
        StopCoroutine ("SetVolume");
        StartCoroutine (SetVolume (1, 1));
    }

    void Update () {
        if (priorityMusic == null) {
            if (normalMusic != null) {
                source.volume = normalVolume;
                if (source.clip != normalMusic) {
                    source.clip = normalMusic;
                    source.Play ();
                    normalVolume = 0;
                    StopCoroutine("SetVolume");
                    SetVolumeGoal(1);
                    StartCoroutine(SetVolume(1,1));
                }
            } else {
                source.volume = 0;
                source.clip = null;
            }

        } else {
            source.volume = 1;
            if (source.clip != priorityMusic) {
                source.clip = priorityMusic;
                source.Play ();
            }
        }

        if (Input.GetKeyDown (KeyCode.Alpha2)) {
            if (priorityMusic == null) {
                priorityMusic = priorityAudioTest;
            } else {
                priorityMusic = null;
            }
        }
    }

    public void FadeToNewMusic (AudioClip newMus) {
        NewMusic (0.2f, 0.6f, newMus);
    }

    public void StopMusic (float fadeoutSpeed) {
        SetVolumeGoal (0);
        StopCoroutine ("SetVolume");
        StartCoroutine (SetVolume (fadeoutSpeed, fadeoutSpeed));
        normalMusic = null;
    }

    void NewMusic (float fadeoutSpeed, float fadeInSpeed, AudioClip newMus) {
        SetVolumeGoal (0);
        StopCoroutine ("SetVolume");
        StartCoroutine (SetVolume (fadeInSpeed, fadeoutSpeed));
        StartCoroutine (FadeIn (fadeoutSpeed, fadeInSpeed, newMus));
    }

    IEnumerator FadeIn (float fadeoutSpeed, float fadeInSpeed, AudioClip newMus) {
        while (normalVolume != 0) {
            yield return null;
        }

        normalMusic = newMus;
        source.Play ();
        SetVolumeGoal (1);
        StopCoroutine ("SetVolume");
        StartCoroutine (SetVolume (fadeInSpeed, fadeoutSpeed));
    }

    IEnumerator SetVolume (float upSpeed, float downSpeed) {
        float speed = upSpeed;
        if (normalVolume < volumeGoal) {
            speed = downSpeed;
        }
        normalVolume = Mathf.MoveTowards (normalVolume, volumeGoal, Time.unscaledDeltaTime * speed);
        yield return new WaitForSecondsRealtime (Time.unscaledDeltaTime);
        if (normalVolume != volumeGoal) {
            StartCoroutine (SetVolume (upSpeed, downSpeed));
        }
    }

    void SetVolumeGoal (float f) {
        volumeGoal = f;
    }

}