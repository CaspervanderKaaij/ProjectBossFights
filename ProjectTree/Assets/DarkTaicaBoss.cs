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
        Dash,
        Attack,
        JumpSlam,
        Gun,
        Other
    }
    public State curState = State.Normal;
    [SerializeField] Collider battleArenaBorder;
    PlayerCam cam;
    [Header ("Movement Stats")]
    [SerializeField] float walkSpeed = 5;
    [SerializeField] float accelerationSpeed = 3;
    [SerializeField] float rotSpeed = 5;
    [SerializeField] float jumpStrength = 10;
    [SerializeField] float gravityStrength = 5;
    [SerializeField] float gravitySpeed = 10;
    [Header ("Dash")]
    [SerializeField] Renderer[] dashInvisRend;
    [SerializeField] GameObject[] dashRends;
    [SerializeField] float dashSpeed = 30;
    [SerializeField] float dashTime = 1;
    [SerializeField] AudioClip[] dashAudio;
    [SerializeField] GameObject stopDashParticle;
    [Header ("Attack")]
    [SerializeField] GameObject[] hitboxes;
    [SerializeField] AudioClip slamAttackAudio;
    [Header ("Gun")]
    [SerializeField] GameObject spearObj;
    [SerializeField] GameObject gunObj;
    [SerializeField] GameObject hitEffectParticle;
    [SerializeField] GameObject bullet;
    void Start () {
        player = FindObjectOfType<PlayerController> ();
        cc = GetComponent<CharacterController> ();
        cam = FindObjectOfType<PlayerCam> ();
    }

    void Update () {
        switch (curState) {
            case State.Normal:
                Walk (player.transform.position + (player.transform.forward * 3));
                Gravity ();
                FinalMove ();
                break;
            case State.Dash:
                Dash ();
                FinalMove ();
                break;
            case State.JumpSlam:
                Gravity ();
                JumpSlam ();
                FinalMove ();
                break;
            case State.Gun:
                Gun (player.transform.position);
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
        if (Input.GetKeyDown (KeyCode.Alpha8)) {
            StartAttack ();
        }
        if (Input.GetKeyDown (KeyCode.Alpha7)) {
            StartGun ();
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
            Knockback ();
            transform.position = oldPos;
        }

    }

    public void GetHit () {
        Knockback ();
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

    void StartAttack () {
        if (IsGrounded () == true) {
            curState = State.Attack;
            anim.Play ("TaicaAtkJab1");
            StartCoroutine (SetHitboxActive (0, 0.2f, 0.2f));
            transform.rotation = Quaternion.LookRotation (-(new Vector3 (transform.position.x, player.transform.position.y, transform.position.z) - player.transform.position), Vector3.up);
        } else {
            curState = State.JumpSlam;
            moveV3.x = (transform.forward * curAccDec * walkSpeed).x;
            moveV3.z = (transform.forward * curAccDec * walkSpeed).z;
            moveV3.y = jumpStrength / 2;
            anim.Play ("TaicaJumpSlam");
        }
    }

    IEnumerator SetHitboxActive (int hBox, float startupTime, float endTime) {
        yield return new WaitForSeconds (startupTime);
        hitboxes[hBox].SetActive (true);
        yield return new WaitForSeconds (endTime);
        curState = State.Normal;
        curAccDec = 0;
    }

    void Knockback () {
        anim.Play ("TaicaGetHit");
        curState = State.Other;
        Invoke ("SetStateNormal", 0.3f);

        for (int i = 0; i < dashInvisRend.Length; i++) {
            dashInvisRend[i].enabled = true;
        }

        for (int i = 0; i < dashRends.Length; i++) {
            dashRends[i].SetActive (false);
        }

        spearObj.SetActive (true);
        gunObj.SetActive (false);
    }

    void SetStateNormal () {
        curState = State.Normal;
    }

    void Gravity () {
        if (moveV3.y > 0) {
            moveV3.y = Mathf.MoveTowards (moveV3.y, gravityStrength, Time.deltaTime * gravitySpeed);
        } else {
            moveV3.y = Mathf.MoveTowards (moveV3.y, gravityStrength, Time.deltaTime * gravitySpeed / 2);

        }

        if (Input.GetKeyDown (KeyCode.Alpha9) == true && IsGrounded () == true) {
            moveV3.y = jumpStrength;
        }
        anim.SetBool ("grounded", IsGrounded ());
    }

    bool IsGrounded () {
        RaycastHit hit;
        if (Physics.Raycast (transform.position + Vector3.up, Vector3.down, out hit, 1.3f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore)) {
            return true;
        }
        return false;
    }

    void JumpSlam () {
        if (IsGrounded () == true) {
            anim.Play ("TaicaJumpSlamHit");
            StartCoroutine (SetHitboxActive (1, 0, 1));
            moveV3 = Vector3.zero;
            curState = State.Attack;
            anim.SetFloat ("curSpeed", 0);

            cam.MediumShake (0.1f);
            Instantiate (stopDashParticle, transform.position, Quaternion.identity);
            SpawnAudio.AudioSpawn (slamAttackAudio, 0.5f, Random.Range (4, 5), 0.5f);
            FindObjectOfType<TimescaleManager> ().SlowMo (0.15f, 0.05f, 0.1f);

        }
    }

    void StartGun () {
        curAccDec = 0;
        anim.Play ("TaicaStartGun");

        spearObj.SetActive (false);
        gunObj.SetActive (true);

        curState = State.Gun;
    }

    void Gun (Vector3 targetPos) {

        targetPos.y = transform.position.y;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-(transform.position - targetPos),Vector3.up),Time.deltaTime * 25);

        if (Input.GetKeyDown (KeyCode.Alpha7)) {
            StopGun ();
        }

        if (Input.GetKeyDown (KeyCode.Alpha8)) {
            GunShot (targetPos);
        }
    }

    void StopGun () {
        anim.Play ("TaicaEndGun");
        Invoke ("SetStateNormal", 0.3f);

        spearObj.SetActive (true);
        gunObj.SetActive (false);
    }

    void GunShot (Vector3 target) {
        anim.Play ("TaicaGunShoot", 0, 0f);

        FindObjectOfType<TimescaleManager>().SlowMo (0.1f, 0.05f);

        Instantiate (hitEffectParticle, gunObj.transform.position, Quaternion.identity);
        SpawnAudio.AudioSpawn (dashAudio, 0, Random.Range (2.5f, 3), 1);
        cam.SmallShake (0.2f);
        target.y = transform.position.y;
        Instantiate (bullet, gunObj.transform.position, Quaternion.LookRotation(-(transform.position - target),Vector3.up));
    }

    void FinalMove () {
        cc.Move (moveV3 * Time.deltaTime);
    }
}