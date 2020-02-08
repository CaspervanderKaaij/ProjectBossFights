using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpShaderValue : MonoBehaviour {
    [SerializeField] string value = "val";
    [SerializeField] float goalValue = 0;
    [SerializeField] float speed = 1;
    [SerializeField] float startValue = 0;
    [SerializeField] Renderer[] rend;
    void Start () {
        for (int i = 0; i < rend.Length; i++) {
            rend[i].material.SetFloat (value, startValue);
        }
    }

    public void SetValue (float newVal) {
        for (int i = 0; i < rend.Length; i++) {
            rend[i].material.SetFloat (value, newVal);
        }
    }

    void Update () {
        for (int i = 0; i < rend.Length; i++) {
            rend[i].material.SetFloat (value, Mathf.Lerp (rend[i].material.GetFloat (value), goalValue, speed * Time.deltaTime));
        }
    }
}