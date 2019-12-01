using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    bool isOver = false;
    [SerializeField] string inputName = "Shoot";
    [SerializeField] UnityEvent clickEv;
    public void OnPointerEnter (PointerEventData pointerEventData) {
        isOver = true;
    }
    public void OnPointerExit (PointerEventData pointerEventData) {
        isOver = false;
    }

    void OnDisable () {
        isOver = false;
    }

    void OnEnable () {
        isOver = false;
    }

    void Update () {
        if (inputName != "") {
            if (isOver == true && Input.GetButtonDown (inputName) == true) {
                Activate ();
                clickEv.Invoke ();
            }
        }
    }

    public virtual void Activate () {
        print ("explosion");
    }
}