using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpawnAudio : MonoBehaviour {
    public static void AudioSpawn (AudioClip clip, float startTime, float pitch, float volume) {
        GameObject g = new GameObject ();
        AudioSource s = g.AddComponent<AudioSource> ();
        s.clip = clip;
        s.pitch = pitch;
        s.time = startTime;
        s.volume = volume;
        AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
        s.outputAudioMixerGroup = mixer.FindMatchingGroups ("SFX") [0];
        s.Play ();
        Destroy (g, clip.length);
    }

    public static void AudioSpawn (AudioClip[] clip, float startTime, float pitch, float volume) {
        for (int i = 0; i < clip.Length; i++) {
            GameObject g = new GameObject ();
            AudioSource s = g.AddComponent<AudioSource> ();
            s.clip = clip[i];
            s.pitch = pitch;
            s.time = startTime;
            s.volume = volume;
            AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
            s.outputAudioMixerGroup = mixer.FindMatchingGroups ("SFX") [0];
            s.Play ();
            Destroy (g, clip[i].length);
        }
    }

    public static void SpawnVoice(AudioClip clip, float startTime, float pitch, float volume,float delay){
        GameObject g = new GameObject ();
        AudioSource s = g.AddComponent<AudioSource> ();
        s.clip = clip;
        s.pitch = pitch;
        s.time = startTime;
        s.volume = volume;
        AudioMixer mixer = Resources.Load ("MainVolume") as AudioMixer;
        s.outputAudioMixerGroup = mixer.FindMatchingGroups ("Voice") [0];
        s.PlayDelayed (delay);
        Destroy (g, clip.length + delay);
    }
}