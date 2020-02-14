using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchWestBoss : MonoBehaviour {

    bool isAttacking = false;
    [SerializeField] Hitbox hp;
    public enum State {
        Idle,
        Attacking,
        FinalAttack
    }
    public State curState = State.Idle;
    PlayerController player;
    public float floorYCoods = 0;
    [Header ("Intro")]
    [SerializeField] GameObject introCam;
    [SerializeField] Camera introCamc;
    [Header ("GroundLaser")]
    public GameObject groundLaserPrefab;
    [Header ("GroundLaserPhase2")]
    public GameObject aimGroundLaserPrefab;
    [Header ("GroundLaserPhase3")]
    public GameObject groundLaser3Prefab;
    [Header ("LaserCircleAttack")]
    public GameObject laserCirclePrefab;
    public GameObject laserP2CirclePrefab;
    [Header ("ShootAttack")]
    public GameObject shooterPrefab;
    public GameObject shooterPrefab2;
    [Header ("Barrier")]
    [SerializeField] GameObject barrier;
    Hitbox hitbox;
    [SerializeField] GameObject barrierParticle;
    [SerializeField] GameObject barrierBreakParticle;
    [SerializeField] GameObject barrierBackTelegraphParticle;
    [SerializeField] AudioClip barrierClip;
    [SerializeField] AudioClip barrierClip2;
    [SerializeField] SpriteRenderer testFace;
    [SerializeField] Sprite[] testFaces;
    PlayerCam cam;
    [SerializeField] Renderer[] shieldMats;
    [SerializeField] float camXRotation = 30;
    [SerializeField] AudioClip finalAttackChargeClip;
    [Header ("VoiceLines")]
    [SerializeField] AudioClip[] barrierVoiceBackAudio;
    [SerializeField] AudioClip[] barrierDownVoice;
    [SerializeField] AudioClip[] phase3Start;
    [SerializeField] AudioClip[] phase2Start;
    [SerializeField] AudioClip[] phase1Start;
    [SerializeField] AudioClip[] finalAttackVoice;
    [SerializeField] AudioClip[] deathVoice;
    [SerializeField] AudioClip[] laserCircleVoice;
    [SerializeField] AudioClip[] quickAttackP1Voice;
    [SerializeField] AudioClip[] surroundPlayerP1Voice;
    [SerializeField] AudioClip[] rapidLaserP2;
    [SerializeField] AudioClip[] aroundWitchP2;
    [SerializeField] AudioClip[] groundLaserP3Voice;
    [SerializeField] AudioClip[] rapidFireP3Voice;
    [SerializeField] AudioClip[] RatataP3Voice;
    [SerializeField] AudioClip[] Phase3Attacks;
    [SerializeField] AudioClip[] taunts;
    [SerializeField] AudioClip[] getHitVoices;
    [SerializeField] AudioClip ratataLoopSound;
    [SerializeField] AudioClip groundLaserBassBoost;
    [SerializeField] AudioClip introSound;
    [SerializeField] AudioClip bozingBellSound;
    GameObject curVoice;
    [Header ("Animations")]
    [SerializeField] Animator anim;
    [SerializeField] AutoRotate animRotator;
    [SerializeField] Transform heinzModel;

    void Start () {
        player = FindObjectOfType<PlayerController> ();
        hitbox = GetComponent<Hitbox> ();
        cam = player.cameraTransform.GetComponent<PlayerCam> ();
        barrierHBox = barrier.GetComponent<Hurtbox> ();
        barrierHBox.enabled = false;
        SetBarrierActive (true);
        InvokeRepeating ("Taunt", 20, 20);
        Invoke ("StartWait", 3);
        StartCoroutine (IntroEv ());
    }

    void StartWait () {

    }

    IEnumerator IntroEv () {
        anim.Play ("HeinzIntro",0,0.45f);
        yield return new WaitForSeconds (0.4f);
        FindObjectOfType<TimescaleManager> ().SlowMo (0.9f, 0.5f);
        yield return new WaitForSeconds (0.1f);
        introCam.SetActive (true);
        SpawnAudio.AudioSpawn(introSound,0,2,1);//bozingBellSound
        for (int i = 0; i < 60; i++) {
            introCamc.fieldOfView = Mathf.MoveTowards (introCamc.fieldOfView, 70, Time.unscaledDeltaTime * 1000);
            yield return new WaitForEndOfFrame ();
        }
        introCamc.fieldOfView = 70;
        yield return new WaitForSeconds (1);
        FindObjectOfType<TimescaleManager> ().SlowMo (0.9f, 0.5f);
        anim.Play ("HeinzIdle");
        Talk (phase1Start);
        introCam.SetActive (false);
        cam.Flash (Color.white, 5);
        SpawnAudio.AudioSpawn(bozingBellSound,0,1,0.3f);
        yield return new WaitForSeconds(0.1f);
        SpawnAudio.AudioSpawn(bozingBellSound,0,1,0.3f);
        yield return new WaitForSeconds(0.1f);
        SpawnAudio.AudioSpawn(bozingBellSound,0,1,0.3f);
    }

    void Taunt () {
        if (barrier.activeSelf == true && curState != State.FinalAttack) {
            TalkIfNotTalking (taunts);
        }
    }
    public void GetHitSound () {
        Talk (getHitVoices);
    }

    void Update () {
        if (IsInvoking ("StartWait") == false) {
            DebugInput ();
        }
        SetCam ();
        SetBarrierVisibility ();
    }

    int lastPhase = 1;
    void DebugInput () {
        if (isAttacking == true) {
            hitbox.enabled = false;
        } else if (barrier.activeSelf == false) {
            hitbox.enabled = true;
        }
        if (curState != State.FinalAttack) {

            if (hitbox.hp > 800) {
                lastPhase = 1;
                StartPhase1Attack ();
            } else if (hitbox.hp > 350) {
                lastPhase = 2;
                StartPhase2Attack ();
            } else {
                lastPhase = 3;
                StartPhase3Attack ();
            }

        }

        IdleSmoother ();

        /*
        if (Input.GetKeyDown (KeyCode.Tab)) {
            StartAttack (State.Attacking, "GroundLaserPhase3Attack");
        }
         */
    }

    void IdleSmoother () {
        if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Idle") == true) {
            anim.SetLayerWeight (1, 1);
        } else {
            anim.SetLayerWeight (1, Mathf.MoveTowards (anim.GetLayerWeight (1), 0, Time.deltaTime * 3));
        }
    }

    public void ActivateFinalAttack () {
        if (curState != State.FinalAttack) {
            StopAllCoroutines ();
            curState = State.FinalAttack;
            StartCoroutine ("FinalAttack");
        }
    }
    IEnumerator FinalAttack () {

        Talk (finalAttackVoice);
        barrier.GetComponent<Hurtbox> ().damage = 0;
        SetBarrierActive (true);
        cam.HardShake (5);
        SmoothLookAtPlayer ();
        SpawnAudio.AudioSpawn (finalAttackChargeClip, 0, 1, 1);
        camXRotation = 30;

        Instantiate (barrierBackTelegraphParticle, barrier.transform.position, Quaternion.identity);
        yield return new WaitForSeconds (1);
        anim.Play ("HeinzFinalAttackIntro");
        Instantiate (barrierBackTelegraphParticle, barrier.transform.position, Quaternion.identity);
        yield return new WaitForSeconds (1);

        Instantiate (barrierBackTelegraphParticle, barrier.transform.position, Quaternion.identity);
        yield return new WaitForSeconds (1);
        Instantiate (barrierBackTelegraphParticle, barrier.transform.position, Quaternion.identity);
        yield return new WaitForSeconds (1.7f);
        CancelInvoke ("SmoothLookAtPlayer");

        StartCoroutine ("AimShootP3");
        yield return new WaitForSeconds (1);
        StartCoroutine ("SurroundPlayer3");
        yield return new WaitForSeconds (3);
        StartCoroutine ("LaserCircleP3Attack");
        StartCoroutine ("AimShootP3");
        yield return new WaitForSeconds (1);
        StartCoroutine ("AimShootP3");
        yield return new WaitForSeconds (5);
        StartCoroutine ("GroundLaserPhase3Attack");
        yield return new WaitForSeconds (6);
        StartCoroutine ("AroundWitch3");
        yield return new WaitForSeconds (1);
        StartCoroutine ("AimShootP2");
        yield return new WaitForSeconds (1);
        StartCoroutine ("AimShootP2");
        yield return new WaitForSeconds (1);
        StartAttack (State.Attacking, "AimShootP2");

        yield return new WaitForSeconds (3);
        StartCoroutine ("GroundLaserPhase2Attack"); //this
        animRotator.enabled = false;
        heinzModel.localEulerAngles = Vector3.zero;
        anim.Play ("HeinzShieldDown");

        yield return new WaitForSeconds (6);
        SetBarrierActive (false);
        yield return new WaitForSeconds (4);

        StartCoroutine ("SurroundPlayer");
        yield return new WaitForSeconds (0.6f);
        anim.Play ("HeinzDeath");
        yield return new WaitForSeconds (1.5f);
        Talk (deathVoice);

    }

    void SetCam () {
        cam.angleGoal.x = camXRotation;
        cam.angleGoal.y = Quaternion.LookRotation (transform.position - cam.transform.position, Vector3.up).eulerAngles.y;
        cam.offset = cam.transform.forward * -20;
    }

    Hurtbox barrierHBox;
    bool phase2Voiced = false;
    bool phase3Voiced = false;
    bool phase1Voiced = false;
    void SetBarrierActive (bool active) {
        bool wasActive = barrierHBox.enabled;
        //barrier.SetActive (active);
        barrierHBox.enabled = active;
        hitbox.enabled = !active;
        if (active == true && wasActive == false) {
            barrier.transform.localScale = Vector3.zero;
            FindObjectOfType<PlayerCam> ().SmallShake (0.3f);
            Invoke ("BarrierShake", 0.1f);
            barrier.GetComponent<AutoScale> ().enabled = true;
            if (curState != State.FinalAttack) {
                camXRotation = 30;
                if (phase2Voiced == false && hitbox.hp <= 800) {
                    phase2Voiced = true;
                    Talk (phase2Start);
                } else if (phase3Voiced == false && hitbox.hp <= 350) {
                    phase3Voiced = true;
                    Talk (phase3Start);
                } else if (phase1Voiced == false) {
                    phase1Voiced = true;
                } else {
                    Talk (barrierVoiceBackAudio);
                }
            }
        }
        if (active == false && wasActive == true) {
            Instantiate (barrierBreakParticle, barrier.transform.position, Quaternion.identity);
            if (curState != State.FinalAttack) {
                camXRotation = 50;
                Talk (barrierDownVoice);
                cam.Flash (Color.white, 10);
            }
            anim.Play ("HeinzShieldDown");
        }
    }

    void Talk (AudioClip[] clips) {
        AudioClip chosenClip = clips[Random.Range (0, clips.Length)];
        Destroy (curVoice);
        curVoice = SpawnAudio.SpawnVoice (chosenClip, 0, 1, 1, 0);
    }

    void TalkIfNotTalking (AudioClip[] clips) {
        if (curVoice == null) {
            Talk (clips);
        }
    }

    float curDissolve = 0;
    void SetBarrierVisibility () {
        if (barrierHBox.enabled == true) {
            curDissolve = Mathf.MoveTowards (curDissolve, 1, Time.deltaTime * 1.5f);
            for (int i = 0; i < shieldMats.Length; i++) {
                shieldMats[i].material.SetFloat ("_DissolveMultiplier", curDissolve);
            }
        } else {
            curDissolve = Mathf.MoveTowards (curDissolve, 0, Time.deltaTime * 1.5f);
            for (int i = 0; i < shieldMats.Length; i++) {
                shieldMats[i].material.SetFloat ("_DissolveMultiplier", curDissolve);
            }
        }
    }

    void BarrierShake () {
        FindObjectOfType<TimescaleManager> ().SlowMo (0.2f, 0.2f);
        FindObjectOfType<PlayerCam> ().HardShake (0.15f);
        Instantiate (barrierParticle, barrier.transform.position, Quaternion.identity);
        //barrier.GetComponent<AutoScale> ().enabled = false;
        //barrier.transform.localScale *= 2;
        SpawnAudio.AudioSpawn (barrierClip, 0, 1, 0.4f);
        SpawnAudio.AudioSpawn (barrierClip2, 0, 2, 1);
    }

    int lastAtk;
    void StartPhase1Attack () {
        if (isAttacking == false) {

            int rng = (int) Mathf.Repeat (lastAtk + Random.Range (1, 3), 6);
            lastAtk = rng;

            switch (rng) {
                case 0:
                    StartAttack (State.Attacking, "GroundLaserAttack");
                    break;
                case 1:
                    StartAttack (State.Attacking, "LaserCircleAttack");
                    break;
                case 2:
                    StartAttack (State.Attacking, "AimShootP1");
                    break;
                case 3:
                    StartAttack (State.Attacking, "SurroundPlayer");
                    break;
                case 4:
                    StartAttack (State.Attacking, "AroundWitch");
                    Invoke ("NoLaser", 20);
                    break;
            }
        }
    }

    void StartPhase2Attack () {
        if (isAttacking == false) {
            int rng = (int) Mathf.Repeat (lastAtk + Random.Range (1, 3), 6);
            lastAtk = rng;

            switch (rng) {
                case 0:
                    StartAttack (State.Attacking, "GroundLaserPhase2Attack");
                    break;
                case 1:
                    StartAttack (State.Attacking, "LaserCircleP2Attack");
                    break;
                case 2:
                    StartAttack (State.Attacking, "AimShootP2");
                    break;
                case 3:
                    StartAttack (State.Attacking, "SurroundPlayer2");
                    break;
                case 4:
                    StartAttack (State.Attacking, "AroundWitch2");
                    break;
            }
        }
    }

    void StartPhase3Attack () {
        if (isAttacking == false) {
            int rng = (int) Mathf.Repeat (lastAtk + Random.Range (1, 3), 6);
            lastAtk = rng;

            switch (rng) {
                case 0:
                    StartAttack (State.Attacking, "GroundLaserPhase3Attack");
                    break;
                case 1:
                    if (IsInvoking ("NoLaser") == false) {
                        StartAttack (State.Attacking, "LaserCircleP3Attack");
                    } else {
                        StartAttack (State.Attacking, "GroundLaserPhase3Attack");
                    }
                    break;
                case 2:
                    StartAttack (State.Attacking, "AimShootP3");
                    break;
                case 3:
                    StartAttack (State.Attacking, "SurroundPlayer3");
                    break;
                case 4:
                    StartAttack (State.Attacking, "AroundWitch3");
                    break;
            }
        }
    }

    void NoLaser () {

    }

    int curAtk = 0;
    void StartAttack (State atk, string coroutineName) {
        SetBarrierActive (!IsInvoking ("NoAttack"));
        if (isAttacking == false && IsInvoking ("NoAttack") == false) {
            if (curAtk < 5) {
                if (curState != State.FinalAttack) {
                    curAtk++;
                }
                if (curState != State.FinalAttack) {
                    curState = atk;
                }
                StartCoroutine (coroutineName);
                isAttacking = true;
            } else {
                curAtk = 0;
                Invoke ("TelegraphBarrierBack", 4);
                Invoke ("NoAttack", 5);
            }
        }
    }

    void NoAttack () {
        SetBarrierActive (true);
        isAttacking = true;
        Invoke ("StopAttack", 1);
    }

    void TelegraphBarrierBack () {
        Instantiate (barrierBackTelegraphParticle, barrier.transform.position, Quaternion.identity);
        if (curState != State.FinalAttack) {
            anim.Play ("HeinzShieldUp");
        }
    }

    IEnumerator GroundLaserAttack () { //here
        //start animation
        Vector3 spawnPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        float repeatSpeed = Random.Range (0.6f, 0.9f);
        anim.Play ("HeinzGroundLaserStart");
        yield return new WaitForSeconds (1);
        for (int i = 0; i < 5; i++) {
            anim.Play ("HeinzGroundLaser", 0, 0);
            spawnPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
            Instantiate (groundLaserPrefab, spawnPos, Quaternion.identity).GetComponent<WitchWestLaser> ().chargeTime *= 1.5f;
            yield return new WaitForSeconds (repeatSpeed);
        }
        StopAttack ();
    }

    IEnumerator GroundLaserPhase2Attack () {
        SmoothLookAtPlayer ();
        anim.Play ("HeinzLaserStart");
        yield return new WaitForSeconds (0.8f);
        TalkIfNotTalking (rapidLaserP2);
        float repeatSpeed = Random.Range (0.2f, 0.3f);
        for (int i = 0; i < 15; i++) {
            anim.Play ("HeinzLaser");
            yield return new WaitForSeconds (repeatSpeed);
            Instantiate (aimGroundLaserPrefab, transform.position + transform.forward * 3, transform.rotation * Quaternion.Euler (90, 0, 0));
        }
        yield return new WaitForSeconds (1);
        CancelInvoke ("SmoothLookAtPlayer");
        StopAttack ();
    }

    IEnumerator GroundLaserPhase3Attack () {
        anim.Play ("HeinzDoubleGroundLaserStart");
        yield return new WaitForSeconds (1);
        TalkIfNotTalking (groundLaserP3Voice);
        Vector3 centerPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        float rngRange = 20;
        SpawnAudio.AudioSpawn (groundLaserBassBoost, 0, 1, 1, 1);
        for (int i = 0; i < 5; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity).GetComponent<WitchWestLaser> ().clip = null;
        }
        yield return new WaitForSeconds (1f);
        SpawnAudio.AudioSpawn (groundLaserBassBoost, 0, 1.2f, 1, 1);

        anim.Play ("HeinzDoubleGroundLaser", 0, 0);
        centerPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        for (int i = 0; i < 10; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity).GetComponent<WitchWestLaser> ().clip = null;
        }
        yield return new WaitForSeconds (1);
        SpawnAudio.AudioSpawn (groundLaserBassBoost, 0, 1.4f, 1, 1);

        anim.Play ("HeinzDoubleGroundLaser", 0, 0);
        centerPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        for (int i = 0; i < 20; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity).GetComponent<WitchWestLaser> ().clip = null;
        }
        yield return new WaitForSeconds (1);
        SpawnAudio.AudioSpawn (groundLaserBassBoost, 0, 1.6f, 1, 1);

        anim.Play ("HeinzDoubleGroundLaser", 0, 0);
        centerPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        for (int i = 0; i < 30; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity).GetComponent<WitchWestLaser> ().clip = null;
        }
        yield return new WaitForSeconds (1);
        SpawnAudio.AudioSpawn (groundLaserBassBoost, 0, 1.8f, 1, 1);

        anim.Play ("HeinzDoubleGroundLaser", 0, 0);
        centerPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        for (int i = 0; i < 35; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity).GetComponent<WitchWestLaser> ().clip = null;
        }
        yield return new WaitForSeconds (1);
        SpawnAudio.AudioSpawn (groundLaserBassBoost, 0, 2f, 1, 1);

        anim.Play ("HeinzDoubleGroundLaser", 0, 0);
        centerPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        for (int i = 0; i < 40; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity).GetComponent<WitchWestLaser> ().clip = null;
        }
        yield return new WaitForSeconds (1);
        anim.Play ("HeinzDoubleGroundLaser", 0, 0);
        StopAttack ();
    }

    IEnumerator LaserCircleAttack () {
        TalkIfNotTalking (laserCircleVoice);
        anim.Play ("HeinzLaserCircle");
        animRotator.enabled = true;
        GameObject g = Instantiate (laserCirclePrefab, transform.position, Quaternion.identity);
        for (int i = 0; i < g.transform.childCount; i++) {
            g.transform.GetChild (i).GetComponent<WitchWestLaser> ().activeTime /= 2;
        }
        yield return new WaitForSeconds (7.5f);
        StopAttack ();
    }

    IEnumerator LaserCircleP2Attack () {
        anim.Play ("HeinzLaserCircle");
        animRotator.enabled = true;
        TalkIfNotTalking (laserCircleVoice);
        Instantiate (laserP2CirclePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds (12.5f);
        StopAttack ();
    }

    IEnumerator LaserCircleP3Attack () {
        anim.Play ("HeinzLaserCircle");
        animRotator.enabled = true;
        TalkIfNotTalking (laserCircleVoice);
        Invoke ("NoLaser", 10.5f * 4);
        Instantiate (laserCirclePrefab, transform.position, Quaternion.identity).GetComponent<AutoRotate> ().v3 /= 10;
        Instantiate (laserCirclePrefab, transform.position, Quaternion.identity).GetComponent<AutoRotate> ().v3 /= -4;

        yield return new WaitForSeconds (0.5f);
        for (int i = 0; i < 20; i++) {
            Instantiate (groundLaser3Prefab, new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z), Quaternion.identity);
            yield return new WaitForSeconds (0.5f);
        }
        yield return new WaitForSeconds (2f);
        StopAttack ();
    }

    IEnumerator AimShootP1 () {
        SmoothLookAtPlayer ();
        anim.Play ("HeinzPredictionShotStart");
        TalkIfNotTalking (quickAttackP1Voice);
        yield return new WaitForSeconds (0.5f);
        SpawnShooterPrefab (player.transform.position + Vector3.up + player.transform.forward * 5, new Vector3 (0, 4, -3), true);
        yield return new WaitForSeconds (0.2f);
        anim.Play ("HeinzPredictionShot");
        yield return new WaitForSeconds (0.5f);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");
    }

    IEnumerator AimShootP2 () {
        anim.Play ("HeinzDoubleShotAStart");
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (0.5f);
        SpawnShooterPrefab (player.transform.position + Vector3.up, new Vector3 (-3, 3, -3), true);
        SpawnShooterPrefab (player.transform.position + Vector3.up, new Vector3 (3, 3, -3), false);
        yield return new WaitForSeconds (0.1f);
        anim.Play ("HeinzDoubleShotA");
        yield return new WaitForSeconds (0.4f);
        SpawnShooterPrefab (player.transform.position + Vector3.up + player.transform.forward * 5, new Vector3 (-4.3f, 3, -3), true);
        SpawnShooterPrefab (player.transform.position + Vector3.up + player.transform.forward * -5, new Vector3 (4.3f, 3, -3), false);
        yield return new WaitForSeconds (0.1f);
        anim.Play ("HeinzDoubleShotB");
        yield return new WaitForSeconds (0.3f);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");
    }

    IEnumerator AimShootP3 () {
        anim.Play ("HeinzRapidFireStart");
        float rngRange = 2;
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (0.5f);
        anim.Play ("HeinzRapidFireLoop");
        TalkIfNotTalking (RatataP3Voice);
        SpawnShooterPrefab (player.transform.position + Vector3.up + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), new Vector3 (0, 4, -3), true);
        yield return new WaitForSeconds (0.1f);
        SpawnAudio.AudioSpawn (ratataLoopSound, 6.3f, 1, 0.4f);
        for (int i = 0; i < 20; i++) {
            SpawnShooterPrefab (player.transform.position + Vector3.up + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), new Vector3 (0, 4, -3), false);
            yield return new WaitForSeconds (0.1f);
        }
        anim.Play ("HeinzPredictionShotStart");
        yield return new WaitForSeconds (1f);
        SpawnShooterPrefab (player.transform.position + Vector3.up + player.transform.forward * 5, new Vector3 (0, 4, -3), true);
        yield return new WaitForSeconds (0.1f);
        anim.Play ("HeinzPredictionShot");
        yield return new WaitForSeconds (0.3f);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");
    }

    IEnumerator SurroundPlayer () {
        SmoothLookAtPlayer ();
        if (curState != State.FinalAttack) {
            anim.Play ("HeinzNormalAttackStart");
        }
        yield return new WaitForSeconds (0.3f);
        TalkIfNotTalking (surroundPlayerP1Voice);
        GameObject g = Instantiate (shooterPrefab2, player.transform.position + player.transform.forward * -10 + Vector3.up * 2, transform.rotation);
        g.transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        g.transform.Rotate (90, 0, 0);
        yield return new WaitForSeconds (0.6f);
        if (curState != State.FinalAttack) {
            anim.Play ("HeinzNormalAttack");
        }
        yield return new WaitForSeconds (0.3f);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");

    }

    IEnumerator SurroundPlayer2 () {
        anim.Play ("HeinzNormalAttackStart");
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (1);
        GameObject g = Instantiate (shooterPrefab2, player.transform.position + player.transform.forward * -10 + Vector3.up * 2, transform.rotation);
        g.transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        g.transform.Rotate (90, 0, 0);
        yield return new WaitForSeconds (0.05f);

        g = Instantiate (shooterPrefab2, player.transform.position + player.transform.forward * 10 + Vector3.up * 2, transform.rotation);
        g.transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        g.transform.Rotate (90, 0, 0);
        yield return new WaitForSeconds (0.05f);

        g = Instantiate (shooterPrefab2, player.transform.position + player.transform.right * 10 + Vector3.up * 2, transform.rotation);
        g.transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        g.transform.Rotate (90, 0, 0);
        yield return new WaitForSeconds (0.05f);

        g = Instantiate (shooterPrefab2, player.transform.position + player.transform.right * -10 + Vector3.up * 2, transform.rotation);
        g.transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        g.transform.Rotate (90, 0, 0);

        yield return new WaitForSeconds (0.45f);
        anim.Play ("HeinzNormalAttack");
        yield return new WaitForSeconds (1);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");

    }

    IEnumerator SurroundPlayer3 () {
        SmoothLookAtPlayer ();
        anim.Play ("HeinzDoubleShotBStart");
        yield return new WaitForSeconds (1);
        TalkIfNotTalking (Phase3Attacks);
        float curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, player.transform.position + Vector3.up * 2, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 10;
            curAngle += 360 / 10;
            g.transform.LookAt (player.transform.position + Vector3.up);
            g.transform.Rotate (90, 0, 0);
        }
        yield return new WaitForSeconds (0.6f);
        anim.Play ("HeinzDoubleShotB");
        yield return new WaitForSeconds (0.15f);
        curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, player.transform.position + Vector3.up * 2, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 10;
            curAngle += 360 / 10;
            g.transform.LookAt (player.transform.position + Vector3.up);
            g.transform.Rotate (90, 0, 0);
        }
        yield return new WaitForSeconds (0.6f);
        anim.Play ("HeinzDoubleShotB");
        yield return new WaitForSeconds (0.15f);
        curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, player.transform.position + Vector3.up * 2, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 10;
            curAngle += 360 / 10;
            g.transform.LookAt (player.transform.position + Vector3.up);
            g.transform.Rotate (90, 0, 0);
        }
        yield return new WaitForSeconds (0.6f);
        anim.Play ("HeinzDoubleShotB");
        yield return new WaitForSeconds (0.4f);
        CancelInvoke ("SmoothLookAtPlayer");
        StopAttack ();
    }

    IEnumerator AroundWitch () {
        yield return new WaitForSeconds (1);
        anim.Play ("HeinzBulletCircleLoop");
        animRotator.enabled = true;
        animRotator.v3 *= 2;
        float curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, transform.position, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 5;
            g.transform.Rotate (0, 0, 90);
            curAngle += 360 / 10;
        }
        yield return new WaitForSeconds (1);
        animRotator.v3 /= 2;
        StopAttack ();
    }

    IEnumerator AroundWitch2 () {
        yield return new WaitForSeconds (0.5f);
        anim.Play ("HeinzBulletCircleLoop");
        animRotator.enabled = true;
        animRotator.v3 *= 3;
        TalkIfNotTalking (aroundWitchP2);
        float curAngle = 0;
        SpawnAudio.AudioSpawn (ratataLoopSound, 6.5f, 1, 1, 0.65f);
        for (int i = 0; i < 100; i++) {
            GameObject g = Instantiate (shooterPrefab2, transform.position, Quaternion.Euler (90, curAngle, 0));
            if (i > 1) {
                g.GetComponent<WitchWestLaser> ().clip = null;
            }
            g.transform.position -= g.transform.right * 5;
            g.transform.Rotate (0, 0, -90);

            curAngle += 360 / 20 + (i / 3f);
            yield return new WaitForEndOfFrame ();
        }
        yield return new WaitForSeconds (1);
        animRotator.v3 /= 3;
        StopAttack ();
    }
    IEnumerator AroundWitch3 () {
        yield return new WaitForSeconds (0.4f);
        anim.Play ("HeinzBulletCircleLoop");
        animRotator.enabled = true;
        animRotator.v3 *= 3;
        yield return new WaitForSeconds (0.4f);
        SpawnAudio.AudioSpawn (ratataLoopSound, 2.5f, 1, 1, 0.65f);
        TalkIfNotTalking (rapidFireP3Voice);
        float curAngle = 0;
        float curForwardAmount = 2;
        for (int i = 0; i < 100; i++) {
            GameObject g = Instantiate (shooterPrefab2, transform.position, Quaternion.Euler (90, curAngle, 0));
            if (i > 1) {
                g.GetComponent<WitchWestLaser> ().clip = null;
            }
            g.transform.position -= g.transform.right * curForwardAmount;
            g.transform.Rotate (0, 0, -90);
            curAngle += 360 / 24 + (i / 3f);
            curForwardAmount += 0.25f;
            yield return new WaitForSeconds (0.05f);
        }
        yield return new WaitForSeconds (1);
        animRotator.v3 /= 3;
        StopAttack ();
    }

    void SpawnShooterPrefab (Vector3 lookAtPos, Vector3 offset, bool hasSound) {
        GameObject g = Instantiate (shooterPrefab, transform.position + (-transform.forward * offset.z) + (transform.up * offset.y) + (transform.right * offset.x), transform.rotation);
        if (hasSound == false) {
            g.GetComponent<WitchWestLaser> ().clip = null;
        }
        g.transform.LookAt (lookAtPos);
        g.transform.Rotate (90, 0, 0);

    }
    void SmoothLookAtPlayer () {
        Vector3 goalPos = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (-(transform.position - goalPos), Vector3.up), Time.deltaTime * 5);
        Invoke ("SmoothLookAtPlayer", 0);
    }

    void StopAttack () {
        isAttacking = false;
        if (curState != State.FinalAttack) {
            curState = State.Idle;
            anim.Play ("HeinzIdle");
            animRotator.enabled = false;
            heinzModel.localEulerAngles = Vector3.zero;
        }
    }
}