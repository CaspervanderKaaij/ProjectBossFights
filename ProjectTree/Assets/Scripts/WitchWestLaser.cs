using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchWestLaser : MonoBehaviour {
    public GameObject laser;
    public float chargeTime = 1;
    public float activeTime = 0.6f;
    public AudioClip clip;
    void Start () {
        StartCoroutine (Events ());
    }

    IEnumerator Events () {
        transform.localScale = Vector3.zero;
        yield return new WaitForSeconds (chargeTime);
        GetComponent<FlashMaterial> ().Flash (0.2f);
        yield return new WaitForSeconds (0.1f);
        laser.gameObject.SetActive (true);
        FindObjectOfType<PlayerCam> ().HardShake (0.1f);
        if (clip != null) {
            SpawnAudio.AudioSpawn (clip, 0, Random.Range (1.5f, 2.5f), 1);
        }
        transform.localScale *= 1.2f;
        if (GetComponentInChildren<LineRenderer> () != null) {
            Destroy (GetComponentInChildren<LineRenderer> ().gameObject);
        }
        yield return new WaitForSeconds (activeTime);
        if (laser != null && laser.transform.parent != null) {
            laser.SetActive (false);
        }
        GetComponent<AutoScale> ().goal = Vector3.zero;
        GetComponent<AutoScale> ().speed *= 2;
        yield return new WaitForSeconds (1);
        Destroy (gameObject);
    }
}