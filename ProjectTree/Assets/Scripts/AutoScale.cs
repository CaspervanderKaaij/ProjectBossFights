using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScale : MonoBehaviour
{
    public Vector3 goal;
    public float speed = 1;

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale,goal,Time.deltaTime * speed);
    }
}
