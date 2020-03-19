using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSceneManager : MonoBehaviour {
    void OnEnable () {
        if (FindObjectOfType<AudioListener> () == null) {
            gameObject.AddComponent<AudioListener> ();
        }

        if (FindObjectOfType<LevelLoader> () != null) {
            DialogueLevelInfo dia = null;
            if (FindObjectOfType<LevelLoader> ().curInfo.GetType () == typeof (DialogueLevelInfo)) {
                dia = (DialogueLevelInfo) FindObjectOfType<LevelLoader> ().curInfo;
            }
            if (dia != null) {
                FindObjectOfType<DialogueHolder> ().dialogue = dia.dialogue;
            }
        }

    }

    void Update () {
        if(Input.GetButtonDown("Pause") == true){
            Dialogue dia = FindObjectOfType<Dialogue>();
            if(dia != null){
                dia.End();
                dia.CancelInvoke();
                dia.StopAllCoroutines();
                dia.enabled = false;
            }
        }
    }
}