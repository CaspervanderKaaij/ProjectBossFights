using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [HideInInspector] public bool isOver = false;
    [SerializeField] string inputName = "Shoot";
    [SerializeField] UnityEvent clickEv;
    [SerializeField] UnityEvent hoverEv;
    [SerializeField] UnityEvent leaveEv;
    [Header("Neighbours")]
    public UISelect upNeighbour;
    public UISelect downNeighbour;
    public UISelect leftNeighbour;
    public UISelect rightNeighbour;

    public void OnPointerEnter (PointerEventData pointerEventData) {
        isOver = true;
        hoverEv.Invoke();
    }
    public void OnPointerExit (PointerEventData pointerEventData) {
        isOver = false;
        leaveEv.Invoke();
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