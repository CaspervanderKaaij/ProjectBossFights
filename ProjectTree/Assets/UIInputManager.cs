using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputManager : MonoBehaviour {
    [SerializeField] List<UISelect> selectables = new List<UISelect> ();
    public UISelect curSelected;
    [Header ("Input")]
    [SerializeField] string horInput = "Item1";
    [SerializeField] string vertInput = "Item3";
    void Start () {
        selectables.Clear ();
        selectables.AddRange (transform.parent.GetComponentsInChildren<UISelect> ());

        curSelected = selectables[0];
    }

    void Update () {
        transform.position = Vector3.Lerp (transform.position, curSelected.transform.position, Time.unscaledDeltaTime * 15);
        CheckIsOvers ();
        GetInput ();
    }

    void CheckIsOvers () {
        for (int i = 0; i < selectables.Count; i++) {
            if (selectables[i].isOver == true) {
                curSelected = selectables[i];
                break;
            }
        }
    }

    float noInputTime = 0;
    void GetInput () {
        if (noInputTime <= 0) {
            if (Input.GetAxis (vertInput) > 0) {
                // print("up");
                if (curSelected.upNeighbour != null) {
                    SetCurSelected (curSelected.upNeighbour);
                }
            }
            if (Input.GetAxis (vertInput) < 0) {
                // print("down");
                if (curSelected.downNeighbour != null) {
                    SetCurSelected (curSelected.downNeighbour);
                }
            }
            if (Input.GetAxis (horInput) > 0) {
                //  print("right");
                if (curSelected.rightNeighbour != null) {
                    SetCurSelected (curSelected.rightNeighbour);
                }
            }
            if (Input.GetAxis (horInput) < 0) {
                // print("left");
                if (curSelected.leftNeighbour != null) {
                    SetCurSelected (curSelected.leftNeighbour);
                }
            }
        } else {
            noInputTime -= Time.unscaledDeltaTime;

            if(Input.GetAxis(horInput) == 0 && Input.GetAxis(vertInput) == 0){
                noInputTime = -1;
            }
        }
    }

    void SetCurSelected (UISelect newSel) {
        noInputTime = 0.15f;

        curSelected.isOver = false;
        curSelected = newSel;
        curSelected.isOver = true;
    }
}