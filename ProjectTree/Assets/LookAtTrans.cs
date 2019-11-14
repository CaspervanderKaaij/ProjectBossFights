using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTrans : MonoBehaviour
{
    public Transform goal;
    public Vector3 offset = Vector3.zero;

    void LateUpdate()
    {
        transform.LookAt(goal.transform.position);
        transform.Rotate(offset);
    }
}
