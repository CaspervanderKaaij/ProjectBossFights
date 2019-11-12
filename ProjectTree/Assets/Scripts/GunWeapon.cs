using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour {
    public string horInput = "Horizontal";
    public string vertInput = "Vertical";

    public GameObject toSpawn;
    float curAngle = 0;

    void LateUpdate () {
        if (Vector2.SqrMagnitude (new Vector2 (Input.GetAxis (vertInput), Input.GetAxis (horInput))) > 0) {
            curAngle = AnalAngle ();
        }
    }
    public void GetInput () {
        GameObject g = Instantiate (toSpawn, transform.position, transform.rotation * Quaternion.Euler (0, 0, curAngle));
    }

    public float AnalAngle () {
        return Mathf.Atan2 (Input.GetAxis (vertInput), Input.GetAxis (horInput)) * Mathf.Rad2Deg;
    }
}