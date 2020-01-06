using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashMaterial : MonoBehaviour
{
    [SerializeField] Material flashMat;
    Material defaultMat;
    Renderer rend;

    void Start() {
        rend = GetComponent<Renderer>();
        defaultMat = rend.material;    
    }
    public void Flash(float time){
        rend.material = flashMat;
        CancelInvoke("GoBack");
        Invoke("GoBack",time);
    }

    void GoBack(){
        rend.material = defaultMat;
    }
}
