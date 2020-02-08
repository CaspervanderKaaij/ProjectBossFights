using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikaBoss : MonoBehaviour {
    Collider myHitbox;
    [SerializeField] Transform barrierPointsParent;
    public enum State {
        Idle,
        Attacking
    }
    public State curState = State.Idle;

    public enum BarrierState {
        NoOrbs,
        Orbs,
        Desroyed
    }
    public BarrierState barrierState = BarrierState.NoOrbs;
    [SerializeField] GameObject orbsPrefab;
    [SerializeField] Transform lineFollowTrans;
    bool isAttacking = false;
    PlayerController player;
    PlayerCam cam;
    [SerializeField] Vector3 centerPos;
    [SerializeField] Animator anim;
    [SerializeField] float camXBase = 20;
    [Header ("Memory inferno")]
    [SerializeField] GameObject[] memoryInfernoHitboxes = new GameObject[3];
    [SerializeField] GameObject[] memoryInfernoIndicators = new GameObject[3];
    [SerializeField] GameObject memoryInfernoIdicatorParticle;
    [Header ("TeleSlash")]
    [SerializeField] GameObject teleslashHitbox;
    [SerializeField] GameObject jumpslashHitbox;
    [SerializeField] GameObject blackholeSlashHitbox;
    [Header ("Reality Slash")]
    [SerializeField] GameObject realitySlashHitbox;
    [Header ("Black Hole")]
    [SerializeField] GameObject blackholePrefab;

    void Start () {
        myHitbox = GetComponent<Collider> ();
        memoryInfernoPattern = new int[10];
        for (int i = 0; i < 10; i++) {
            memoryInfernoPattern[i] = Random.Range (0, 3);
        }
        player = FindObjectOfType<PlayerController> ();
        cam = FindObjectOfType<PlayerCam> ();
        if (barrierState == BarrierState.Desroyed) {
            lastBState = BarrierState.NoOrbs;
        } else {
            lastBState = BarrierState.Desroyed;
        }

    }

    void Update () {
        UpdateBarrierActive ();
        DebugInput ();
        SetOrbState ();
        SetCam ();
    }

    void RunFromPlayer () {
        if (anim.GetCurrentAnimatorStateInfo (0).IsName ("LoseBarrier") == false) {
            // print ("y ar u runin");
        }
    }

    BarrierState lastBState;
    void SetOrbState () {
        if (barrierState != lastBState) {
            print ("bState");
            switch (barrierState) {
                case BarrierState.Desroyed:
                    Invoke ("BarrierBack", 5);
                    break;
                case BarrierState.NoOrbs:
                    Invoke ("NewOrbs", 5);
                    break;
            }
            lastBState = barrierState;
        }
    }

    void BarrierBack () {
        barrierState = BarrierState.NoOrbs;
    }

    void NewOrbs () {
        barrierState = BarrierState.Orbs;
        barrierPointsParent = Instantiate (orbsPrefab, transform.position, Quaternion.identity).transform;
    }

    float camX = 20;
    void SetCam () {
        cam.angleGoal.x = camX;
        cam.angleGoal.y = Quaternion.LookRotation (transform.position - cam.transform.position, Vector3.up).eulerAngles.y;
        cam.offset = cam.transform.forward * -10 + cam.transform.right * 2;
    }

    void DebugInput () {
        if (Input.GetKeyDown (KeyCode.Tab) == true) {
            StartAttack (State.Attacking, "MemoryInferno"); //              --> activate attack <--
        }

        if (barrierState != BarrierState.Desroyed) {
            //check the phase, then attack
            SetOrbLineRends ();
        } else {
            RunFromPlayer ();
        }
    }

    void SetOrbLineRends () {
        if (barrierPointsParent != null) {
            LineRenderer[] lines = barrierPointsParent.GetComponentsInChildren<LineRenderer> ();
            for (int i = 0; i < lines.Length; i++) {
                lines[i].SetPosition (0, lines[i].transform.position);
                lines[i].SetPosition (1, lineFollowTrans.position);
            }
        }
    }

    void UpdateBarrierActive () {
        if (barrierPointsParent != null) {
            SetHitboxActive ((barrierPointsParent.childCount <= 0 && barrierState != BarrierState.NoOrbs));
            if (barrierPointsParent.childCount <= 0) {
                Destroy (barrierPointsParent.gameObject);
                barrierState = BarrierState.Desroyed;
            }
        } else {
            SetHitboxActive ((barrierState != BarrierState.NoOrbs));
        }
    }

    void SetHitboxActive (bool active) {
        bool wasActive = myHitbox.enabled;

        if (active == true && wasActive == false) {
            myHitbox.enabled = active;

            anim.Play ("LoseBarrier");
        }

        if (active == false && wasActive == true) {
            myHitbox.enabled = active;
        }

    }

    void StartAttack (State atk, string coroutineName) {
        if (isAttacking == false) {

            curState = atk;

            StartCoroutine (coroutineName);
            isAttacking = true;

        }
    }

    void StopAttack () {
        isAttacking = false;
        curState = State.Idle;
    }

    int[] memoryInfernoPattern;
    int memoryInfernoCOunt = 5;
    float memoryInfernoSpeedMulitplier = 2;
    IEnumerator MemoryInferno () {
        camX = 40;
        float startY = anim.transform.eulerAngles.y;
        yield return new WaitForSeconds (0.5f * memoryInfernoSpeedMulitplier);
        for (int i = 0; i < memoryInfernoCOunt; i++) {
            GameObject g = memoryInfernoIndicators[memoryInfernoPattern[i]];
            g.SetActive (true);
            anim.Play ("MikaMemoryInfernoPoint", 0, 0);
            anim.transform.eulerAngles = new Vector3 (anim.transform.eulerAngles.x, memoryInfernoHitboxes[memoryInfernoPattern[i]].transform.eulerAngles.y, anim.transform.eulerAngles.z);
            anim.transform.Rotate (0, -20, 0);
            Instantiate(memoryInfernoIdicatorParticle,transform.position + (-anim.transform.right * 3),Quaternion.identity);
            yield return new WaitForSeconds (0.2f * memoryInfernoSpeedMulitplier);
            memoryInfernoIndicators[memoryInfernoPattern[i]].SetActive (false);
            yield return new WaitForSeconds (0.05f * memoryInfernoSpeedMulitplier);
        }
        yield return new WaitForSeconds (0.5f * memoryInfernoSpeedMulitplier);
        anim.Play ("MikaEvilLaugh", 0, 0);
        anim.transform.eulerAngles = new Vector3 (anim.transform.eulerAngles.x, startY, anim.transform.eulerAngles.z);
        for (int i = 0; i < memoryInfernoCOunt; i++) {
            memoryInfernoHitboxes[memoryInfernoPattern[i]].SetActive (true);
            yield return new WaitForSeconds (0.45f * memoryInfernoSpeedMulitplier);
            memoryInfernoHitboxes[memoryInfernoPattern[i]].SetActive (false);
            yield return new WaitForSeconds (0.2f * memoryInfernoSpeedMulitplier);
        }
        anim.Play ("MikaStopEvilLaugh", 0, 0);
        yield return new WaitForSeconds (0.5f * memoryInfernoSpeedMulitplier);
        StopCoroutine ("PushPlayerToBH");
        camX = camXBase;
        StopAttack ();
    }

    IEnumerator TeleSlash () {
        yield return new WaitForSeconds (0.5f);
        anim.Play ("MikaTeleslashCharge");
        transform.position = player.transform.position + (Vector3.up * 2) - player.transform.forward * 5;
        transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        yield return new WaitForSeconds (0.1f);
        anim.Play ("MikaTeleslash");
        yield return new WaitForSeconds (0.2f);
        Instantiate (teleslashHitbox, transform.position + transform.forward, Quaternion.identity).transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        cam.MediumShake (0.2f);
        yield return new WaitForSeconds (0.8f);
        transform.position = centerPos;
        StopAttack ();
    }

    IEnumerator JumpSlash () {
        yield return new WaitForSeconds (0.2f);
        jumpSlashY = 45;
        JumpSlashMove ();
        float range = 3;
        yield return new WaitForSeconds (0.3f);
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds (0.1f);
            GameObject g = Instantiate (jumpslashHitbox, transform.position + transform.forward, Quaternion.identity);
            g.transform.LookAt (player.transform.position + player.transform.forward * -2);
            g.transform.Rotate (0, 0, Random.Range (0, 360));
            cam.MediumShake (0.2f);
        }
        yield return new WaitForSeconds (0.3f);
        CancelInvoke ("JumpSlashMove");
        JumpSlashFall ();
        yield return new WaitForSeconds (1.5f);
        CancelInvoke ("JumpSlashFall");
        StopAttack ();
    }

    float jumpSlashY;
    void JumpSlashMove () {
        transform.position += Vector3.up * Time.deltaTime * jumpSlashY;
        jumpSlashY = Mathf.MoveTowards (jumpSlashY, 0, Time.deltaTime * 100);
        Invoke ("JumpSlashMove", Time.deltaTime);
    }

    void JumpSlashFall () {
        transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, centerPos.y, transform.position.z), Time.deltaTime * 29);
        if (transform.position.y != centerPos.y) {
            Invoke ("JumpSlashFall", Time.deltaTime);
        }
    }

    IEnumerator BlackHole () {
        cc = player.GetComponent<CharacterController> ();
        anim.Play ("MikaBlackHoleStart");
        anim.transform.position += Vector3.up * 5;
        yield return new WaitForSeconds (1);

        anim.Play ("MikaBlackHole");

        bHoleStr = 0;
        StartCoroutine ("PushPlayerToBH");
        cam.SmallShake (2);
        GameObject bHole = Instantiate (blackholePrefab, transform.position, Quaternion.identity);
        bHole.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds (8);
        anim.transform.position += Vector3.up * -5;
        anim.Play ("MikaBlackHoleStop", 0, 0.8f);
        Destroy (bHole);
        StopCoroutine ("PushPlayerToBH");
        StopAttack ();

    }

    List<GameObject> realitySlashHitboxes = new List<GameObject> ();
    IEnumerator RealitySlash () {
        camX = 50;
        realitySlashHitboxes.Clear ();
        for (int i = 0; i < 5; i++) {
            GameObject g = Instantiate (realitySlashHitbox, transform.position, Quaternion.Euler (0, Random.Range (0, 360), 0));
            g.transform.position += g.transform.forward * Random.Range (3, 15);
            g.transform.Rotate (0, Random.Range (0, 360), 90);
            realitySlashHitboxes.Add (g);
            yield return new WaitForSeconds (0.3f);
        }
        yield return new WaitForSeconds (0.5f);
        for (int i = 0; i < 5; i++) {
            realitySlashHitboxes[i].GetComponent<Collider> ().enabled = true;
            Destroy (realitySlashHitboxes[i], 0.5f);
        }
        cam.HardShake (0.1f);
        yield return new WaitForSeconds (1);
        StopAttack ();
        camX = camXBase;
    }

    IEnumerator LastResort () {
        cc = player.GetComponent<CharacterController> ();
        for (int i = 0; i < memoryInfernoHitboxes.Length; i++) {
            memoryInfernoHitboxes[i].GetComponent<Hurtbox> ().damage = JustDontGetHitAndItWillBeFine ();
        }
        yield return new WaitForSeconds (1);
        bHoleStr = 0;
        StartCoroutine ("PushPlayerToBH");
        StartCoroutine (MemoryInferno ());
    }

    float JustDontGetHitAndItWillBeFine () {
        return Mathf.Infinity;
    }

    CharacterController cc;
    float bHoleStr = 0;
    IEnumerator PushPlayerToBH () {
        Vector3 dir = -(player.transform.position - transform.position).normalized;
        dir.y = 0;
        cc.Move (dir * bHoleStr * Time.deltaTime);
        bHoleStr = Mathf.MoveTowards (bHoleStr, 25, Time.deltaTime * 8);
        yield return new WaitForSeconds (0);
        StartCoroutine ("PushPlayerToBH");
    }
}