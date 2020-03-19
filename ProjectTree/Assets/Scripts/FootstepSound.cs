using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour {
    [SerializeField] AudioClip[] clip;
    [SerializeField] float[] volume;
    [HideInInspector] public int curSoundType = 0;
    PlayerController player;

    void Start () {
        player = FindObjectOfType<PlayerController> ();
    }
    void Footstep () {
        SpawnAudio.AudioSpawn (clip[curSoundType], 0, Random.Range (0.9f, 1.3f), volume[curSoundType]);
    }

    void ActivateHitbox (int num) {
        player.ActivateHitbox (num);
    }

    float weightSmoother = 0;
    void OnAnimatorIK (int layerIndex) {
        if (player != null) {
            if (player.interactables.Count > 0 && player.isGrounded == true) {
                Interact chosenOne = player.interactables[0];
                for (int i = 0; i < player.interactables.Count; i++) {
                    if (player.interactables[i].priority > chosenOne.priority) {
                        chosenOne = player.interactables[i];
                    }
                }
                weightSmoother = Mathf.Lerp (weightSmoother, 1, Time.deltaTime * 5);
                player.anim.SetLookAtWeight (weightSmoother);
                player.anim.SetLookAtPosition (new Vector3 (chosenOne.transform.position.x, player.transform.position.y + 1, chosenOne.transform.position.z));
            } else {
                weightSmoother = Mathf.Lerp (weightSmoother, 0, Time.deltaTime * 5);
                player.anim.SetLookAtWeight (weightSmoother);
            }
        }
    }
}