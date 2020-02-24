using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] bool phase2Active = false;
    Hitbox hitbox;
    [SerializeField] float maxHP = 1200;
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
    [SerializeField] LineRenderer gunLine;
    void Start () {
        player = FindObjectOfType<PlayerController> ();
        cc = GetComponent<CharacterController> ();
        cam = FindObjectOfType<PlayerCam> ();
        hitbox = GetComponent<Hitbox>();

        hitbox.hp = maxHP;
    }

    void Update () {
        switch (curState) {
            case State.Normal:
                // Walk (player.transform.position + (player.transform.forward * 3));
                AIBehaviour ();
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
                //Gun (player.transform.position);
                AIBehaviour ();
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
            //StartAttack ();
        }
        if (Input.GetKeyDown (KeyCode.Alpha7)) {
            // StartGun ();
        }
    }

    void NoDashCheck () {

    }

    void StartDash (float yAngle) {

        CancelInvoke ("Jab2Inv");
        CancelInvoke ("Jab3Inv");

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
        Invoke ("NoAttack", 0.5f);
        CancelInvoke("SetHitboxActive");
        if(curAIMode == AIMode.Ranged){
            curAIMode = AIMode.Aggresive;
            noShootMode = true;
        }
    }

    Vector3 oldPos;
    void StopDash () {
        Invoke ("NoAttack", 0.25f);
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
            if (IsInvoking ("Jab3Inv")) {
                Jab3 ();
            } else if (IsInvoking ("Jab2Inv")) {
                Jab2 ();
            } else {
                Jab1 ();
            }
            transform.rotation = Quaternion.LookRotation (-(new Vector3 (transform.position.x, player.transform.position.y, transform.position.z) - player.transform.position), Vector3.up);
        } else {
            curState = State.JumpSlam;
            moveV3.x = (transform.forward * curAccDec * walkSpeed).x;
            moveV3.z = (transform.forward * curAccDec * walkSpeed).z;
            moveV3.y = jumpStrength / 2;
            anim.Play ("TaicaJumpSlam");
        }
    }

    void Jab1 () {
        anim.Play ("TaicaAtkJab1");
        StartCoroutine (SetHitboxActive (0, 0.2f, 0.2f));
        Invoke ("Jab2Inv", 0.8f);
    }

    void Jab2 () {
        anim.Play ("TaicaAtkJab2");
        StartCoroutine (SetHitboxActive (0, 0.4f, 0.2f));
        Invoke ("Jab3Inv", 1);
    }

    void Jab3 () {
        anim.Play ("TaicaAtkJab3");
        StartCoroutine (SetHitboxActive (0, 0.3f, 0.3f));
    }

    void Jab2Inv () {

    }

    void Jab3Inv () {

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
        if (anim.GetCurrentAnimatorStateInfo (0).IsName ("JumpSlam") == true) {
            anim.Play ("TaicaIdle");
        }
    }

    void Gravity () {
        if (moveV3.y > 0) {
            moveV3.y = Mathf.MoveTowards (moveV3.y, gravityStrength, Time.deltaTime * gravitySpeed);
        } else {
            moveV3.y = Mathf.MoveTowards (moveV3.y, gravityStrength, Time.deltaTime * gravitySpeed / 2);

        }

        if (Input.GetKeyDown (KeyCode.Alpha9) == true) {
            // Jump();
        }
        anim.SetBool ("grounded", IsGrounded ());
    }

    void Jump () {
        if (IsGrounded () == true) {
            moveV3.y = jumpStrength;
        }
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
        Invoke ("NoShoot", 0.3f);
        curAccDec = 0;
        anim.Play ("TaicaStartGun");

        spearObj.SetActive (false);
        gunObj.SetActive (true);

        curState = State.Gun;
    }

    void Gun (Vector3 targetPos) {

        targetPos.y = transform.position.y;

        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (-(transform.position - targetPos), Vector3.up), Time.deltaTime * 25);

        if (Input.GetKeyDown (KeyCode.Alpha7)) {
            //StopGun ();
        }

        if (Input.GetKeyDown (KeyCode.Alpha8)) {
            //  GunShot (targetPos);
        }

        gunLine.SetPosition (0, gunObj.transform.position);
        gunLine.SetPosition (1, gunObj.transform.position + (-(gunObj.transform.position - targetPos) * 1000));
        gunLine.SetPosition (1, new Vector3 (gunLine.GetPosition (1).x, gunLine.GetPosition (0).y, gunLine.GetPosition (1).z));
    }

    void StopGun () {
        anim.Play ("TaicaEndGun");
        Invoke ("SetStateNormal", 0.3f);

        spearObj.SetActive (true);
        gunObj.SetActive (false);
    }

    void GunShot (Vector3 target) {
        anim.Play ("TaicaGunShoot", 0, 0f);

        FindObjectOfType<TimescaleManager> ().SlowMo (0.1f, 0.05f);

        Instantiate (hitEffectParticle, gunObj.transform.position, Quaternion.identity);
        SpawnAudio.AudioSpawn (dashAudio, 0, Random.Range (2.5f, 3), 1);
        cam.SmallShake (0.2f);
        target.y = transform.position.y;
        Instantiate (bullet, gunObj.transform.position, Quaternion.LookRotation (-(transform.position - target), Vector3.up));
    }

    void FinalMove () {
        cc.Move (moveV3 * Time.deltaTime);
    }

    //
    // A I
    //
    public enum AIMode {
        Aggresive,
        Defensive,
        Ranged
    }

    [Header ("AI")]
    public AIMode curAIMode = AIMode.Aggresive;
    [Header ("Ranged")]
    [SerializeField] Transform rangedNodesParent;

    void AIBehaviour () {
        SetAIMode();
        switch (curAIMode) {
            case AIMode.Ranged:
                RangedAI ();
                break;
            case AIMode.Aggresive:
                AggresiveAI ();
                break;
            case AIMode.Defensive:
                DefensiveAI ();
                break;
        }
    }

    Vector3 target = Vector3.zero;
    void RangedAI () {
        if (Vector3.Distance (transform.position, player.transform.position) < 12) {
            if (curState == State.Gun) {
                StopGun ();
            } else if (curState == State.Normal) {
                Walk (GetFurthestNode (rangedNodesParent.GetComponentsInChildren<Transform> ()));
            }
        } else {
            if (curState == State.Normal) {
                StartGun ();
                target = player.transform.position;
            } else if (curState == State.Gun) {
                Gun (target);
                if (IsInvoking ("NoShoot") == false) {
                    target = player.transform.position;
                    if (phase2Active == true && Random.Range (0, 2) != 0) {
                        target += player.transform.forward * 5;
                    }
                    Invoke ("NoShoot", 0.3f);
                    GunShot (target);
                }
            }
        }
    }

    void AggresiveAI () {
        if (curState == State.Normal) {
            if (Vector3.Distance (player.transform.position, transform.position) > 3) {
                if (player.curState != PlayerController.State.SlamAttack && player.curState != PlayerController.State.Attack) {
                    if (player.isGrounded == true) {
                        Walk (player.transform.position);
                    } else {
                        Walk (player.transform.position + (transform.position - player.transform.position).normalized * 3);
                    }
                } else if (Vector3.Distance (player.transform.position, transform.position) < 6) {
                    StartDash (Quaternion.LookRotation (transform.position - player.transform.position, Vector3.up).eulerAngles.y);
                } else {
                    Walk (transform.position);
                }
            } else if (IsInvoking ("NoAttack") == false) {
                if (phase2Active == false) {
                    Invoke ("NoAttack", 0.5f);
                }
                if (Random.Range (0, 100) < 70) {
                    StartAttack ();
                }
            } else {
                Walk (player.transform.position + player.transform.forward * 3);
            }
        }
    }

    void NoAttack () {

    }
    void NoShoot () {

    }

    Vector3 GetFurthestNode (Transform[] posses) {
        List<float> distances = new List<float> ();
        for (int i = 0; i < posses.Length; i++) {
            distances.Add (Vector3.Distance (player.transform.position, posses[i].position));
        }

       return posses[distances.IndexOf(Mathf.Max(distances.ToArray()))].position;
    }

    void DefensiveAI () {
        switch (player.curState) {
            case PlayerController.State.Normal:

                if (player.isGrounded == true) {
                    Walk (player.transform.position - player.transform.forward);
                } else {
                    Walk (player.transform.position + (transform.position - player.transform.position).normalized * 10);
                }

                break;

            case PlayerController.State.SlamAttack:
                if (Vector3.Distance (transform.position, player.transform.position) < 6) {
                    StartDash (Quaternion.LookRotation (transform.position - player.transform.position, Vector3.up).eulerAngles.y);
                } else {
                    Walk (player.transform.position + (transform.position - player.transform.position).normalized * 7);
                }
                break;

            case PlayerController.State.Dash:
                if (Vector3.Distance (transform.position, player.transform.position) > 5) {
                    transform.LookAt (new Vector3 (player.transform.position.x + (player.transform.forward.x * 15), transform.position.y, player.transform.position.z + (player.transform.forward.z * 15)));
                    StartDash (transform.eulerAngles.y + Random.Range (-10, 10));
                }
                break;

            case PlayerController.State.Gun:
                if (Vector3.Distance (transform.position, new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z)) > 5) {

                    Jump ();
                    Walk (player.transform.position);
                } else {
                    if (IsGrounded () == true) {
                        Jump ();
                        Walk (player.transform.position);
                    } else if (moveV3.y < 0) {
                        curAccDec = 0;
                        StartAttack ();
                    }
                }
                break;

            case PlayerController.State.Parry:

                Walk (player.transform.position);
                if (curAccDec == 0) {
                    StartAttack ();
                }

                break;

            case PlayerController.State.Attack:
                Walk (player.transform.position);
                if (curAccDec == 0) {
                    StartAttack ();
                }
                break;
        }
    }

    bool noShootMode = false;
    void SetAIMode(){
        if(hitbox.hp < maxHP / 2){
            phase2Active = true;
        }

        if(hitbox.hp > maxHP - (maxHP / 10)){
            curAIMode = AIMode.Defensive;
        } else if(hitbox.hp > maxHP / 2){
            curAIMode = AIMode.Aggresive;
        } else if(hitbox.hp > maxHP / 4){
            if(noShootMode == false){
            curAIMode = AIMode.Ranged;
            } else {
                curAIMode = AIMode.Aggresive;
            }
        } else {
            curAIMode = AIMode.Aggresive;
        }


    }
}