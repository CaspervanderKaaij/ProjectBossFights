using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpawnAudio : MonoBehaviour {
    public static void AudioSpawn (AudioClip clip, float startTime, float pitch, float volume) {
        GameObject g = new GameObject (clip.name);
        AudioSource s = g.AddComponent<AudioSource> ();
        s.clip = clip;
        s.pitch = pitch;
        s.time = startTime;
        s.volume = volume;
        AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
        s.outputAudioMixerGroup = mixer.FindMatchingGroups ("SFX") [0];
        s.Play ();
        Destroy (g, clip.length);

        if (FindObjectOfType<TimescaleManager> () != null) {
            s.pitch *= FindObjectOfType<TimescaleManager> ().normalScale;
        }
    }

    public static void AudioSpawn (AudioClip clip, float startTime, float pitch, float volume,float delay) {
        GameObject g = new GameObject (clip.name);
        AudioSource s = g.AddComponent<AudioSource> ();
        s.clip = clip;
        s.pitch = pitch;
        s.time = startTime;
        s.volume = volume;
        AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
        s.outputAudioMixerGroup = mixer.FindMatchingGroups ("SFX") [0];
        s.PlayDelayed (delay);
        Destroy (g, clip.length);

        if (FindObjectOfType<TimescaleManager> () != null) {
            s.pitch *= FindObjectOfType<TimescaleManager> ().normalScale;
        }
    }

    public static void AudioSpawn (AudioClip[] clip, float startTime, float pitch, float volume) {
        for (int i = 0; i < clip.Length; i++) {
            GameObject g = new GameObject ("audioSFX");
            AudioSource s = g.AddComponent<AudioSource> ();
            s.clip = clip[i];
            s.pitch = pitch;
            s.time = startTime;
            s.volume = volume;
            AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
            s.outputAudioMixerGroup = mixer.FindMatchingGroups ("SFX") [0];
            s.Play ();
            Destroy (g, clip[i].length);

            if (FindObjectOfType<TimescaleManager> () != null) {
                s.pitch *= FindObjectOfType<TimescaleManager> ().normalScale;
            }
        }
    }

    public static GameObject SpawnVoice (AudioClip clip, float startTime, float pitch, float volume, float delay) {
        GameObject g = new GameObject (clip.name);
        AudioSource s = g.AddComponent<AudioSource> ();
        s.priority = 200;
        s.clip = clip;
        s.pitch = pitch;
        s.time = startTime;
        s.volume = volume;
        AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
        s.outputAudioMixerGroup = mixer.FindMatchingGroups ("Voice") [0];
        Destroy (g, clip.length + delay);

        if (FindObjectOfType<TimescaleManager> () != null) {
            s.pitch *= FindObjectOfType<TimescaleManager> ().normalScale;
            s.PlayDelayed (delay / FindObjectOfType<TimescaleManager> ().normalScale);
        } else {
        s.PlayDelayed (delay);
        }
        return g;
    }
}