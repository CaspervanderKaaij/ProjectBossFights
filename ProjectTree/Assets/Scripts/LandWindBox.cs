using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandWindBox : LandAction
{

    public Vector3 velocity = Vector3.zero;
    CharacterController player;

    void Start(){
        player = FindObjectOfType<PlayerController>().GetComponent<CharacterController>();
    }
    public override void Activate(){
        player.Move(velocity * Time.deltaTime);
    }
}
