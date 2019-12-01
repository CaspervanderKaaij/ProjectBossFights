using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendshapeWave : MonoBehaviour {
    SkinnedMeshRenderer rend;
    [SerializeField] float speed = 10;
    void Start () {
        rend = GetComponent<SkinnedMeshRenderer> ();
    }

    void Update () {
        rend.SetBlendShapeWeight (0, Mathf.Lerp(rend.GetBlendShapeWeight(0), Mathf.PingPong (Time.time * speed, 100),Time.deltaTime * speed / 10));
    }
}