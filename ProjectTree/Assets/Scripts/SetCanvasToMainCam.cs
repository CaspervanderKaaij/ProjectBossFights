using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasToMainCam : MonoBehaviour {
    void Start () {
        Canvas mainCanvas = null;
        PlayerController player = FindObjectOfType<PlayerController> ();
        if (player != null) {
            mainCanvas = player.hudCanvas;

            if (mainCanvas != null) {
                Canvas c = GetComponent<Canvas> ();
                c.renderMode = RenderMode.WorldSpace;
                c.transform.SetParent (player.hudCanvas.transform, false);
                c.transform.localPosition = Vector3.zero;
                c.transform.localScale = Vector3.one;

            }

        }
        Destroy (this);
    }

}