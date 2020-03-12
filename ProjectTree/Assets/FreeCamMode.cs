using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamMode : MonoBehaviour {
    bool unlocked = true;
    TimescaleManager timeMan;
    void Start () {
        timeMan = FindObjectOfType<TimescaleManager> ();
    }

    void LateUpdate () {
        if ((Input.GetKeyDown (KeyCode.Tab) || Input.GetKeyDown (KeyCode.Joystick1Button11)) && unlocked == true) {
            Activate ();
        }

        if(active == true){
            Movement();

            if(timeMan != null){
                if(timeMan.curState != TimescaleManager.State.None){
                    Activate();
                }
            }
        }
    }

    bool active = false;
    Vector3 lastCamPos = Vector3.zero;
    Vector3 lastCamRot = Vector3.zero;
    void Activate () {
        if (timeMan != null) {
            if (timeMan.isPaused == false) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                timeMan.isPaused = true;
                active = true;
                lastCamRot = transform.eulerAngles;
                lastCamPos = transform.position;

                timeMan.PauseAllAudio(true,false);
                timeMan.uiCam.enabled = false;

                timeMan.enabled = false;//it was buggy. Feel free to mod it lol im too lazy

                Time.timeScale = 0;
            } else if (active == true) {
                active = false;
                transform.eulerAngles = lastCamRot;
                transform.position = lastCamPos;

                
                timeMan.PauseAllAudio(false,false);
                timeMan.uiCam.enabled = true;
                
                timeMan.isPaused = false;

                timeMan.enabled = true;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    void Movement(){
        transform.Rotate(Input.GetAxis("Mouse Y")  * Time.unscaledDeltaTime * -200,Input.GetAxis("Mouse X")  * Time.unscaledDeltaTime * 200,0);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0);
        transform.position += transform.forward * Input.GetAxisRaw("Vertical") * 10 * Time.unscaledDeltaTime;
        transform.position += transform.right * Input.GetAxisRaw("Horizontal") * 10 * Time.unscaledDeltaTime;


    }
}