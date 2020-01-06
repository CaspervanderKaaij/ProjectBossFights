using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PReload : MonoBehaviour {

    void Update () {
        if (Input.GetKeyDown (KeyCode.P)) {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Scene[] curLoadedScene = SceneManager.GetAllScenes ();
            string[] scenes = new string[curLoadedScene.Length];
            for (int i = 0; i < curLoadedScene.Length; i++)
            {
                scenes[i] = curLoadedScene[i].name;
            }
            SceneManager.LoadScene(scenes[0]);
            for (int i = 1; i < curLoadedScene.Length; i++)
            {
                SceneManager.LoadScene(scenes[i],LoadSceneMode.Additive);
            }
        }
    }
}