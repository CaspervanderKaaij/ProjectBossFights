using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpShaderValue : MonoBehaviour
{
    [SerializeField] string value = "val";
    [SerializeField] float goalValue = 0;
    [SerializeField] float speed = 1;
    [SerializeField] float startValue = 0;
    Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.SetFloat(value,startValue);
    }

    void Update()
    {
        rend.material.SetFloat(value, Mathf.Lerp(rend.material.GetFloat(value),goalValue,speed * Time.deltaTime));
    }
}
