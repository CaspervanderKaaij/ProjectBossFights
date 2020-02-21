using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialGroup : MonoBehaviour {
    [HideInInspector] public Renderer[] rends;
    public List<SetMatHelper> myMats = new List<SetMatHelper> ();
    [SerializeField] string description;
    [SerializeField] GameObject rootOfRends;

    public List<Renderer> rendRefrences = new List<Renderer> ();


    public void SetToGroup () {
        for (int i = 0; i < myMats.Count; i++) {
            rendRefrences[i].materials = myMats[i].matArray;
        }
    }

    public void SetInspectorMats () {
        
        rends = rootOfRends.GetComponentsInChildren<Renderer> ();
        myMats.Clear();
        for (int i = 0; i < rends.Length; i++) {
            SetMatHelper hlp = new SetMatHelper();
            hlp.matArray = rends[i].sharedMaterials;
            myMats.Add (hlp);
            rendRefrences.Add (rends[i]);
        }
    }


}

[System.Serializable] public class SetMatHelper
{
    public Material[] matArray;
}

