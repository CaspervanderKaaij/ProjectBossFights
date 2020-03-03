using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFaceObject : MonoBehaviour
{
    [SerializeField] GameObject[] heads;

    public void SetHead(int newHead){
        for (int i = 0; i < heads.Length; i++)
        {
            heads[i].SetActive((i == newHead));
        }
    }
}
