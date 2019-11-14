using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {
    [SerializeField] Text text;
    public Image textBack;
    public DialogueHolder curHolder;
    [HideInInspector] public int curDia;
    PlayerController player;
    void Start () {
        player = FindObjectOfType<PlayerController> ();
    }

    void Update () {
        if (curHolder != null) {
            textBack.enabled = true;
            text.text = curHolder.dialogue[curDia];
            if (Input.GetButtonDown (player.shootInput) == true) {
                curDia++;
                if(curDia + 1 > curHolder.dialogue.Length){
                    curHolder = null;
                }
            }
        } else if(textBack.enabled == true) {
            text.text = "";
            textBack.enabled = false;
            player.curState = PlayerController.State.Normal;
        }
    }
}