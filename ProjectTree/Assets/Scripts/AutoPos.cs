using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPos : MonoBehaviour {
    public Vector3 v3;
    public enum Type {
        Add,
        Translate,
        Lerp,
        MoveTowards
    }
    public Type myType = Type.Add;
    public float speed = 1;
    [SerializeField] bool unscaled = false;

    void Update () {
        switch (myType) {
            case Type.Add:
                transform.position += v3 * DeltaTime() * speed;
                break;
            case Type.Translate:
                transform.Translate (v3 * DeltaTime() * speed);
                break;
            case Type.Lerp:
                transform.position = Vector3.Lerp (transform.position, v3, DeltaTime() * speed);
                break;
            case Type.MoveTowards:
                transform.localPosition = Vector3.MoveTowards (transform.localPosition, v3, DeltaTime() * speed);
                break;
        }
    }

    float DeltaTime(){
        if(unscaled == true){
            return Time.unscaledDeltaTime;
        } else {
            return Time.deltaTime;
        }
    }

    public void SetSpeed(float newSpeed){
        speed = newSpeed;
    }
}