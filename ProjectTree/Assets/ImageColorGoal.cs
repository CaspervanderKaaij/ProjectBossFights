using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorGoal : MonoBehaviour {
    Image img;
    public Color goalColor;
    [SerializeField] float speed = 5;
    [Header ("Presets for UnityEvents")]
    [SerializeField] Color[] colors;
    void Start () {
        img = GetComponent<Image> ();
    }

    void LateUpdate () {
        if (img.color != goalColor) {
            Vector4 crlHelper = Vector4.MoveTowards (new Vector4 (img.color.r, img.color.g, img.color.b, img.color.a), new Vector4 (goalColor.r, goalColor.g, goalColor.b, goalColor.a), Time.unscaledDeltaTime * speed);
            img.color = new Color (crlHelper.x, crlHelper.y, crlHelper.z, crlHelper.w);
        }
    }

    public void SetGoal (int id) {
        goalColor = colors[id];
    }
}