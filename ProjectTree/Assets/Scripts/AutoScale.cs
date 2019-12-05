using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScale : MonoBehaviour
{
    [SerializeField] Vector3 goal;
    [SerializeField] float speed = 1;

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale,goal,Time.deltaTime * speed);
    }
}
