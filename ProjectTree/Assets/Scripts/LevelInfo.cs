using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu (fileName = "Chapter1Intro", menuName = "Level/LevelInfo", order = 0)]

public class LevelInfo : ScriptableObject {
    public string[] scenes;
    public string nextScene;

    public virtual void Load () {
        //unload all active scenes
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            SceneManager.UnloadSceneAsync (SceneManager.GetAllScenes()[i]);
        }

        //load all my scenes
        for (int i = 0; i < scenes.Length; i++)
        {
            SceneManager.LoadSceneAsync(scenes[i]);
        }
    }
}