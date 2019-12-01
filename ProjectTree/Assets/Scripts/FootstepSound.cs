using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [SerializeField] AudioClip[] clip;
    [SerializeField] float[] volume;
    [HideInInspector] public int curSoundType = 0;
    void Footstep(){
        SpawnAudio.AudioSpawn(clip[curSoundType],0,Random.Range(0.9f,1.3f),volume[curSoundType]);
    }

    void ActivateHitbox(int num){
        FindObjectOfType<PlayerController>().ActivateHitbox(num);
    }
}
