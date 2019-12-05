using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLoadNextScene : MonoBehaviour {

    void Update () {
        if (Input.GetKeyDown (KeyCode.Equals) == true) {
            if(SceneManager.GetActiveScene ().buildIndex < SceneManager.GetAllScenes().Length){
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
            } else {
               SceneManager.LoadScene(1); 
            }
        }
    }
}