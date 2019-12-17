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
    [SerializeField] AudioClip[] voiceAudio;
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
            SetTextPerLetter ();
            if (Input.GetButtonUp (player.shootInput) == true && IsInvoking ("NoInput") == false) {
                if (firstInput == false) {
                    text.text = "";
                    Invoke ("NoInput", noInputTime);
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
            player.curState = PlayerController.State.Normal;
            endEv.Invoke ();
        }
    }

    void SetTextPerLetter () {
        if (text.text.Length < curHolder.dialogue[curDia].dia.Length) {
            text.text += curHolder.dialogue[curDia].dia[text.text.Length];
            SpawnAudio.AudioSpawn (voiceAudio[curHolder.dialogue[curDia].talkerID], 0, Random.Range (2.2f, 2.9f), Random.Range (0.05f, 0.1f));
        }
    }

    void SelfBased () {
        if (curHolder != null) {
            textBack.enabled = true;
            SetTextPerLetter ();
            if (Input.GetButtonUp ("Shoot") == true && IsInvoking ("NoInput") == false) {
                if (firstInput == false) {
                    text.text = "";
                    Invoke ("NoInput", noInputTime);
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

    void NoInput () {

    }
}