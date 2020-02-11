﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] GameObject[] orbsPrefab;
    [SerializeField] Transform lineFollowTrans;
    PlayerController player;
    PlayerCam cam;
    [SerializeField] Vector3 centerPos;
    [SerializeField] Animator anim;
    [SerializeField] float camXBase = 20;

    [SerializeField] Hitbox hp;
    [SerializeField] float maxHp = 1200;
    [Header ("Memory inferno")]
    [SerializeField] GameObject[] memoryInfernoHitboxes = new GameObject[3];
    [SerializeField] GameObject[] memoryInfernoIndicators = new GameObject[3];
    [SerializeField] GameObject memoryInfernoIdicatorParticle;
    [SerializeField] GameObject memoryInfernoStartParticle;
    [SerializeField] MikaMemPattern[] memPatterns;
    [Header ("TeleSlash")]
    [SerializeField] GameObject teleslashHitbox;
    [SerializeField] GameObject jumpslashHitbox;
    [SerializeField] GameObject blackholeSlashHitbox;
    [SerializeField] GameObject teleportParicle;
    [Header ("Reality Slash")]
    [SerializeField] GameObject realitySlashHitbox;
    [Header ("Black Hole")]
    [SerializeField] GameObject blackholePrefab;
    [Header ("Spatialist Friend")]
    [SerializeField] GameObject spatialistPortal;
    [SerializeField] Material spatialistLineMat;
    [SerializeField] Material spatialistMikaMat;
    [SerializeField] float spatialistChargeTime = 0.3f;
    [SerializeField] float spatialistAttackTime = 0.3f;
    [SerializeField] int spatialistPortals = 30;

    [Header ("Pandemonim")]
    [SerializeField] GameObject snekwurmPrefab;
    [Header ("Gluttony")]
    [SerializeField] GameObject gluttonySnekwurm;
    [Header ("CenterOfTheUniverse")]
    [SerializeField] GameObject centeroftheuniverseProjectile;

    void Start () {
        myHitbox = GetComponent<Collider> ();

        memPattern = Random.Range (0, memPatterns.Length);

        player = FindObjectOfType<PlayerController> ();
        cam = FindObjectOfType<PlayerCam> ();
        if (barrierState == BarrierState.Desroyed) {
            lastBState = BarrierState.NoOrbs;
        } else {
            lastBState = BarrierState.Desroyed;
        }

        cc = FindObjectOfType<PlayerController> ().GetComponent<CharacterController> ();

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
                    cam.Flash (Color.white, 7.5f);
                    Invoke ("BarrierBack", 5);
                    break;
                case BarrierState.NoOrbs:
                    Invoke ("NewOrbs", 0.1f);
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
        if (hp.hp > (maxHp / 30) * 2) {
            //  phase 1
            barrierPointsParent = Instantiate (orbsPrefab[0], transform.position, Quaternion.identity).transform;
        } else if (hp.hp > maxHp / 30) {
            // phase 2
            barrierPointsParent = Instantiate (orbsPrefab[1], transform.position, Quaternion.identity).transform;

        } else {
            //phase 3
            barrierPointsParent = Instantiate (orbsPrefab[2], transform.position, Quaternion.identity).transform;

        }
    }

    float camX = 20;
    void SetCam () {
        cam.angleGoal.x = camX;
        cam.angleGoal.y = Quaternion.LookRotation (transform.position - cam.transform.position, Vector3.up).eulerAngles.y;
        cam.offset = cam.transform.forward * -10 + cam.transform.right * 2;
    }

    void DebugInput () {
        if (Input.GetKeyDown (KeyCode.Tab) == true) {
            StartAttack (State.Attacking, "SpatialistFriend"); //                                                                              --> activate attack <--
            //MemoryInferno
            //RealitySlash
            //Gluttony
            //CenterOfTheUniverse
            //SpatialistFriend
            //Pandemonim
            //TeleSlash
            //BlackHole

            /*

            if (hp.hp > (maxHp / 30) * 2) {
                print ("phase 1");
                ActivatePhase2Attack ();
            } else if (hp.hp > maxHp / 30) {
                print ("phase 2");
                ActivatePhase2Attack ();
            } else {
                print ("phase 3");
                ActivatePhase3Attack ();
            }

             */
        }

        if (barrierState != BarrierState.Desroyed) {
            //check the phase, then attack
            SetOrbLineRends ();
        } else {
            RunFromPlayer ();
        }
    }

    void ActivatePhase1Attack () {
        int rng = Random.Range (0, 3);
        switch (rng) {
            case 0:
                StartAttack (State.Attacking, "TeleSlash");
                break;
            case 1:
                StartAttack (State.Attacking, "MemoryInferno");
                break;
            case 2:
                StartAttack (State.Attacking, "RealitySlash");
                break;
        }
    }

    void ActivatePhase2Attack () {
        int rng = Random.Range (0, 4);
        switch (rng) {
            case 0:
                StartAttack (State.Attacking, "TeleSlash");
                break;
            case 1:
                StartAttack (State.Attacking, "MemoryInferno");
                break;
            case 2:
                StartAttack (State.Attacking, "RealitySlash");
                break;
            case 3:
                StartAttack (State.Attacking, "BlackHole");
                break;
        }
    }

    void ActivatePhase3Attack () {

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
        if (curState != State.Attacking) {

            curState = atk;

            StartCoroutine (coroutineName);

        }
    }

    void StopAttack () {
        curState = State.Idle;
    }

    int memPattern;
    int memoryInfernoCOunt = 1;
    float memoryInfernoSpeedMulitplier = 2;
    IEnumerator MemoryInferno () {
        camX = 40;
        float startY = anim.transform.eulerAngles.y;
        yield return new WaitForSeconds (0.5f * memoryInfernoSpeedMulitplier);
        for (int i = 0; i < memoryInfernoCOunt; i++) {
            int m = memPatterns[memPattern].pattern[i];
            GameObject g = memoryInfernoIndicators[m];
            g.SetActive (true);
            anim.Play ("MikaMemoryInfernoPoint", 0, 0);
            anim.transform.eulerAngles = new Vector3 (anim.transform.eulerAngles.x, memoryInfernoHitboxes[m].transform.eulerAngles.y, anim.transform.eulerAngles.z);
            anim.transform.Rotate (0, -20, 0);
            Instantiate (memoryInfernoIdicatorParticle, transform.position + (-anim.transform.right * 3), Quaternion.identity);
            yield return new WaitForSeconds (0.2f * memoryInfernoSpeedMulitplier);
            memoryInfernoIndicators[m].SetActive (false);
            yield return new WaitForSeconds (0.05f * memoryInfernoSpeedMulitplier);
        }
        yield return new WaitForSeconds (0.5f * memoryInfernoSpeedMulitplier);
        anim.Play ("MikaEvilLaugh", 0, 0);
        yield return new WaitForSeconds (0.3f);
        anim.transform.eulerAngles = new Vector3 (anim.transform.eulerAngles.x, startY, anim.transform.eulerAngles.z);
        for (int i = 0; i < memoryInfernoCOunt; i++) {
            int m = memPatterns[memPattern].pattern[i];
            Instantiate (memoryInfernoStartParticle, transform.position, memoryInfernoHitboxes[m].transform.rotation * Quaternion.Euler (90, 0, 0));
            yield return new WaitForSeconds (0.4f);
            memoryInfernoHitboxes[m].SetActive (true);
            yield return new WaitForSeconds (0.45f * memoryInfernoSpeedMulitplier);
            memoryInfernoHitboxes[m].SetActive (false);
            yield return new WaitForSeconds (0.2f * memoryInfernoSpeedMulitplier);
        }
        anim.Play ("MikaStopEvilLaugh", 0, 0);
        yield return new WaitForSeconds (0.5f * memoryInfernoSpeedMulitplier);
        StopCoroutine ("PushPlayerToBH");
        camX = camXBase;
        StopAttack ();

        memoryInfernoCOunt = Mathf.Min (memoryInfernoCOunt + 1, 10);
    }

    IEnumerator TeleSlash () {
        DisableOrbs (false);
        Instantiate (teleportParicle, transform.position, Quaternion.identity);
        Vector3 oldScale = anim.transform.localScale;
        anim.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds (0.75f);
        Vector3 savedPos = player.transform.position + (Vector3.up * 1) - player.transform.forward * 5;
        Instantiate (teleportParicle, savedPos, Quaternion.identity);
        transform.position = savedPos;
        anim.transform.localScale = oldScale;
        anim.Play ("MikaTeleslashCharge");
        transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        yield return new WaitForSeconds (0.1f);
        anim.Play ("MikaTeleslash");
        yield return new WaitForSeconds (0.15f);
        Instantiate (teleslashHitbox, transform.position + transform.forward, Quaternion.identity).transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        cam.MediumShake (0.2f);
        yield return new WaitForSeconds (0.8f);
        Instantiate (teleportParicle, transform.position, Quaternion.identity);
        anim.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds (0.5f);
        Instantiate (teleportParicle, centerPos, Quaternion.identity);
        yield return new WaitForSeconds (0.1f);
        anim.transform.localScale = oldScale;
        DisableOrbs (true);
        transform.position = centerPos;
        yield return new WaitForSeconds (0.3f);
        StopAttack ();
    }

    void DisableOrbs (bool able) {
        if (barrierPointsParent != null) {
            barrierPointsParent.gameObject.SetActive (able);
        }
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
        int amount = 15;
        yield return new WaitForSeconds (0.5f);
        realitySlashHitboxes.Clear ();
        for (int i = 0; i < amount; i++) {
            GameObject g = Instantiate (realitySlashHitbox, transform.position, Quaternion.Euler (0, Random.Range (0, 360), 0));
            g.transform.position += g.transform.forward * Random.Range (3, 20);
            g.transform.Rotate (180, Random.Range (0, 360), 90);
            realitySlashHitboxes.Add (g);
            yield return new WaitForSeconds (0.1f);
        }
        FindObjectOfType<TimescaleManager> ().SlowMo (0.5f, 0.1f);
        yield return new WaitForSeconds (0.1f);
        for (int i = 0; i < amount; i++) {
            realitySlashHitboxes[i].GetComponent<Collider> ().enabled = true;
            realitySlashHitboxes[i].GetComponent<LerpShaderValue> ().SetValue (1);
            realitySlashHitboxes[i].transform.GetChild (0).gameObject.SetActive (false);
            realitySlashHitboxes[i].transform.GetChild (1).gameObject.SetActive (true);
            Destroy (realitySlashHitboxes[i], 1);
        }
        cam.HardShake (0.1f);
        yield return new WaitForSeconds (1);
        StopAttack ();
        camX = camXBase;
    }

    IEnumerator SpatialistFriend () {
        DisableOrbs (false);
        int pairsToSpawn = spatialistPortals;
        List<int> pathOrder = new List<int> ();
        for (int i = 0; i < pairsToSpawn * 2; i++) {
            pathOrder.Add (i);
        }
        pathOrder = pathOrder.OrderBy (x => Random.value).ToList ();

        List<GameObject> portals = new List<GameObject> ();
        float curRot = 0;
        for (int i = 0; i < pairsToSpawn * 2; i++) {
            portals.Add (Instantiate (spatialistPortal, centerPos, Quaternion.Euler (0, curRot, 0)));
            portals[i].transform.position -= portals[i].transform.forward * 30;
            curRot += 180 / pairsToSpawn;
            yield return new WaitForEndOfFrame ();
        }

        int curPortal = Random.Range (0, pairsToSpawn * 2);
        Vector3 oldScale = anim.transform.localScale;
        CreateSpatialistLine (transform.position, portals[curPortal].transform.position, spatialistAttackTime, false, spatialistLineMat);
        yield return new WaitForSeconds (spatialistChargeTime);
        //fist hurtbox
        anim.transform.localScale = Vector3.zero;
        CreateSpatialistLine (transform.position, portals[curPortal].transform.position, spatialistAttackTime, true, spatialistMikaMat);
        yield return new WaitForSeconds (spatialistChargeTime);
        for (int i = 0; i < pairsToSpawn; i += 2) {
            curPortal = Random.Range (0, pairsToSpawn * 2);
            CreateSpatialistLine (portals[curPortal].transform.position, portals[(int) Mathf.Repeat (curPortal + pairsToSpawn, pairsToSpawn * 2)].transform.position, spatialistChargeTime * 2, false, spatialistLineMat);
            yield return new WaitForSeconds (spatialistChargeTime * 2);
            //hurtbox
            CreateSpatialistLine (portals[curPortal].transform.position, portals[(int) Mathf.Repeat (curPortal + pairsToSpawn, pairsToSpawn * 2)].transform.position, spatialistAttackTime / 2, true, spatialistMikaMat);
            yield return new WaitForSeconds (spatialistChargeTime);

        }
        CreateSpatialistLine (portals[curPortal].transform.position, transform.position, spatialistAttackTime, false, spatialistLineMat);
        yield return new WaitForSeconds (spatialistChargeTime);
        CreateSpatialistLine (portals[curPortal].transform.position, transform.position, spatialistAttackTime, true, spatialistMikaMat);

        yield return new WaitForSeconds (1);
        for (int i = 0; i < portals.Count; i++) {
            Destroy (portals[i]);
        }
        anim.transform.localScale = oldScale;
        DisableOrbs (true);
        StopAttack ();
    }

    void CreateSpatialistLine (Vector3 startPos, Vector3 endPos, float destroyTime, bool hasHurtbox, Material mat) {
        GameObject startLine = new GameObject ();
        LineRenderer predicStartLine = startLine.AddComponent<LineRenderer> ();
        predicStartLine.SetPosition (0, startPos);
        predicStartLine.SetPosition (1, endPos);

        predicStartLine.material = mat;
        predicStartLine.startWidth = 0.1f;
        predicStartLine.endWidth = 0.1f;
        predicStartLine.textureMode = LineTextureMode.Tile;

        if (hasHurtbox == true) {
            predicStartLine.gameObject.AddComponent<LineHurtbox> ().line = predicStartLine;
            predicStartLine.gameObject.GetComponent<LineHurtbox> ().hurtbox = predicStartLine.GetComponent<Hurtbox> ();
            Hurtbox h = predicStartLine.GetComponent<Hurtbox> ();

            predicStartLine.startWidth = 2;
            predicStartLine.endWidth = 2;

            cam.MediumShake (0.1f);

            h.team = 2;
            h.damage = 45;
        }

        Destroy (startLine, destroyTime);
    }

    IEnumerator Pandemonim () {
        GameObject snek = Instantiate (snekwurmPrefab, player.transform.position + Vector3.up + -player.transform.forward, Quaternion.Euler (90, player.angleGoal, 0));
        snek.GetComponent<MikaSnekwurm> ().StartCoroutine (snek.GetComponent<MikaSnekwurm> ().DeathEv (20));
        yield return new WaitForSeconds (2.5f);
        StopAttack ();
    }

    IEnumerator Gluttony () {
        Instantiate (gluttonySnekwurm, new Vector3 (player.transform.position.x, centerPos.y - 1, player.transform.position.z), Quaternion.identity);
        yield return new WaitForSeconds (0.2f);
        StopAttack ();
    }

    IEnumerator CenterOfTheUniverse () {
        anim.Play ("MikaBlackHoleStart");
        anim.transform.position += Vector3.up * 5;
        yield return new WaitForSeconds (1);

        anim.Play ("MikaBlackHole");

        bHoleStr = 0;
        StartCoroutine ("PushPlayerToBH");
        cam.SmallShake (2);
        GameObject bHole = Instantiate (blackholePrefab, transform.position, Quaternion.identity);
        bHole.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds (1f);
        for (int i = 0; i < 14; i++) {
            Instantiate (centeroftheuniverseProjectile, new Vector3 (player.transform.position.x, centerPos.y - 1, player.transform.position.z), Quaternion.identity);
            yield return new WaitForSeconds (0.5f);
        }

        anim.transform.position += Vector3.up * -5;
        anim.Play ("MikaBlackHoleStop", 0, 0.8f);
        Destroy (bHole);
        StopCoroutine ("PushPlayerToBH");
        StopAttack ();
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

[System.Serializable]
public class MikaMemPattern {
    public int[] pattern;
}