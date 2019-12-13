using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextEvent : MonoBehaviour {
    public void Activate () {
        if (FindObjectOfType<LevelLoader> () != null) {
            LevelLoader loader = FindObjectOfType<LevelLoader> ();
            loader.Activate(loader.curInfo.nextScene);
        } else {
            Debug.LogError("Level Loader not found, please start the game from the Init scene");
        }
    }
}