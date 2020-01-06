using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUITimer : MonoBehaviour {
    float time = 0;
    Text txt;
    void Start () {
        txt = GetComponent<Text> ();
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.T) == true) {
            time = 0;
        }
        time += Time.unscaledDeltaTime;
        txt.text = (int)time + " seconds";
    }
}