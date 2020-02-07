using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikaSnekwurmPart : MonoBehaviour
{
    Vector3 lastWorldRot = Vector3.zero;
    Vector3 lastWorldPos = Vector3.zero;
    Vector3 posSaver;
    void Start(){
        lastWorldRot = transform.eulerAngles;
        lastWorldPos = transform.position;
        posSaver = transform.localPosition;
    }
    public void UpdateMe(float speed)
    {
        transform.eulerAngles = lastWorldRot;
        transform.position = lastWorldPos;
       
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero),Time.deltaTime * speed);
      
        transform.localPosition = Vector3.Lerp(transform.localPosition,posSaver,Time.deltaTime * speed);
        lastWorldRot = transform.eulerAngles;
        lastWorldPos = transform.position;

        transform.LookAt(transform.parent.position);
    }
}
