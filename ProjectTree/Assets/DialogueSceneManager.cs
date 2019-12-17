using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSceneManager : MonoBehaviour
{
    void OnEnable() {
        if(FindObjectOfType<LevelLoader>() != null){
            DialogueLevelInfo dia = (DialogueLevelInfo)FindObjectOfType<LevelLoader>().curInfo;
            if(dia != null){
            FindObjectOfType<DialogueHolder>().dialogue = dia.dialogue;
            }
        } 

        if(FindObjectOfType<AudioListener>() == null){
            gameObject.AddComponent<AudioListener>();
        }   
    }
}
