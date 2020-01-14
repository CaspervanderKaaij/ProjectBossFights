using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRenderFix : MonoBehaviour {

    void Start () {
        transform.GetComponent<MeshFilter>().mesh.bounds = new Bounds (Vector3.zero, Vector3.one * 2000);
        Destroy(this);
    }
}