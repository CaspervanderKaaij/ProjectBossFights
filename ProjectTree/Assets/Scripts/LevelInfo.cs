using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu (fileName = "Chapter1Intro", menuName = "Level/LevelInfo", order = 0)]

public class LevelInfo : ScriptableObject {
    public string[] scenes;
    public string nextScene;

    public IEnumerator Activate () {
        yield return null;
        //activate some events
        LoadEvent[] events = FindObjectsOfType<LoadEvent> ();
        for (int i = 0; i < events.Length; i++) {
            events[i].onStartLoad.Invoke ();
        }
        //unload all scenes
        Time.timeScale = 0;
        //actual loading
        AsyncOperation[] async = new AsyncOperation[scenes.Length];
        async [0] = SceneManager.LoadSceneAsync (scenes[0], LoadSceneMode.Single);
        async [0].allowSceneActivation = false;
        for (int i = 1; i < async.Length; i++) {
            async [i] = SceneManager.LoadSceneAsync (scenes[i], LoadSceneMode.Additive);
            async [i].allowSceneActivation = false;
        }

        bool isDone = false;
        while (!isDone) {
            bool allDone = true;
            for (int i = 0; i < async.Length; i++) {
                if (async [i].progress < 0.9f) {
                    allDone = false;
                }
            }
            if (allDone = true) {
                for (int i = 0; i < async.Length; i++) {
                    async [i].allowSceneActivation = true;
                }
                yield return new WaitForSecondsRealtime (1.5f);
                for (int i = 0; i < events.Length; i++) {
                    events[i].onEndLoad.Invoke ();
                }
                Time.timeScale = 1;
                isDone = true;
                yield return null;
            }
        }

    }
}