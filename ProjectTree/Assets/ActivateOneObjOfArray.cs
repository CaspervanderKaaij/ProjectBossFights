using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOneObjOfArray : MonoBehaviour
{
    [SerializeField] GameObject[] objects;

    public void SetOneActive(int obj){
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive((i == obj));
        }
    }
}
