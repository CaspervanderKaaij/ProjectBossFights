using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedUpdateParenter : MonoBehaviour
{
    public Transform parent;
    //this script is used for player parenting
    void FixedUpdate(){
        transform.position = parent.position;
        transform.eulerAngles = parent.eulerAngles;
    }
}