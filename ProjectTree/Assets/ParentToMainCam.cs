using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToMainCam : MonoBehaviour
{
    void Start()
    {
        transform.SetParent(Camera.main.transform,false);       
    }

}
