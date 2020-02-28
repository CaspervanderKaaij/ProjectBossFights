using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikaSnekwurm : MonoBehaviour {
    [SerializeField] MikaSnekwurmPart[] parts;
    [SerializeField] float partSpeed = 10;
    List<Vector3> playerPosses = new List<Vector3> ();
    Transform player;
    [SerializeField] int delay = 50;
    [SerializeField] GameObject spawnParticle;
    [SerializeField] AudioClip spawnAudio;
    void Start () {
        player = FindObjectOfType<PlayerController> ().transform;

        StartCoroutine (SpawnEv ());
    }

    IEnumerator SpawnEv () {
        Instantiate (spawnParticle, parts[0].transform.position, Quaternion.identity);
        for (int i = 0; i < parts.Length; i++) {
            yield return new WaitForSeconds (0.05f);
            parts[i].GetComponent<MeshRenderer> ().enabled = true;
            parts[i].GetComponent<LineRenderer> ().enabled = true;
            Instantiate (spawnParticle, parts[i].transform.position, Quaternion.identity);

            SpawnAudio.AudioSpawn(spawnAudio,0.1f,Random.Range(0.9f,1.2f),0.3f);
        }
        SpawnAudio.AudioSpawn(spawnAudio,0.1f,1,0.7f);
        transform.GetChild (0).GetComponent<Collider> ().enabled = true;
        for (int i = 0; i < parts.Length; i++) {
            parts[i].GetComponent<Collider> ().enabled = true;
        }
    }

    bool ded = false;
    public IEnumerator DeathEv (float after) {
        yield return new WaitForSeconds (after);
        transform.GetChild (0).GetComponent<Collider> ().enabled = false;
        for (int i = 0; i < parts.Length; i++) {
            parts[i].GetComponent<Collider> ().enabled = false;
        }
        ded = true;
        transform.GetChild(0).GetComponent<AutoRotate>().enabled = false;
        yield return new WaitForSeconds (1);
        for (int i = 0; i < parts.Length; i++) {
            yield return new WaitForSeconds (0.05f);
            parts[parts.Length - i - 1].GetComponent<MeshRenderer> ().enabled = false;
            parts[parts.Length - i - 1].GetComponent<LineRenderer> ().enabled = false;
            SpawnAudio.AudioSpawn(spawnAudio,0,1,1);
            SpawnAudio.AudioSpawn(spawnAudio,0.1f,Random.Range(0.9f,1.2f),0.1f);
        }
        yield return new WaitForSeconds (0.05f);
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        Instantiate (spawnParticle, transform.GetChild(0).transform.position, Quaternion.identity);
        Destroy (gameObject);
    }

    void FixedUpdate () {
        if (ded == false) {
            Move ();
            UpdateParts ();
        }
    }

    void Move () {
        playerPosses.Add (player.position + new Vector3 (0, 1.5f, 0));
        if (playerPosses.Count > delay) {
            transform.position = Vector3.MoveTowards (transform.position, playerPosses[0], Time.deltaTime * 20);
            if (Vector3.Distance (playerPosses[0], playerPosses[1]) > 0.01f) {
                transform.LookAt (transform.position + (playerPosses[0] - playerPosses[1]));
                transform.Rotate (-90, 0, 0);
            }

            playerPosses.RemoveAt (0);
        } else {
            transform.Rotate (0, -500 * Time.deltaTime, 0);
        }

    }

    void UpdateParts () {
        if (playerPosses.Count > 2) {
            for (int i = 0; i < parts.Length; i++) {
                parts[i].UpdateMe (partSpeed);
            }
        }
    }
}