using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBlendShape : MonoBehaviour
{
    int shapeToSet = 0;
    [SerializeField] SkinnedMeshRenderer[] mesh;
    int curRend = 0;
    public void SetShape(float v){
        print(v + "v <-      currend ->" + curRend);
        mesh[curRend].SetBlendShapeWeight(shapeToSet,v);
    }

    public void SetShapeToSet(int s){
        shapeToSet = s;
    }

    public void SetCurRend(int r){
        curRend = r;
    }


    //Heinz Boss
    public void SetLeftHand(float f){
        mesh[0].SetBlendShapeWeight(0,f);
    }

    public void SetRightHand(float f){
        mesh[0].SetBlendShapeWeight(1,f);
    }

    public void SetBellyLines(float f){
        mesh[1].SetBlendShapeWeight(0,f);
    }
}
