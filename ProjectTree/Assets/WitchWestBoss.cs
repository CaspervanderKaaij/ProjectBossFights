using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchWestBoss : MonoBehaviour {

    bool isAttacking = false;
    [SerializeField] Hitbox hp;
    public enum State {
        Idle,
        Attacking
    }
    public State curState = State.Idle;
    PlayerController player;
    public float floorYCoods = 0;
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
    [SerializeField] AudioClip barrierClip;
    [SerializeField] AudioClip barrierClip2;
    [SerializeField] SpriteRenderer testFace;
    [SerializeField] Sprite[] testFaces;
    PlayerCam cam;
    [SerializeField] Renderer[] shieldMats;

    void Start () {
        player = FindObjectOfType<PlayerController> ();
        hitbox = GetComponent<Hitbox> ();
        cam = player.cameraTransform.GetComponent<PlayerCam> ();
        barrierHBox = barrier.GetComponent<Hurtbox> ();
        barrierHBox.enabled = false;
        SetBarrierActive (true);
    }

    void Update () {
        DebugInput ();
        SetCam ();
        SetBarrierVisibility ();
    }

    void DebugInput () {
        if (isAttacking == true) {
            hitbox.enabled = false;
        } else if (barrier.activeSelf == false) {
            hitbox.enabled = true;
        }

        if (hitbox.hp > 800) {
            StartPhase1Attack ();
        } else if (hitbox.hp > 350) {
            StartPhase2Attack ();
        } else {
            StartPhase3Attack ();
        }

    }

    void SetCam () {
        cam.angleGoal.x = 25;
        cam.angleGoal.y = Quaternion.LookRotation (transform.position - cam.transform.position, Vector3.up).eulerAngles.y;
        cam.offset = cam.transform.forward * -20;
    }

    Hurtbox barrierHBox;
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
        }
        if (active == false && wasActive == true) {
            Instantiate (barrierBreakParticle, barrier.transform.position, Quaternion.identity);
        }
    }

    float curDissolve = 0;
    void SetBarrierVisibility () {
        if (barrierHBox.enabled == true) {
            curDissolve = Mathf.MoveTowards(curDissolve,1,Time.deltaTime * 1.5f);
            for (int i = 0; i < shieldMats.Length; i++) {
                shieldMats[i].material.SetFloat ("_DissolveMultiplier", curDissolve);
            }
        } else {
            curDissolve = Mathf.MoveTowards(curDissolve,0,Time.deltaTime * 1.5f);
            for (int i = 0; i < shieldMats.Length; i++) {
                shieldMats[i].material.SetFloat ("_DissolveMultiplier", curDissolve);
            }
        }
    }

    void BarrierShake () {
        FindObjectOfType<TimescaleManager> ().SlowMo (0.2f, 0.2f);
        FindObjectOfType<PlayerCam> ().HardShake (0.15f);
        Instantiate (barrierParticle, barrier.transform.position, Quaternion.identity);
        barrier.GetComponent<AutoScale> ().enabled = false;
        barrier.transform.localScale *= 2;
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
                curAtk++;
                curState = atk;
                StartCoroutine (coroutineName);
                isAttacking = true;
            } else {
                curAtk = 0;
                Invoke ("NoAttack", 5);
            }
        }
    }

    void NoAttack () {
        SetBarrierActive (true);
        isAttacking = true;
        Invoke ("StopAttack", 1);
    }

    IEnumerator GroundLaserAttack () {
        //start animation
        Vector3 spawnPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        float repeatSpeed = Random.Range (0.6f, 0.9f);
        yield return new WaitForSeconds (1);
        for (int i = 0; i < 5; i++) {
            spawnPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
            Instantiate (groundLaserPrefab, spawnPos, Quaternion.identity).GetComponent<WitchWestLaser> ().chargeTime *= 1.5f;
            yield return new WaitForSeconds (repeatSpeed);
        }
        StopAttack ();
    }

    IEnumerator GroundLaserPhase2Attack () {
        float repeatSpeed = Random.Range (0.2f, 0.3f);
        SmoothLookAtPlayer ();
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds (repeatSpeed);
            Instantiate (aimGroundLaserPrefab, transform.position + transform.forward * 3, transform.rotation * Quaternion.Euler (90, 0, 0));
        }
        yield return new WaitForSeconds (1);
        CancelInvoke ("SmoothLookAtPlayer");
        StopAttack ();
    }

    IEnumerator GroundLaserPhase3Attack () {
        yield return new WaitForSeconds (1);
        Vector3 centerPos = new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z);
        float rngRange = 20;
        for (int i = 0; i < 5; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity);
        }
        yield return new WaitForSeconds (1);
        for (int i = 0; i < 10; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity);
        }
        yield return new WaitForSeconds (1);
        for (int i = 0; i < 20; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity);
        }
        yield return new WaitForSeconds (1);
        for (int i = 0; i < 30; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity);
        }
        yield return new WaitForSeconds (1);
        for (int i = 0; i < 35; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity);
        }
        yield return new WaitForSeconds (1);
        for (int i = 0; i < 40; i++) {
            Instantiate (groundLaser3Prefab, centerPos + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), Quaternion.identity);
        }
        yield return new WaitForSeconds (1);
        StopAttack ();
    }

    IEnumerator LaserCircleAttack () {
        GameObject g = Instantiate (laserCirclePrefab, transform.position, Quaternion.identity);
        for (int i = 0; i < g.transform.childCount; i++) {
            g.transform.GetChild (i).GetComponent<WitchWestLaser> ().activeTime /= 2;
        }
        yield return new WaitForSeconds (7.5f);
        StopAttack ();
    }

    IEnumerator LaserCircleP2Attack () {
        Instantiate (laserP2CirclePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds (15);
        StopAttack ();
    }

    IEnumerator LaserCircleP3Attack () {
        Invoke ("NoLaser", 10.5f * 4);
        Instantiate (laserCirclePrefab, transform.position, Quaternion.identity).GetComponent<AutoRotate> ().v3 /= 10;
        Instantiate (laserCirclePrefab, transform.position, Quaternion.identity).GetComponent<AutoRotate> ().v3 /= -4;

        yield return new WaitForSeconds (0.5f);
        for (int i = 0; i < 20; i++) {
            Instantiate (groundLaser3Prefab, new Vector3 (player.transform.position.x, floorYCoods + 0.1f, player.transform.position.z), Quaternion.identity);
            yield return new WaitForSeconds (0.5f);
        }
        StopAttack ();
    }

    IEnumerator AimShootP1 () {
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (0.5f);
        SpawnShooterPrefab (player.transform.position + Vector3.up, new Vector3 (0, 4, -3));
        yield return new WaitForSeconds (0.3f);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");
    }

    IEnumerator AimShootP2 () {
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (0.5f);
        SpawnShooterPrefab (player.transform.position + Vector3.up, new Vector3 (-3, 3, -3));
        SpawnShooterPrefab (player.transform.position + Vector3.up, new Vector3 (3, 3, -3));
        yield return new WaitForSeconds (0.5f);
        SpawnShooterPrefab (player.transform.position + Vector3.up + player.transform.forward * 5, new Vector3 (-4.3f, 3, -3));
        SpawnShooterPrefab (player.transform.position + Vector3.up + player.transform.forward * -5, new Vector3 (4.3f, 3, -3));
        yield return new WaitForSeconds (0.3f);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");
    }

    IEnumerator AimShootP3 () {
        float rngRange = 2;
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (0.5f);
        SpawnShooterPrefab (player.transform.position + Vector3.up + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), new Vector3 (0, 4, -3));
        for (int i = 0; i < 20; i++) {
            yield return new WaitForSeconds (0.1f);
            SpawnShooterPrefab (player.transform.position + Vector3.up + new Vector3 (Random.Range (-rngRange, rngRange), 0, Random.Range (-rngRange, rngRange)), new Vector3 (0, 4, -3));
        }
        yield return new WaitForSeconds (1f);
        SpawnShooterPrefab (player.transform.position + Vector3.up, new Vector3 (0, 4, -3));
        yield return new WaitForSeconds (0.1f);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");
    }

    IEnumerator SurroundPlayer () {
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (1);
        GameObject g = Instantiate (shooterPrefab2, player.transform.position + player.transform.forward * -10 + Vector3.up * 2, transform.rotation);
        g.transform.LookAt (new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z));
        g.transform.Rotate (90, 0, 0);
        yield return new WaitForSeconds (1);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");

    }

    IEnumerator SurroundPlayer2 () {
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

        yield return new WaitForSeconds (1);
        StopAttack ();
        CancelInvoke ("SmoothLookAtPlayer");

    }

    IEnumerator SurroundPlayer3 () {
        SmoothLookAtPlayer ();
        yield return new WaitForSeconds (1);
        float curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, player.transform.position + Vector3.up * 2, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 10;
            curAngle += 360 / 10;
            g.transform.LookAt (player.transform.position + Vector3.up);
            g.transform.Rotate (90, 0, 0);
        }
        yield return new WaitForSeconds (0.75f);
        curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, player.transform.position + Vector3.up * 2, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 10;
            curAngle += 360 / 10;
            g.transform.LookAt (player.transform.position + Vector3.up);
            g.transform.Rotate (90, 0, 0);
        }
        yield return new WaitForSeconds (0.75f);
        curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, player.transform.position + Vector3.up * 2, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 10;
            curAngle += 360 / 10;
            g.transform.LookAt (player.transform.position + Vector3.up);
            g.transform.Rotate (90, 0, 0);
        }
        yield return new WaitForSeconds (1);
        CancelInvoke ("SmoothLookAtPlayer");
        StopAttack ();
    }

    IEnumerator AroundWitch () {
        yield return new WaitForSeconds (1);
        float curAngle = 0;
        for (int i = 0; i < 10; i++) {
            GameObject g = Instantiate (shooterPrefab2, transform.position, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 5;
            g.transform.Rotate (0, 0, 90);
            curAngle += 360 / 10;
        }
        yield return new WaitForSeconds (1);
        StopAttack ();
    }

    IEnumerator AroundWitch2 () {
        yield return new WaitForSeconds (1);
        float curAngle = 0;
        for (int i = 0; i < 100; i++) {
            GameObject g = Instantiate (shooterPrefab2, transform.position, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * 5;
            g.transform.Rotate (0, 0, -90);
            curAngle += 360 / 20;
            yield return new WaitForEndOfFrame ();
        }
        yield return new WaitForSeconds (1);
        StopAttack ();
    }
    IEnumerator AroundWitch3 () {
        yield return new WaitForSeconds (1);
        float curAngle = 0;
        float curForwardAmount = 5;
        for (int i = 0; i < 100; i++) {
            GameObject g = Instantiate (shooterPrefab2, transform.position, Quaternion.Euler (90, curAngle, 0));
            g.transform.position -= g.transform.right * curForwardAmount;
            g.transform.Rotate (0, 0, -90);
            curAngle += 360 / 20;
            curForwardAmount += 0.25f;
            yield return new WaitForSeconds (0.05f);
        }
        yield return new WaitForSeconds (1);
        StopAttack ();
    }

    void SpawnShooterPrefab (Vector3 lookAtPos, Vector3 offset) {
        GameObject g = Instantiate (shooterPrefab, transform.position + (-transform.forward * offset.z) + (transform.up * offset.y) + (transform.right * offset.x), transform.rotation);
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
        curState = State.Idle;
    }
}