using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class DialogueActor : MonoBehaviour {
    [SerializeField] DialogueHolder holder;
    Dialogue ui;
    int lastDia = -1;
    public DiaEvent[] events;
    void Start () {
        ui = FindObjectOfType<Dialogue> ();
        lastDia = -1;
    }

    void Update () {
        if (ui.curHolder == holder) {
            if (lastDia != ui.curDia) {
                CheckEvents ();
            }
            lastDia = ui.curDia;
        }
    }

    void CheckEvents () {
        for (int i = 0; i < events.Length; i++) {
            if (events[i].textNum == ui.curDia) {
                switch (events[i].evType) {
                    case DiaEvent.EventType.MoveTowards:
                        StartCoroutine(MoveTow(i));
                        break;
                    case DiaEvent.EventType.SetAnimation:
                        SetAnim (i);
                        break;
                    case DiaEvent.EventType.Teleport:
                        TeleportMe (i);
                        break;
                    case DiaEvent.EventType.PlaySound:
                        PlayS (i);
                        break;
                }
            }
        }
    }

    void TeleportMe (int ev) {
        events[ev].trans.position = events[ev].newPos;
        events[ev].trans.eulerAngles = events[ev].newRot;
    }
    void SetAnim (int ev) {
        events[ev].anim.Play (events[ev].animName);
    }
    IEnumerator MoveTow (int ev) {
        if (ui.curDia == events[ev].textNum) {
            events[ev].trans.position = Vector3.MoveTowards (events[ev].trans.position, events[ev].pos, Time.deltaTime * events[ev].speed);
            events[ev].trans.rotation = Quaternion.Lerp (events[ev].trans.rotation, Quaternion.Euler (events[ev].pos), Time.deltaTime * events[ev].rotSpeed);
            yield return new WaitForEndOfFrame();
            StartCoroutine(MoveTow(ev));
        } else {
            events[ev].trans.position = events[ev].pos;
            events[ev].trans.eulerAngles = events[ev].rot;
        }
    }
    void PlayS (int ev) {
        //I'll do this later
    }
}

[System.Serializable]
public class DiaEvent {
    public Transform trans;
    public int textNum = 0;
    public enum EventType {
        Teleport,
        SetAnimation,
        MoveTowards,
        PlaySound,
    }
    public EventType evType = EventType.Teleport;
    //teleport
    [ConditionalField ("evType", false, EventType.Teleport)] public Vector3 newPos = Vector3.zero;
    [ConditionalField ("evType", false, EventType.Teleport)] public Vector3 newRot = Vector3.zero;
    //movetowards
    [ConditionalField ("evType", false, EventType.MoveTowards)] public Vector3 pos = Vector3.zero;
    [ConditionalField ("evType", false, EventType.MoveTowards)] public Vector3 rot = Vector3.zero;
    [ConditionalField ("evType", false, EventType.MoveTowards)] public float speed = 1;
    [ConditionalField ("evType", false, EventType.MoveTowards)] public float rotSpeed = 1;
    //set animation
    [ConditionalField ("evType", false, EventType.SetAnimation)] public string animName = "Walk";
    [ConditionalField ("evType", false, EventType.SetAnimation)] public Animator anim;
    //play sound
    [ConditionalField ("evType", false, EventType.PlaySound)] public AudioClip clip;
    [Space]
    [SerializeField] UnityEvent ev;
}