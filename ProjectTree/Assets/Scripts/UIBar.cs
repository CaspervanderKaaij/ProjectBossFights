using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour {

    [Range (0, 100)] public float curPercent = 100;
    float maxScale;
    [SerializeField] float lerpSpeed = 1000;
    [SerializeField] Gradient colors;
    Image img;
    void Start () {
        maxScale = transform.localScale.x;
        img = GetComponent<Image> ();
    }

    void Update () {
        transform.localScale = Vector3.Lerp (transform.localScale, new Vector3 (maxScale * (curPercent / 100), transform.localScale.y, transform.localScale.z), Time.deltaTime * lerpSpeed);
        img.color = colors.Evaluate(curPercent / 100);
    }
}