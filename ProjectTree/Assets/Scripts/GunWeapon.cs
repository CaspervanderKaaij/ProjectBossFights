using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour {
    public string horInput = "Horizontal";
    public string vertInput = "Vertical";

    public GameObject toSpawn;
    public Transform spawnPoint;
    float curAngle = 0;
    Transform camTransform;

    void Start(){
        camTransform = Camera.main.transform;
    }

    void LateUpdate () {
        if (Vector2.SqrMagnitude (new Vector2 (Input.GetAxis (vertInput), Input.GetAxis (horInput))) > 0) {
            curAngle = AnalAngle ();
        }
    }
    public void GetInput () {
        GameObject g = Instantiate (toSpawn, spawnPoint.position, Quaternion.Euler (0, curAngle + camTransform.eulerAngles.y, 0));
    }

    public float AnalAngle () {
        return Mathf.Atan2 (-Input.GetAxis (vertInput), Input.GetAxis (horInput)) * Mathf.Rad2Deg;
    }
}