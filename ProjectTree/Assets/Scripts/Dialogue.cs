using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {
    [SerializeField] Text text;
    [SerializeField] Text talker;
    public GameObject textBack;
    public DialogueHolder curHolder;
    [HideInInspector] public int curDia;
    PlayerController player;
    [HideInInspector] public bool firstInput = false;
    public UnityEvent endEv;
    //[SerializeField] AudioClip[] voiceAudio;
    [SerializeField] DiaTalkerValues[] voices;
    void Start () {
        player = FindObjectOfType<PlayerController> ();
    }

    public float noInputTime = 1;
    void Update () {
        if (curHolder != null) {
            talker.text = curHolder.dialogue[curDia].talker;
            if (player != null) {
                player.hudCanvas.enabled = false;
            }
            textBack.SetActive (true);
            switch (curHolder.dialogue[curDia].method) {
                case DiaVars.NextDiaMethod.Press:
                    UpdatePress ();
                    break;
                case DiaVars.NextDiaMethod.Wait:
                    if (IsInvoking ("NextLine") == false) {
                        Invoke ("NextLine", curHolder.dialogue[curDia].waitTime);
                    }
                    break;
                case DiaVars.NextDiaMethod.NoBackPress:
                    UpdatePress ();
                    textBack.SetActive (false);
                    break;
                case DiaVars.NextDiaMethod.NoBackWait:
                    curHolder.dialogue[curDia].waitTime = curHolder.dialogue[curDia]._waitTime;
                    if (IsInvoking ("NextLine") == false) {
                        Invoke ("NextLine", curHolder.dialogue[curDia].waitTime);
                    }
                    textBack.SetActive (false);
                    break;
            }

            if (curHolder != null) {
                if (curDia < curHolder.dialogue.Length) {
                    if (text.text == "" && IsInvoking ("SetTextPerLetter") == false && curHolder.dialogue[curDia].dia != "") {
                        InvokeRepeating ("SetTextPerLetter", 0, curHolder.dialogue[curDia].letterSpeed);
                    }
                }
            }
        } else if (text.text != " ") {
            End();
        }
    }

    public void End(){
            talker.text = "";
            text.text = " ";
            textBack.SetActive (false);
            if (player != null) {
                player.hudCanvas.enabled = true;
                player.curState = PlayerController.State.Normal;
            }
            endEv.Invoke ();
    }

    void UpdatePress () {

        if (Input.GetButtonUp ("Shoot") == true && IsInvoking ("NoInput") == false) {
            if (IsInvoking ("SetTextPerLetter") == false) {
                if (firstInput == false) {
                    NextLine ();
                } else {
                    firstInput = false;
                }
            } else {
                if (firstInput == false) {
                    CancelInvoke ("SetTextPerLetter");
                    text.text = curHolder.dialogue[curDia].dia;
                } else {
                    firstInput = false;
                }
            }
        }

    }

    void NextLine () {
        text.text = "";
        curDia++;
        CancelInvoke ("SetTextPerLetter");
        if (curDia < curHolder.dialogue.Length) {
            InvokeRepeating ("SetTextPerLetter", 0, curHolder.dialogue[curDia].letterSpeed);
        }
        if (curDia + 1 > curHolder.dialogue.Length) {
            curHolder = null;
        }
    }

    void SetTextPerLetter () {
        if (text.text.Length < curHolder.dialogue[curDia].dia.Length) {
            text.text += curHolder.dialogue[curDia].dia[text.text.Length];
            DiaTalkerValues vals = voices[curHolder.dialogue[curDia].talkerID];
            SpawnAudio.AudioSpawn (vals.clip, vals.startTime, Random.Range (vals.pitchMin, vals.pitchMax), vals.volume);
        } else {
            CancelInvoke ("SetTextPerLetter");
            firstInput = false;
        }
    }

    void NoInput () {

    }
}

[System.Serializable]
public class DiaTalkerValues {
    public AudioClip clip;
    public float pitchMin = 2.2f;
    public float pitchMax = 2.9f;
    public float volume = 0.00f;
    public float startTime = 0;
}