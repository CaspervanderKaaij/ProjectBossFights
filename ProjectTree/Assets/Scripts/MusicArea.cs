using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicArea : MonoBehaviour {
    public AudioClip music;
    void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            if (FindObjectOfType<MusicManager> () != null) {
                FindObjectOfType<MusicManager> ().FadeToNewMusic (music);
            }
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.tag == "Player") {
            if (FindObjectOfType<MusicManager> () != null) {
                FindObjectOfType<MusicManager> ().StopMusic (1);
            }
        }
    }
}