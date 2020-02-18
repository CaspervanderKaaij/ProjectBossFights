using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkTaicaBoss : MonoBehaviour {
    PlayerController player;
    Vector3 moveV3;
    CharacterController cc;
    [SerializeField] Animator anim;
    float curAccDec = 0;
    public enum State {
        Normal,
        Dash
    }
    public State curState = State.Normal;
    [SerializeField] Collider battleArenaBorder;
    PlayerCam cam;
    [Header ("Movement Stats")]
    [SerializeField] float walkSpeed = 5;
    [SerializeField] float accelerationSpeed = 3;
    [SerializeField] float rotSpeed = 5;
    [Header ("Dash")]
    [SerializeField] Renderer[] dashInvisRend;
    [SerializeField] GameObject[] dashRends;
    [SerializeField] float dashSpeed = 30;
    [SerializeField] float dashTime = 1;
    [SerializeField] AudioClip[] dashAudio;
    [SerializeField] GameObject stopDashParticle;
    void Start () {
        player = FindObjectOfType<PlayerController> ();
        cc = GetComponent<CharacterController> ();
        cam = FindObjectOfType<PlayerCam> ();
    }

    void Update () {
        switch (curState) {
            case State.Normal:
                Walk (player.transform.position + (player.transform.forward * 3));
                FinalMove ();
                break;
            case State.Dash:
                Dash ();
                FinalMove ();
                break;
        }
    }

    void Walk (Vector3 goalPos) {
        goalPos = battleArenaBorder.ClosestPoint (goalPos);
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (-(new Vector3 (transform.position.x, goalPos.y, transform.position.z) - goalPos), Vector3.up), Time.deltaTime * rotSpeed);
        float distance = Vector2.Distance (new Vector2 (transform.position.x, transform.position.z), new Vector2 (goalPos.x, goalPos.z));
        if (distance > 3) {
            curAccDec = Mathf.MoveTowards (curAccDec, 1, Time.deltaTime * accelerationSpeed);
        } else {
            curAccDec = Mathf.MoveTowards (curAccDec, 0, Time.deltaTime * accelerationSpeed);
        }
        moveV3.x = transform.forward.x * curAccDec * walkSpeed;
        moveV3.z = transform.forward.z * curAccDec * walkSpeed;

        anim.SetFloat ("curSpeed", curAccDec);
        if (IsInvoking ("NoDashCheck") == false) {
            if (distance > 5 && Random.Range (0, 100) > 30) {
                StartDash (Quaternion.LookRotation (-(new Vector3 (transform.position.x, goalPos.y, transform.position.z) - goalPos), Vector3.up).eulerAngles.y);
            }
            Invoke ("NoDashCheck", Random.Range (0.1f, 1));
        }

    }

    void NoDashCheck () {

    }

    void StartDash (float yAngle) {

        oldPos = transform.position;
        transform.eulerAngles = new Vector3 (transform.eulerAngles.x, yAngle, transform.eulerAngles.z);

        for (int i = 0; i < dashInvisRend.Length; i++) {
            dashInvisRend[i].enabled = false;
        }

        for (int i = 0; i < dashRends.Length; i++) {
            dashRends[i].SetActive (true);
        }

        curState = State.Dash;

        Invoke ("StopDash", dashTime);

        // SpawnAudio.AudioSpawn (dashAudio[0], 0f, Random.Range (4.5f, 4.5f), 1);
        SpawnAudio.AudioSpawn (dashAudio[1], 0.4f, Random.Range (0.75f, 1.25f), 1);
    }

    void Dash () {
        if (battleArenaBorder.ClosestPoint (transform.position) == transform.position) {

            moveV3 = transform.forward * dashSpeed;
            oldPos = transform.position;

        } else {
            if (IsInvoking ("StopDash") == true) {
                CancelInvoke ("StopDash");
            }
            StopDash ();
            transform.position = oldPos;
        }
    }

    Vector3 oldPos;
    void StopDash () {
        curAccDec = 0;
        curState = State.Normal;
        for (int i = 0; i < dashInvisRend.Length; i++) {
            dashInvisRend[i].enabled = true;
        }

        for (int i = 0; i < dashRends.Length; i++) {
            dashRends[i].SetActive (false);
        }

        cam.MediumShake (0.2f);
        Instantiate (stopDashParticle, transform.position + transform.up, Quaternion.identity);
    }

    void FinalMove () {
        cc.Move (moveV3 * Time.deltaTime);
    }
}