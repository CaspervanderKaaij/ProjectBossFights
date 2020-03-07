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
    [SerializeField] UnityEvent endEv;
    //[SerializeField] AudioClip[] voiceAudio;
    [SerializeField] DiaTalkerValues[] voices;
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
            talker.text = curHolder.dialogue[curDia].talker;
            player.hudCanvas.enabled = false;
            textBack.SetActive(true);
            if (curHolder.dialogue[curDia].method == DiaVars.NextDiaMethod.Press) {
                if (Input.GetButtonUp (player.shootInput) == true && IsInvoking ("NoInput") == false) {
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
            } else if (IsInvoking ("NextLine") == false) {
                Invoke ("NextLine", curHolder.dialogue[curDia].waitTime);
            }

            if (curHolder != null) {
                if (curDia < curHolder.dialogue.Length) {
                    if (text.text == "" && IsInvoking ("SetTextPerLetter") == false && curHolder.dialogue[curDia].dia != "") {
                        InvokeRepeating ("SetTextPerLetter", 0, curHolder.dialogue[curDia].letterSpeed);
                    }
                }
            }
        } else if (textBack.activeSelf == true) {
            talker.text = "";
            text.text = "";
            textBack.SetActive(false);
            player.hudCanvas.enabled = true;
            player.curState = PlayerController.State.Normal;
            endEv.Invoke ();
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

    void SelfBased () {
        if (curHolder != null) {
            textBack.SetActive(true);
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
        } else if (textBack.activeSelf == true) {
            text.text = "";
            textBack.SetActive(false);
            endEv.Invoke ();
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