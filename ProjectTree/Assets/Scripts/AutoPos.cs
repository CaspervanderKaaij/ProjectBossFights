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

    void Update () {
        switch (myType) {
            case Type.Add:
                transform.position += v3 * Time.deltaTime * speed;
                break;
            case Type.Translate:
                transform.Translate (v3 * Time.deltaTime * speed);
                break;
            case Type.Lerp:
                transform.position = Vector3.Lerp (transform.position, v3, Time.deltaTime * speed);
                break;
            case Type.MoveTowards:
                transform.localPosition = Vector3.MoveTowards (transform.localPosition, v3, Time.deltaTime * speed);
                break;
        }
    }
}