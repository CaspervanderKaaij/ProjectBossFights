using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasToMainCam : MonoBehaviour {
    void Start () {
        Canvas mainCanvas = null;
        if (FindObjectOfType<PlayerController> () != null) {
            mainCanvas = FindObjectOfType<PlayerController> ().hudCanvas;
        }
        if (mainCanvas != null) {
            Canvas c = GetComponent<Canvas> ();
            c.renderMode = RenderMode.WorldSpace;
            c.transform.SetParent (FindObjectOfType<PlayerController> ().cameraTransform, false);
            c.transform.localPosition = new Vector3 (0, 0, 0.2f);
            c.transform.localScale = Vector3.one * 0.00025f;

        }
        Destroy (this);
    }

}