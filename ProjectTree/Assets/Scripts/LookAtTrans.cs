using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTrans : MonoBehaviour
{
    public Transform goal;
    public Vector3 offset = Vector3.zero;
    public bool lookatCam = false;

    void Start(){
        if(lookatCam == true && Camera.main != null){
            goal = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        transform.LookAt(goal.transform.position);
        transform.Rotate(offset);
    }
}
