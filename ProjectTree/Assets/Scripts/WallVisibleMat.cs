using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallVisibleMat : MonoBehaviour {
    Renderer[] rends;
    List<Material[]> myMats = new List<Material[]> ();
    List<Renderer> rendRefrences = new List<Renderer> ();
    [SerializeField] Material invisMat;
    bool vis;
    Transform cam;
    void Start () {
        rends = GetComponentsInChildren<Renderer> ();
        for (int i = 0; i < rends.Length; i++) {
            myMats.Add (rends[i].materials);
            rendRefrences.Add (rends[i]);
        }

        lastFrame = !vis;
        cam = Camera.main.transform;

    }

    bool lastFrame = false;
    void LateUpdate () {

        SetVis ();

        if (lastFrame != vis) {
            if (vis == true) {
                SetVisibleMats ();
            } else {
                SetInvisMats ();
            }
            lastFrame = vis;
        }
    }

    void SetVis () {
        if (Physics.Raycast (cam.position, -(cam.position - rendRefrences[0].transform.position), Vector3.Distance (cam.position, rendRefrences[0].transform.position) * 0.8f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore)) {
            vis = false;
        } else if (Physics.Raycast (cam.position, -(cam.position - rendRefrences[0].transform.position), Vector3.Distance (cam.position, rendRefrences[0].transform.position) * 0.8f, LayerMask.GetMask ("TriggerViewBlock"), QueryTriggerInteraction.Collide)) {
            vis = false;
        } else {
            vis = true;
        }
    }

    void SetInvisMats () {

        for (int i = 0; i < myMats.Count; i++) {

            Material[] mats = new Material[rendRefrences[i].materials.Length];

            for (int i2 = 0; i2 < mats.Length; i2++) {
                mats[i2] = invisMat;
            }

            rendRefrences[i].materials = mats;

        }

    }

    void SetVisibleMats () {

        for (int i = 0; i < myMats.Count; i++) {
            rendRefrences[i].materials = myMats[i];
        }

    }
}