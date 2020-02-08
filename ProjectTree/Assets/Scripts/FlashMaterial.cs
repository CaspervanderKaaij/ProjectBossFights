using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashMaterial : MonoBehaviour
{
    [SerializeField] Material flashMat;
    Material[] defaultMat;
    Renderer rend;

    void Start() {
        rend = GetComponent<Renderer>();
        defaultMat = rend.materials;    
    }
    public void Flash(float time){
        Material[] mat = new Material[defaultMat.Length];
        for (int i = 0; i < defaultMat.Length; i++)
        {
            mat[i] = flashMat;
        }
        rend.materials = mat;
        CancelInvoke("GoBack");
        Invoke("GoBack",time);
    }

    void GoBack(){
        rend.materials = defaultMat;
    }
}
