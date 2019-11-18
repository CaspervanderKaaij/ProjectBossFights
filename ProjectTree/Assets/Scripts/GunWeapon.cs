using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour {

    public GameObject toSpawn;
    public Transform spawnPoint;
    public void GetInput () {
        GameObject g = Instantiate (toSpawn, spawnPoint.position, Quaternion.Euler (0, transform.eulerAngles.y - 90, 0));
    }
}