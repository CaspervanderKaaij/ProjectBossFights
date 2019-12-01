using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamOnLand : LandAction
{
    PlayerCam cam;
    [SerializeField] Vector3 newOffset = new Vector3(7.2f,5,-7.2f);
    [SerializeField] Vector3 newAngle = new Vector3(30,-45,0);

    void Start(){
        cam = FindObjectOfType<PlayerCam>();
    }
    public override void Activate(){
        cam.offset = newOffset;
        cam.angleGoal = newAngle;
    }
}
