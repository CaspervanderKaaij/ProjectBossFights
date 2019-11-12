using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public bool unscaled = false;
    public Vector3 v3;

    void Update()
    {
        transform.Rotate(v3 * DeltaTime());
    }

    float DeltaTime(){
        if(unscaled == true){
            return Time.unscaledDeltaTime;
        }
        return Time.deltaTime;
    }
}
