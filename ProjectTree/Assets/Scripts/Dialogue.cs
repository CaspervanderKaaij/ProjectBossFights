using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {
    [SerializeField] Text text;
    public Image textBack;
    public DialogueHolder curHolder;
    [HideInInspector] public int curDia;
    PlayerController player;
    [HideInInspector] public bool firstInput = false;
    [SerializeField] UnityEvent endEv;
    [SerializeField] AudioClip nextDiaAudio;
    void Start () {
        player = FindObjectOfType<PlayerController> ();
    }

    void Update () {
        if (player != null) {
            PlayerBased ();
        } else {
            SelfBased ();
        }
    }

    public float noInputTime = 1;
    void PlayerBased () {
        if (curHolder != null) {
            textBack.enabled = true;
            text.text = curHolder.dialogue[curDia];
            if (Input.GetButtonUp (player.shootInput) == true && IsInvoking("NoInput") == false) {
                if (firstInput == false) {
                    Invoke("NoInput",noInputTime);
                    curDia++;
                    if (curDia + 1 > curHolder.dialogue.Length) {
                        curHolder = null;
                    } else {
                        SpawnAudio.AudioSpawn (nextDiaAudio, 0, 1, 1);
                    }
                } else {
                    firstInput = false;
                }
            }
        } else if (textBack.enabled == true) {
            text.text = "";
            textBack.enabled = false;
            player.curState = PlayerController.State.Normal;
            endEv.Invoke ();
        }
    }

    void SelfBased () {
        if (curHolder != null) {
            textBack.enabled = true;
            text.text = curHolder.dialogue[curDia];
            if (Input.GetButtonUp ("Shoot") == true && IsInvoking("NoInput") == false) {
                if (firstInput == false) {
                    Invoke("NoInput",noInputTime);
                    curDia++;
                    if (curDia + 1 > curHolder.dialogue.Length) {
                        curHolder = null;
                    }
                } else {
                    firstInput = false;
                }
            }
        } else if (textBack.enabled == true) {
            text.text = "";
            textBack.enabled = false;
            endEv.Invoke ();
        }
    }

    void NoInput(){

    }
}