﻿using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
[RequireComponent (typeof (CharacterController))]

public class PlayerController : MonoBehaviour {

    CharacterController cc;
    public Vector3 movev3;
    public bool ThreeDMode = true;
    public Transform cameraTransform;
    TimescaleManager timescaleManager;
    PlayerCam playerCam;
    public Animator anim;
    public enum State {
        Normal,
        Dash,
        Dialogue,
        Knockback,
        Attack,
        SlamAttack,
        Gun,
        Death,
        Parry,
        WallJump,
        WallSlide
    }
    public State curState = State.Normal;
    [Header ("Input")]
    public string horInput = "Horizontal";
    public string vertInput = "Vertical";
    public string jumpInput = "Jump";
    public string dashInput = "Dash";
    public string shootInput = "Shoot";
    public string parryInput = "Parry";
    public string switchWeaponInput = "Mouse ScrollWheel";
    public string interactInput = "Interact";
    public string reverseInput = "Reverse";
    [Header ("Stats")]
    public MoveStats[] moveStats;
    public int curMoveStats = 0;
    public bool isGrounded = true;
    bool wasGrounded = true;
    public int maxJumps = 5;
    public float jumpStrength = 20;
    public float gravityStrength = -29.81f;
    [SerializeField] AudioClip landAudio;
    [HideInInspector] public float willpower = 100;
    public float willpowerRefillSpeed = 10;
    [SerializeField] FootstepSound footstep;
    [SerializeField] AudioClip parryAudio;
    [Header ("Dash")]
    [SerializeField] GameObject[] dashVisible;
    [SerializeField] GameObject[] dashInvisible;
    [SerializeField] float dashTime = 0.4f;
    [SerializeField] float dashSpeed = 4;
    bool canAirDash = true;
    [SerializeField] float dashWPCost = 10;
    [SerializeField] AudioClip[] dashAudio;
    public enum Weapon {
        Gun,
        Spear
    }

    [Header ("Weapon")] //here
    public Weapon curWeapon = Weapon.Gun;
    [SerializeField] AttackStats[] attacks;
    public bool canBuffer = false;
    int bufferedAttack = 0;
    [SerializeField] GameObject spear;
    [SerializeField] GameObject snekSpear;
    [SerializeField] GameObject backspear;
    [SerializeField] GameObject backSnekSpear;
    [SerializeField] AudioClip slamAttackAudio;
    [SerializeField] TrailRenderer[] attackTrails;
    [Header ("Hitboxes")]
    [SerializeField] GameObject hitboxParent;
    List<GameObject> hitboxes = new List<GameObject> ();
    [SerializeField] AudioClip hitEffectAudio;
    [Header ("Shooting")]
    public GunWeapon gunWeapon;
    [SerializeField] GameObject shootMagicCircle;
    [SerializeField] GameObject backMagicCircle;
    [SerializeField] float shootWPCost = 5;
    [SerializeField] LineRenderer shooterLineRend;
    [Header ("Particles")]
    [SerializeField] GameObject stopDashParticle;
    [SerializeField] GameObject getHitParticle;
    [SerializeField] AudioClip getHitAudio;
    [SerializeField] ParticleSystem walkParticles;
    [SerializeField] GameObject hitEffectParticle;
    [SerializeField] GameObject WdRipple;
    [SerializeField] RandomWind wind;
    [Header ("Voice Lines")]
    public AudioClip[] voiceLines;

    [Header ("UI")]
    [Header ("HUD")]
    public Canvas hudCanvas;
    public Text curModeText;
    public UIBar willPowerBar;
    [Header ("Dialogue")]
    [SerializeField] GameObject textIcon;
    [HideInInspector] public List<Interact> interactables;
    [SerializeField] Dialogue diaUI;
    [Header ("Health")]
    [SerializeField] PostProcessVolume getHitPP;
    [SerializeField] PostProcessVolume parryPP;
    [SerializeField] GameObject lowHPPostProccesing;
    [SerializeField] UIBar hpBar;
    [HideInInspector] public Hitbox hitbox;
    public float maxHP = 100;
    [Header ("Buffs")]
    public float speedMuliplier = 1;
    [Header ("Reverse")]
    [SerializeField] SetMaterialGroup normalMats;
    [SerializeField] SetMaterialGroup reverseMats;
    [Header ("AbilitiesUnlocked")]
    [SerializeField] bool hasGun = true;
    [SerializeField] bool hasWallJump = true;
    [SerializeField] bool hasDoubleJump = true;
    [SerializeField] bool hasDash = true;
    [SerializeField] bool hasSpear = true;
    [SerializeField] bool hasParry = true;
    [SerializeField] bool hasQuickGun = false;
    [SerializeField] bool hasReverse = false;
    [SerializeField] bool hasDashEX = false;
    public enum ShadowProgress {
        NoShadow,
        Shadow,
        Reverse
    }

    [Header ("Other")]
    public ShadowProgress shadow = ShadowProgress.NoShadow;
    public Renderer heinzHead;

    void Start () {

        //if (FindObjectOfType<StartSaveInitializer> () != null) {
        //   FindObjectOfType<StartSaveInitializer> ().OnEnable ();
        //  }
        cc = GetComponent<CharacterController> ();
        angleGoal = transform.eulerAngles.y;
        playerCam = cameraTransform.GetComponent<PlayerCam> ();
        hitbox = GetComponent<Hitbox> ();
        // maxHP = hitbox.hp;
        timescaleManager = FindObjectOfType<TimescaleManager> ();
        for (int i = 0; i < hitboxParent.transform.childCount; i++) {
            hitboxes.Add (hitboxParent.transform.GetChild (i).gameObject);
        }
        cc.enabled = false;
        Invoke ("SetStartPos", 0);

        transform.SetParent (null);

        if (shadow != ShadowProgress.NoShadow) {
            for (int i = 0; i < dashInvisible.Length; i++) {
                if (dashInvisible[i].GetComponent<Renderer> () != null) {
                    dashInvisible[i].GetComponent<Renderer> ().shadowCastingMode = ShadowCastingMode.TwoSided;
                }
            }
        }

        normalMats.SetToGroup ();

        if (hasDashEX == false) {
            dashVisible[0].GetComponentInChildren<Hurtbox> ().gameObject.SetActive (false);
        }

        SaveStuff data = SaveSystem.LoadStuff ();
        heinzHead.enabled = data.heinzHeadMode;
        if (data.hedgehogMode == true) {
            speedMuliplier = 2;
        }

    }

    void SetStartPos () {
        if (GameObject.Find ("PlayerSpawnPoint") != null) {
            transform.position = GameObject.Find ("PlayerSpawnPoint").transform.position;
        }
        cc.enabled = true;
    }

    void Update () {
        if (timescaleManager.isPaused == false) {
            playerCam.UpdateMe ();
            transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
            anim.transform.localEulerAngles = new Vector3 (anim.transform.localEulerAngles.x, anim.transform.localEulerAngles.y, 0);
            shootMagicCircle.SetActive (false);
            backMagicCircle.SetActive (true);
            switch (curState) {
                case State.Normal:
                    if (dashInvisible[0].activeSelf == false) {
                        SetDashInvisible (false);
                        playerCam._enabled = true;
                    }
                    WallJump ();
                    Jump (jumpStrength);
                    isGrounded = IsGrounded ();
                    SetAngle ();
                    MoveForward ();
                    Gravity ();
                    Reverse ();
                    FinalMove ();
                    IdleFidget ();
                    SetWillpowerBar ();
                    SetHPBar ();
                    DashInput ();
                    SetCurWeapon ();
                    GetParryInput ();
                    if (interactables.Count == 0 && curState == State.Normal) {
                        switch (curWeapon) {
                            case Weapon.Gun:
                                ShootInput ();
                                break;
                            case Weapon.Spear:
                                SpearInput ();
                                break;
                        }
                    }
                    InteractCheck ();
                    DebugMoves ();
                    break;
                case State.Dash:
                    isGrounded = false;
                    Dash ();
                    FinalMove ();
                    break;
                case State.Knockback:

                    Gravity ();
                    FinalMove ();

                    break;

                case State.Attack:
                    if (IsInvoking ("IsStartingUp") == false) {
                        FinalMove ();
                    }
                    if (Input.GetButtonDown (jumpInput) == true && curState != State.WallJump) {
                        Invoke ("JumpBuffer", 0.2f);
                    }
                    if (canBuffer == true) {
                        SpearInput ();
                    }
                    break;
                case State.Dialogue:
                    curAccDec = 0;
                    isGrounded = true;
                    wasGrounded = true;
                    break;
                case State.SlamAttack:
                    isGrounded = IsGrounded ();
                    Gravity ();
                    movev3.x = Mathf.MoveTowards (movev3.x, 0, Time.deltaTime * moveStats[0].deceleration);
                    movev3.z = Mathf.MoveTowards (movev3.z, 0, Time.deltaTime * moveStats[0].deceleration);
                    FinalMove ();
                    if (isGrounded == true) {
                        GetAttackInput (3);
                        playerCam.MediumShake (0.3f);
                        // playerCam.ripple.Emit ();
                        Instantiate (WdRipple, hitboxes[1].transform.position, Quaternion.Euler (90, 0, 0));
                        Instantiate (stopDashParticle, transform.position, Quaternion.identity);
                        SpawnAudio.AudioSpawn (slamAttackAudio, 0.5f, Random.Range (4, 5), 0.5f);
                        //timescaleManager.SlowMo (0.15f, 0.05f, 0.1f);
                        playerCam.transform.Rotate (10, 0, 0);

                    } else {
                        DashInput ();
                    }
                    break;
                case State.Gun:
                    ShootInput ();
                    SetWillpowerBar ();
                    Jump (jumpStrength);
                    break;
                case State.WallJump:
                    Gravity ();
                    FinalMove ();
                    break;
                case State.WallSlide:
                    isGrounded = IsGrounded ();
                    WallSlide ();
                    FinalMove ();
                    break;
            }
            lowHPPostProccesing.SetActive ((hitbox.hp / maxHP < 0.3f));
            playerCam.UpdateMe ();
        }
    }

    // Inputs

    float GetHorInput () {
        return Input.GetAxis (horInput);
    }

    float GetVertInput () {
        if (ThreeDMode == true) {
            return Input.GetAxis (vertInput);
        } else {
            return 0;
        }
    }

    // Movement

    void GetParryInput () {
        if (Input.GetButtonDown (parryInput) == true && isGrounded == true && hasParry == true) {
            curState = State.Parry;
            curWeapon = Weapon.Spear;
            movev3 = Vector3.zero;
            anim.Play ("Parry");
            Invoke ("StopAttack", 0.3f);
            isGrounded = true;
            wasGrounded = true;
            curAccDec = 0;

            playerCam.ripple.Emit ();

            SetSpearActive (false);

        }
    }

    public void ParrySuccess () {
        playerCam.HardShake (0.1f);
        // playerCam.ripple.Emit ();
        //playerCam.Flash (new Color(0,0,0,1), 5);

        timescaleManager.SlowMo (0.2f, 0.1f);
        Instantiate (hitEffectParticle, hitboxes[0].transform.position, Quaternion.identity);
        SpawnAudio.SpawnVoice (voiceLines[0], 0, 1, 1f, 0);
        parryPP.weight = 1;
        Invoke ("SetParryPPWeight", 0);
        SpawnAudio.AudioSpawn (slamAttackAudio, 0.5f, Random.Range (4, 5), 0.4f);

        anim.Play ("ParryHit");

        playerCam.transform.Rotate (-10, 0, 5);
    }

    float AnalAngle () {
        return Mathf.Atan2 (GetVertInput (), -GetHorInput ()) * Mathf.Rad2Deg;
    }

    float AnalMagnitude () {
        return Mathf.Min (1, Mathf.RoundToInt (new Vector2 (GetHorInput (), GetVertInput ()).sqrMagnitude));
    }

    [HideInInspector] public float angleGoal = 0;
    float zDifference = 0;
    void SetAngle () {
        if (AnalMagnitude () > 0) {
            angleGoal = AnalAngle () + cameraTransform.eulerAngles.y - 90;
        }
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z), Time.deltaTime * moveStats[curMoveStats].rotSpeed);
        if (curAccDec > 0.2f && isGrounded == true) {
            zDifference = Mathf.Lerp (zDifference, -Mathf.DeltaAngle (transform.eulerAngles.y, angleGoal) / 3, Time.deltaTime * 10);
            if (Mathf.Abs (Mathf.DeltaAngle (transform.eulerAngles.y, angleGoal)) > 130) {
                curAccDec = 0;
            }
        } else {
            zDifference = Mathf.Lerp (zDifference, 0, Time.deltaTime * 30);
        }
        anim.transform.localEulerAngles = new Vector3 (anim.transform.localEulerAngles.x, anim.transform.localEulerAngles.y, zDifference);
    }

    void Dash () {
        Vector3 helper = transform.TransformDirection (0, 0, dashSpeed * speedMuliplier);
        movev3.x = helper.x;
        movev3.z = helper.z;
        movev3.y = 0;

        if (hasDashEX == false && Physics.Raycast (transform.position + transform.up, transform.forward, 0.75f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) == true) {
            curState = State.Knockback;
            SetDashInvisible (false);
            movev3.x = -transform.forward.x * 10;
            movev3.z = -transform.forward.z * 10;
            movev3.y = jumpStrength / 2;
            Invoke ("StopKnockback", 0.2f);
            curAccDec = 0;
            playerCam._enabled = true;
            anim.Play ("GetHit");
            SpawnAudio.SpawnVoice (voiceLines[Random.Range (6, 9)], 0, 1, 1, 0f);
            shootMagicCircle.SetActive (false);
            backMagicCircle.SetActive (true);
            Instantiate (stopDashParticle, transform.position + transform.up, Quaternion.identity);
            playerCam.MediumShake (0.15f);

        }

        if (hasDashEX == true) {
            hitbox.enabled = false;
        }
    }

    [HideInInspector] public float curAccDec = 0;
    LandAction lastLand = null;
    void MoveForward () {

        if (isGrounded == true) {

            if (AnalMagnitude () > 0) {
                curAccDec = Mathf.MoveTowards (curAccDec, AnalMagnitude (), Time.deltaTime * moveStats[curMoveStats].acceleration);
            } else {
                curAccDec = Mathf.MoveTowards (curAccDec, AnalMagnitude (), Time.deltaTime * moveStats[curMoveStats].deceleration);
            }

            Vector3 forwardhelper = transform.TransformDirection (0, 0, curAccDec * moveStats[curMoveStats].speed * speedMuliplier);
            movev3.x = forwardhelper.x;
            movev3.z = forwardhelper.z;

            anim.SetFloat ("curSpeed", AnalMagnitude ());
            anim.SetFloat ("speedMuliplier", speedMuliplier);

            if (land != lastLand && land != null) {
                land.Activate ();
            }
            lastLand = land;
        } else if (curState != State.WallJump) {
            float[] inputs = new float[2];
            inputs[0] = Mathf.Clamp (GetHorInput () * 2, -1, 1);
            inputs[1] = Mathf.Clamp (GetVertInput () * 2, -1, 1);
            Vector3 helper = new Vector3 (inputs[0], 0, inputs[1]) * moveStats[curMoveStats].speed * speedMuliplier;

            float oldCamZ = cameraTransform.eulerAngles.z;
            cameraTransform.eulerAngles = new Vector3 (cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, 0);
            helper = cameraTransform.TransformDirection (helper);
            cameraTransform.eulerAngles = new Vector3 (cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, oldCamZ);
            movev3.x = Mathf.Lerp (movev3.x, helper.x, Time.deltaTime * moveStats[curMoveStats].acceleration * 6);
            movev3.z = Mathf.Lerp (movev3.z, helper.z, Time.deltaTime * moveStats[curMoveStats].acceleration * 6);
        }
    }

    float jumpsLeft = 2;
    void Jump (float strength) {
        if (Input.GetButtonDown (jumpInput) == true && curState != State.WallJump) {
            Invoke ("JumpBuffer", 0.2f);
        }
        if (jumpsLeft > 0 && IsInvoking ("JumpBuffer") == true) {
            jumpsLeft--;
            if (jumpsLeft < maxJumps - 1 && (anim.GetCurrentAnimatorStateInfo (0).IsName ("Fall") == true || anim.GetCurrentAnimatorStateInfo (0).IsName ("ShootAir") == true || anim.GetCurrentAnimatorStateInfo (0).IsName ("DoubleJump") || anim.GetCurrentAnimatorStateInfo (0).IsName ("GetHit"))) {
                anim.Play ("DoubleJump");
            }
            movev3.y = strength + (curAccDec * 2);
            CancelInvoke ("JumpBuffer");
            CancelInvoke ("CayoteTime");
            if (Random.Range (0, 100) <= 40) {
                SpawnAudio.SpawnVoice (voiceLines[Random.Range (1, 3)], 0, 1, 1, 0);
            }
            if (curState == State.Gun) {
                curState = State.Normal;
                anim.Play ("Fall");
            }
        }
    }

    void JumpBuffer () {

    }

    void Gravity () {
        if (movev3.y > 0) {
            movev3.y = Mathf.MoveTowards (movev3.y, gravityStrength, Time.deltaTime * 100);

            if (Input.GetAxis (jumpInput) == 0) {
                movev3.y = Mathf.MoveTowards (movev3.y, gravityStrength, Time.deltaTime * 100);
            }

            if (Physics.Raycast (transform.position, Vector3.up, cc.height * 1.1f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) == true) {
                movev3.y = -0.01f;
                playerCam.SmallShake (0.1f);
            }

        } else {
            movev3.y = Mathf.MoveTowards (movev3.y, gravityStrength, Time.deltaTime * 50);

        }
        if (isGrounded == true) {
            movev3.y = -1f;
        }
    }

    float groundedTime = 0;
    LandAction land;
    bool IsGrounded () {

        if (hasDoubleJump == true) {
            maxJumps = 2;
        } else {
            maxJumps = 1;
        }

        RaycastHit hit;
        // Debug.DrawRay (transform.position + new Vector3 (0, 0.1f, 0), Vector3.down * 0.5f, Color.red, 0);
        if (Physics.Raycast (transform.position + new Vector3 (0, 0.1f, 0), Vector3.down, out hit, 0.4f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) || cc.isGrounded == true) {
            //if (Vector3.Angle (hit.normal, transform.up) <= cc.slopeLimit) {
            if (groundHit == null || (Vector3.Angle (groundHit.normal, transform.up) <= cc.slopeLimit && groundHit.transform.tag != "NoFloor")) {
                if (groundedTime > Time.deltaTime / 2 && movev3.y < 0) {
                    groundHit = null;
                    if (movev3.y < 0) {
                        CancelInvoke ("CayoteTime");
                        Invoke ("CayoteTime", 0.1f);
                    }
                    if (hit.transform != null && hit.transform.GetComponent<LandAction> () != null) {
                        land = hit.transform.GetComponent<LandAction> ();
                        land.Activate ();
                        footstep.curSoundType = land.footstepSoundID;

                    } else {
                        footstep.curSoundType = 0;
                        if (land != null) {
                            land.End ();
                            land = null;
                        }
                    }

                    //print (Vector3.Angle (hit.normal, transform.up));
                    cc.Move (new Vector3 (0, -100, 0) * Time.deltaTime);
                } else {
                    groundedTime += Time.deltaTime;
                }
            } else if ((curState == State.Normal || curState == State.SlamAttack) && IsInvoking ("CayoteTime") == false) {
                //move backwards
                Vector3 oldEuler = transform.eulerAngles;
                transform.LookAt (transform.position + new Vector3 (groundHit.normal.x, 0, groundHit.normal.z));
                if (ThreeDMode == true) {
                    cc.Move (transform.forward * Time.deltaTime * Mathf.Abs (movev3.y / 2));
                }
                transform.eulerAngles = oldEuler;
                curAccDec = 0;
                movev3.x = 0;
                movev3.z = 0;
                wasGrounded = false;

                if (AnalMagnitude () > 0) {
                    if (Vector3.Dot ((groundHit.point - transform.position).normalized, transform.forward) > 0) {
                        if (movev3.y < -3 && IsInvoking ("NoWallJump") == false && willpower > 0 && curState == State.Normal && hasWallJump == true) {
                            curState = State.WallSlide;
                            firstWallJump = false;
                            canAirDash = true;
                        }
                    }
                }
            }

        } else {
            if (land != null) {
                land.End ();
                land = null;
            }
            groundedTime = 0;
        }

        anim.SetBool ("grounded", IsInvoking ("CayoteTime"));
        if (IsInvoking ("CayoteTime") == true) {
            if (wasGrounded == false) {
                SpawnAudio.AudioSpawn (landAudio, 0, Random.Range (1, 1.5f), 0.2f);
                curAccDec = Mathf.Min (curAccDec, 0.25f);
                jumpsLeft = maxJumps;
            }
        } else {
            if (jumpsLeft == maxJumps) {
                jumpsLeft = maxJumps - 1;
            }
        }
        wasGrounded = IsInvoking ("CayoteTime");
        var emmision = walkParticles.emission;
        emmision.enabled = IsInvoking ("CayoteTime");

        return IsInvoking ("CayoteTime");
    }
    void WallJump () {
        if (isGrounded == false && AnalMagnitude () > 0) {
            RaycastHit hit;
            float oldY = transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
            if (Physics.SphereCast (transform.position + cc.center, cc.radius, transform.forward, out hit, cc.radius * 4.5f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) == true && isGrounded == false) {
                if (Physics.Raycast (transform.position + cc.center * 2, transform.forward, cc.radius * 4.5f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) == true) {
                    if (groundHit == null || Vector3.Angle (groundHit.normal, transform.up) > cc.slopeLimit && IsInvoking ("NoWallJump") == false && willpower > 0 && hasWallJump == true) {
                        transform.rotation = Quaternion.Euler (transform.eulerAngles.x, oldY, transform.eulerAngles.z);
                        //if ((firstWallJump == true || (Mathf.DeltaAngle (angleGoal, lastWallAngle)) > 91) && movev3.y < -3) {
                        if (movev3.y < -3) {
                            curState = State.WallSlide;
                            //lastWallAngle = angleGoal;
                            firstWallJump = false;
                            canAirDash = true;
                        }
                    }
                }
            }
            transform.rotation = Quaternion.Euler (transform.eulerAngles.x, oldY, transform.eulerAngles.z);
        } else if (isGrounded == true) {
            firstWallJump = true;
        }
        if (curState != State.WallSlide && anim.GetCurrentAnimatorStateInfo (0).IsName ("WallSlide") == true) {
            anim.Play ("Idle");
        }
    }

    void StopWallJump () {
        curState = State.Normal;
        angleGoal = transform.eulerAngles.y;
        isGrounded = false;
        wasGrounded = false;
    }

    float lastWallAngle = 0;
    bool firstWallJump = false;
    void WallSlide () {
        SetWillpowerBar ();
        if (willpower < Time.deltaTime * 20) {
            anim.Play ("Fall");
            curState = State.Normal;
        }
        RaycastHit hit;
        movev3 = Vector3.zero;
        SetSpearActive (true);
        curAccDec = 0;
        if (anim.GetCurrentAnimatorStateInfo (0).IsName ("WallSlide") == false) {
            anim.Play ("WallSlide");
        }
        if (Physics.SphereCast (transform.position + cc.center, cc.radius, transform.forward, out hit, cc.radius * 4.5f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) == true && isGrounded == false) {
            if (Input.GetButtonDown (jumpInput) == true) {
                if (willpower > 10) {
                    transform.Rotate (0, 180, 0);
                    angleGoal = transform.eulerAngles.y;
                    movev3 = transform.forward * 10;
                    movev3.y = jumpStrength * 1.2f;
                } else {
                    movev3.y = jumpStrength * 1.5f;
                    willpower = 0;
                }
                curState = State.WallJump;
                // Vector3 oldRot = transform.eulerAngles;
                // transform.forward = hit.normal;
                //  transform.eulerAngles = new Vector3 (oldRot.x, transform.eulerAngles.y, oldRot.z);
                if (willpower > 0) {
                    willpower -= 10;
                }
                Invoke ("NoWallJump", 0.5f);
                anim.Play ("DoubleJump");
                Invoke ("StopWallJump", 0.1f);
                isGrounded = false;
                wasGrounded = false;
            }
        } else {
            anim.Play ("Fall");
            curState = State.Normal;
            Invoke ("NoWallJump", 0.5f);
            Invoke ("NoWallJump", 0.5f);
        }
        DashInput ();
    }

    void NoWallJump () {

    }

    ControllerColliderHit groundHit;
    void OnControllerColliderHit (ControllerColliderHit ccHit) {
        // groundAngle = Vector3.Angle (ccHit.normal, Vector3.up);
        if (ccHit.transform != null) {
            groundHit = ccHit;
        } else {
            groundHit = null;
        }
    }

    void PreventMoveCrash () {

    }

    void CayoteTime () {

    }

    bool dashInputted = false;
    void DashInput () {
        if (isGrounded == true) {
            canAirDash = true;
        }
        if (Input.GetAxis (dashInput) <= 0) {
            dashInputted = false;
        }
        if (Input.GetAxis (dashInput) > 0 && IsInvoking ("BufferDash") == false && dashInputted == false) {
            Invoke ("BufferDash", 0.2f);
            dashInputted = true;
        }
        if (IsInvoking ("BufferDash") == true && IsInvoking ("IgnoreDashInput") == false && willpower > dashWPCost && hasDash == true) {
            bool willDash = false;
            if (isGrounded == false) {
                if (canAirDash == true) {
                    canAirDash = false;
                    willDash = true;
                }
            } else {
                willDash = true;
            }
            if (willDash == true) {
                CancelInvoke ("BufferDash");

                transform.eulerAngles = new Vector3 (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
                curState = State.Dash;
                SpawnAudio.AudioSpawn (dashAudio[0], 0f, Random.Range (4.5f, 4.5f), 1);
                SpawnAudio.AudioSpawn (dashAudio[1], 0.4f, Random.Range (0.75f, 1.25f), 1);
                willpower -= dashWPCost;
                Invoke ("StopDash", dashTime / speedMuliplier);
                curAccDec = 1;
                anim.SetFloat ("curSpeed", 0);
                playerCam._enabled = false;
                anim.Play ("Fall");

                SetDashInvisible (true);

                playerCam.SmallShake (dashTime);
                Invoke ("WaitShoot", dashTime + 0.2f);

            }
        }
    }

    void BufferDash () {

    }

    void ShootInput () {
        SetSpearActive (true);
        curModeText.text = "Shoot Mode";
        shootMagicCircle.SetActive ((curState == State.Gun));
        backMagicCircle.SetActive (!(curState == State.Gun));
        if (curState == State.Gun) {
            shooterLineRend.SetPosition (0, shootMagicCircle.transform.position);
            if (UseMouse () == false) {
                if (AnalMagnitude () > 0) {
                    angleGoal = AnalAngle () + cameraTransform.eulerAngles.y - 90;
                }
                transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z), Time.deltaTime * moveStats[curMoveStats].rotSpeed * 2 * speedMuliplier);
                shooterLineRend.SetPosition (1, transform.position + (Quaternion.AngleAxis (angleGoal, Vector3.up) * (Vector3.forward * 1000)));
            } else {
                Vector3 mPos = transform.position - GetMousePlanePos ();
                transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (-(mPos), Vector3.up), Time.deltaTime * moveStats[curMoveStats].rotSpeed * 2 * speedMuliplier);
                shooterLineRend.SetPosition (1, new Vector3 (transform.position.x, shootMagicCircle.transform.position.y, transform.position.z) - (new Vector3 (mPos.x, 0, mPos.z) * 1000));
            }
            if (Input.GetButtonDown (shootInput) == true) {
                Invoke ("ShootInputBuffer", 0.3f);
            }
            if ((Input.GetAxis (shootInput) != 0 || IsInvoking ("ShootInputBuffer")) && IsInvoking ("WaitShoot") == false) {
                if (willpower > shootWPCost) {
                    if (AnalMagnitude () > 0) {
                        transform.rotation = Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
                    }
                    CancelInvoke ("ShootBufferInput");
                    anim.Play ("GunShot", 0, 0f);
                    gunWeapon.GetInput ();
                    if (hasQuickGun == false) {
                        willpower -= shootWPCost;
                        Invoke ("WaitShoot", 0.3f);
                        timescaleManager.SlowMo (0.1f, 0.05f);
                    } else {
                        willpower -= shootWPCost / 3;
                        Invoke ("WaitShoot", 0.1f);
                    }
                    Instantiate (hitEffectParticle, gunWeapon.spawnPoint.position, Quaternion.identity);
                    SpawnAudio.AudioSpawn (dashAudio, 0, Random.Range (2.5f, 3), 1);
                    playerCam.SmallShake (0.2f);
                }
            }
            if (IsInvoking ("WaitShoot") == false) {
                if (Input.GetAxis (shootInput) == 0 && IsInvoking ("ShootBufferInput") == false) {
                    curState = State.Normal;
                    anim.Play ("EndShoot", 0, 0);
                }
            }
            DashInput ();
        } else {
            if (Input.GetAxis (shootInput) != 0 && isGrounded == true) {
                curState = State.Gun;
                if (Input.GetKeyDown (KeyCode.Mouse0) == true) {
                    mouseControlled = true;
                } else {
                    mouseControlled = false;
                }
                anim.Play ("StartShoot", 0, 0f);
                Invoke ("WaitShoot", 0.3f);
                curAccDec = 0;
            } else if (Input.GetAxis (shootInput) != 0 && IsInvoking ("WaitShoot") == false && willpower > shootWPCost) {
                anim.Play ("ShootAir");
                movev3.y = jumpStrength / 1.5f;
                Invoke ("WaitShoot", 0.6f);
                timescaleManager.SlowMo (0.2f, 0.05f, 0.1f);
                Instantiate (hitEffectParticle, gunWeapon.spawnPoint.position, Quaternion.identity);
                SpawnAudio.AudioSpawn (dashAudio, 0, Random.Range (2.5f, 3), 1);
                playerCam.SmallShake (0.2f);
                gunWeapon.GetInput (new Vector3 (0, 0, -90));
                willpower -= shootWPCost;
                Invoke ("IgnoreDashInput", 0.2f);
            }
        }

        //here!!
    }

    bool mouseControlled = true;
    bool UseMouse () {
        if (mouseControlled == true) {
            //if (AnalMagnitude () > 0) {
            if (Input.GetButtonDown (horInput) == true || Input.GetButtonDown (vertInput) == true) {
                mouseControlled = false;
            }
        } else {
            if (Vector2.SqrMagnitude (new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"))) != 0 && AnalMagnitude () == 0) {
                mouseControlled = true;
            }
        }
        return mouseControlled;
    }

    Vector3 GetMousePlanePos () {
        Vector3 toReturn = Vector3.zero;

        //set rotation to mouse on plane position
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        Plane hPlane = new Plane (Vector3.up, new Vector3 (0, transform.position.y, 0));
        float distance = 0;
        if (hPlane.Raycast (ray, out distance)) {
            toReturn = ray.GetPoint (distance);
        }

        //set rotation to enemy position
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, Mathf.Infinity, LayerMask.GetMask ("Enemy"), QueryTriggerInteraction.Collide) == true) {
            toReturn = new Vector3 (hit.point.x, transform.position.y, hit.point.z);
        }

        return toReturn;
    }

    void ShootInputBuffer () {

    }

    void SpearInput () {
        curModeText.text = "Spear Mode";
        SetSpearActive (false);
        if (Input.GetButtonDown (shootInput) == true && isGrounded == true) {
            GetAttackInput (0);
        }
        if (Input.GetButtonDown (shootInput) == true && isGrounded == false) {
            SlamAttack ();
        }
    }

    void SetCurWeapon () {
        if (Input.GetAxis (switchWeaponInput) != 0 && IsInvoking ("NoSwitchWeapon") == false) {
            if (curWeapon == Weapon.Gun) {
                curWeapon = Weapon.Spear;
            } else {
                curWeapon = Weapon.Gun;
            }
            Invoke ("NoSwitchWeapon", 0.3f);
        }

        if (curWeapon == Weapon.Gun && hasGun == false) {
            curWeapon = Weapon.Spear;
        }
        if (curWeapon == Weapon.Spear && hasSpear == false) {
            curWeapon = Weapon.Gun;
        }
    }

    void NoSwitchWeapon () {

    }

    void SlamAttack () {
        if (IsInvoking ("CantAttack") == false) {
            transform.rotation = Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
            for (int i = 0; i < attackTrails.Length; i++) {
                attackTrails[i].emitting = true;
            }
            curState = State.SlamAttack;
            anim.Play ("JumpAttackStart");
            movev3 = transform.forward * moveStats[0].speed * AnalMagnitude ();
            movev3.y = jumpStrength;
            SpawnAudio.SpawnVoice (voiceLines[3], 0, 1, 1, 0);

            //slam
        }
    }

    void GetAttackInput (int attack) {
        AttackStats at = attacks[attack];
        if (canBuffer == true && bufferedAttack != -666) {
            at = attacks[bufferedAttack];
        }
        if (at.hasFollowAttack == true) {
            bufferedAttack = at.followUpAttack;
        } else {
            bufferedAttack = -666;
        }
        anim.Play (at.animation);
        transform.rotation = Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
        SpawnAudio.SpawnVoice (voiceLines[at.voiceID], 0, 1, 1, at.voiceDelay);
        curState = State.Attack;
        curAccDec = 0;
        Vector3 helper = transform.TransformDirection (at.localVelocity);
        movev3.x = helper.x;
        movev3.z = helper.z;
        for (int i = 0; i < attackTrails.Length; i++) {
            attackTrails[i].emitting = true;
        }
        CancelInvoke ("StopAttack");
        Invoke ("StopAttack", at.totalTime);
        anim.SetFloat ("curSpeed", 0);
        canBuffer = false;
        if (at.hasFollowAttack == true) {
            CancelInvoke ("CanBuffer");
            Invoke ("CanBuffer", at.bufferStartTime);
            CancelInvoke ("StopCanBuffer");
            Invoke ("StopCanBuffer", at.bufferDuration + at.bufferStartTime);
        } else {
            Invoke ("CantAttack", at.totalTime + at.bufferDuration);
        }
        Invoke ("IsStartingUp", at.startupTime);

        //aim helper
        Collider[] col = Physics.OverlapSphere (transform.position, 3, LayerMask.GetMask ("Enemy"), QueryTriggerInteraction.Collide);
        if (col.Length > 0 && Vector3.Dot ((col[0].transform.position - transform.position).normalized, transform.forward) > 0) {
            angleGoal = Quaternion.LookRotation (transform.position - col[0].transform.position, Vector3.up).eulerAngles.y;
            angleGoal += 180;
            transform.rotation = Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
        }
    }

    void IsStartingUp () {

    }

    void CantAttack () {

    }

    void CanBuffer () {
        canBuffer = true;
    }

    void StopCanBuffer () {
        canBuffer = false;
    }

    void StopAttack () {
        curState = State.Normal;
        for (int i = 0; i < attackTrails.Length; i++) {
            attackTrails[i].emitting = false;
        }
    }

    public void ActivateHitbox (int num) {
        hitboxes[num].SetActive (true);
        SpawnAudio.AudioSpawn (dashAudio[0], 0f, Random.Range (0.75f, 1.25f), 2);
        playerCam.SmallShake (0.2f);
        // Instantiate(hitEffectParticle,hitboxes[num].transform.position,Quaternion.identity);
    }

    public void HitSuccessEffects (int hitter) {
        playerCam.MediumShake (0.3f);
        playerCam.transform.Rotate (Random.Range (-7, 7), 0, Random.Range (-7, 7));
        Instantiate (hitEffectParticle, hitboxes[hitter].transform.position, Quaternion.identity);
        timescaleManager.SlowMo (0.15f, 0.05f);
        SpawnAudio.AudioSpawn (hitEffectAudio, 0f, Random.Range (4.95f, 6.45f), 1);
        SpawnAudio.AudioSpawn (hitEffectAudio, 0f, Random.Range (0.95f, 1.45f), 1);
        SpawnAudio.AudioSpawn (dashAudio[1], 0.3f, Random.Range (4.5f, 5.5f), 1);

        playerCam.SpeedLines (0.1f, 0);

    }

    public void SetDashInvisible (bool isDash) {
        for (int i = 0; i < dashInvisible.Length; i++) {
            dashInvisible[i].SetActive (!isDash);
        }
        for (int i = 0; i < dashVisible.Length; i++) {
            dashVisible[i].SetActive (isDash);
        }

        heinzHead.enabled = SaveSystem.LoadStuff ().heinzHeadMode;
    }

    void WaitShoot () {

    }

    void IgnoreDashInput () {

    }

    void StopDash () {
        for (int i = 0; i < attackTrails.Length; i++) {
            attackTrails[i].emitting = false;
        }
        if (curState == State.Dash) {
            curState = State.Normal;
            playerCam._enabled = true;
            SetDashInvisible (false);
            playerCam.MediumShake (0.2f);
            Invoke ("IgnoreDashInput", 0.1f);

            if (IsGrounded () == false) {
                canAirDash = false;
            }
            Instantiate (stopDashParticle, transform.position + transform.up, Quaternion.identity);

            if (hasDashEX == true && IsInvoking ("StopInvincible") == false) {
                hitbox.enabled = true;
            }

        }
    }

    public void GetHit () {
        if (SaveSystem.LoadStuff ().oneHitDieMode == false) {
            if (SaveSystem.LoadStuff ().godMode == false) {
                curState = State.Knockback;
                if (hitbox.hp / maxHP < 0.3f && lowHPPostProccesing.activeSelf == false) {
                    getHitPP.weight = 1;
                    Invoke ("SetHitPPWeight", 0);
                    playerCam.HardShake (0.3f);
                    timescaleManager.SlowMo (1, 0);
                    SpawnAudio.AudioSpawn (getHitAudio, 0, 1, 2);
                    playerCam.ripple.Emit ();
                    playerCam.transform.LookAt (transform.position);
                } else {
                    playerCam.MediumShake (0.2f);
                    timescaleManager.SlowMo (0.3f, 0f);
                }
                playerCam.SpeedLines (0.1f, 0);
                Instantiate (getHitParticle, transform.position + transform.up, Quaternion.identity);
                SetDashInvisible (false);
                movev3.x = -transform.forward.x * 10;
                movev3.z = -transform.forward.z * 10;
                Invoke ("StopKnockback", 0.2f);
                curAccDec = 0;
                playerCam._enabled = true;
                anim.Play ("GetHit");
                SpawnAudio.SpawnVoice (voiceLines[Random.Range (6, 9)], 0, 1, 1, 0f);
                // spear.SetActive (false);
                // snekSpear.SetActive(false);
                SetSpearActive (true);

                GetComponent<Hitbox> ().enabled = false;
                StopCoroutine ("HitFlash");
                StartCoroutine ("HitFlash");
                CancelInvoke ("StopInvincible");
                Invoke ("StopInvincible", 1);
            } else {
                hitbox.hp = maxHP;
            }
        } else if (curState != State.Death) {
            hitbox.deathEv.Invoke ();
        }

    }

    void StopInvincible () {
        GetComponent<Hitbox> ().enabled = true;
        if (curState != State.Dash) {
            for (int i = 0; i < dashInvisible.Length; i++) {
                dashInvisible[i].SetActive (true);
            }
        }
    }

    public IEnumerator HitFlash () {
        if (GetComponent<Hitbox> ().enabled == false) {
            if (dashInvisible[0].activeSelf == true) {
                if(timescaleManager.isPaused == false){
                for (int i = 0; i < dashInvisible.Length; i++) {
                    dashInvisible[i].SetActive (false);
                }
                }
            } else if (curState != State.Dash) {
                for (int i = 0; i < dashInvisible.Length; i++) {
                    dashInvisible[i].SetActive (true);
                }
            }
            yield return new WaitForSecondsRealtime (0.02f);
            StartCoroutine ("HitFlash");
        }
    }

    public void Die () {
        if (SaveSystem.LoadStuff ().godMode == false) {
            hitbox.hp = 0;
            curState = State.Death;
            anim.Play ("Death", 0);
            Camera.main.cullingMask = (1 << 9) | (1 << 10);
            playerCam.HardShake (0.2f);
            playerCam._enabled = false;
            movev3 = Vector3.zero;
            SpawnAudio.SpawnVoice (voiceLines[6], 0, 1f, 1, 1.5f);
            SpawnAudio.AudioSpawn (slamAttackAudio, 0.5f, 1, 1);
            SetDashInvisible (false);
            //spear.SetActive (false);
            //snekSpear.SetActive(false);
            SetSpearActive (true);
            hitbox.enabled = false;
            hpBar.curPercent = 0;
            GetComponent<Hitbox> ().enabled = false;
            if (GameObject.FindGameObjectWithTag ("BackgroundCam") != null) {
                GameObject.FindGameObjectWithTag ("BackgroundCam").SetActive (false);
            }
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        } else {
            hitbox.hp = maxHP;
        }
    }

    void StopKnockback () {
        curState = State.Normal;
        curAccDec = 0;
    }

    void SetHitPPWeight () {
        getHitPP.weight = Mathf.MoveTowards (getHitPP.weight, 0, Time.deltaTime / 2);
        if (getHitPP.weight != 0) {
            Invoke ("SetHitPPWeight", 0);
        }
    }

    void SetParryPPWeight () {
        parryPP.weight = Mathf.MoveTowards (parryPP.weight, 0, Time.deltaTime / 2);
        if (parryPP.weight != 0) {
            Invoke ("SetParryPPWeight", 0);
        }
    }

    bool isReverse = false;
    void Reverse () {
        if (hasReverse == true) {
            if (Input.GetButtonDown (reverseInput) == true) {
                if (isReverse == false) {
                    reverseMats.SetToGroup ();
                    hitbox.friends[2] = 3;
                } else {
                    normalMats.SetToGroup ();
                    hitbox.friends[2] = 4;
                }
                isReverse = !isReverse;
                playerCam.ReverseColors (Time.deltaTime);
                timescaleManager.SlowMo (0.2f, 0);
                playerCam.ripple.Emit ();
            }
        }
    }

    void FinalMove () {
        if (curState != State.WallSlide) {
            Vector3 oldPos = transform.position;
            if (ThreeDMode == false) {
                Vector3 oldAngle = cameraTransform.eulerAngles;
                cameraTransform.eulerAngles = new Vector3 (0, playerCam.angleGoal.y, 0);

                movev3 = cameraTransform.TransformDirection (new Vector2 (movev3.x, movev3.z).magnitude * Mathf.Clamp (movev3.x + movev3.z, -1, 1), movev3.y, 0);

                cameraTransform.eulerAngles = oldAngle;
            }
            cc.Move (new Vector3 (movev3.x, 0, movev3.z) * Time.deltaTime);
            cc.Move (new Vector3 (0, movev3.y, 0) * Time.deltaTime);
        }
    }

    //UI
    void InteractCheck () {
        textIcon.SetActive ((interactables.Count > 0));
        if (interactables.Count > 0) {
            curModeText.text = "Interact";
        } else {
            textIcon.transform.localScale = Vector3.zero;
        }
        if (Input.GetButtonDown (shootInput) == true && interactables.Count > 0 && isGrounded == true) { //here
            Interact chosenOne = interactables[0];
            for (int i = 0; i < interactables.Count; i++) {
                if (interactables[i].priority > chosenOne.priority) {
                    chosenOne = interactables[i];
                }
            }

            chosenOne.Activate (this);
        }
    }

    public void StartUIDialogue (DialogueHolder holder) {
        diaUI.curDia = 0;
        curState = State.Dialogue;
        textIcon.SetActive (false);

        diaUI.curHolder = holder;
        diaUI.firstInput = true;
        if (curWeapon != Weapon.Spear) {
            // spear.SetActive (false);
            // snekSpear.SetActive(false);
            SetSpearActive (true);
        }
        angleGoal = transform.eulerAngles.y;
        curAccDec = 0;
        anim.SetFloat ("curSpeed", 0);

    }

    void SetWillpowerBar () {
        if (SaveSystem.LoadStuff ().godMode == true) {
            willpower = 100;
        }
        if (isGrounded == true) {
            willpower = Mathf.MoveTowards (willpower, 100, Time.deltaTime * willpowerRefillSpeed);
        }
        willPowerBar.curPercent = willpower;
    }

    float fidgetTimer = 0;
    void IdleFidget () {
        if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Idle") == true && interactables.Count <= 0 && AnalMagnitude () == 0) {
            if (anim.GetFloat ("fidget") == 0) {
                fidgetTimer = Mathf.MoveTowards (fidgetTimer, 6, Time.deltaTime);
                if (fidgetTimer > 5.9f) {
                    anim.SetFloat ("fidget", (float) ((int) Random.Range (0, 3)) / 2f);
                    anim.Play ("Idle", 0, 0);
                }
            } else if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.999f) {
                fidgetTimer = 0;
                anim.SetFloat ("fidget", 0);
                anim.Play ("Idle", 0, 0);
            }
        } else {
            fidgetTimer = 0;
            anim.SetFloat ("fidget", 0);
        }

        //couldn't find a good place to put this, didn't feel like adding one
        if (spear.activeSelf == true || snekSpear.activeSelf == true) {
            anim.SetLayerWeight (1, 0);
        } else {
            anim.SetLayerWeight (1, 1);
        }

    }

    void SetSpearActive (bool onBack) {
        if (SaveSystem.LoadStuff ().snekwurmSpear == false) {
            spear.SetActive (!onBack);
            backspear.SetActive (onBack);

            snekSpear.SetActive (false);
            backSnekSpear.SetActive (false);
        } else {
            snekSpear.SetActive (!onBack);
            backSnekSpear.SetActive (onBack);

            spear.SetActive (false);
            backspear.SetActive (false);
        }
    }

    void SetHPBar () {
        hpBar.curPercent = (hitbox.hp / maxHP) * 100;
    }

    public void SetHairWindVel (Vector3 dir, float str) {
        wind.direction = dir;
        wind.strength = str;
    }

    //DEBUG

    void DebugMoves () {
        if (Input.GetKeyDown (KeyCode.Alpha0) == true) {
            cc.Move (new Vector3 (0, 10, 0));
            movev3.y = 0;
        }
    }

}

[System.Serializable]
public class MoveStats {
    public float speed = 5;
    public float acceleration = 5;
    public float deceleration = 8;
    public float rotSpeed = 10;
}

[System.Serializable]
public class AttackStats {
    public string animation;
    public float startupTime = 0.1f;
    public float totalTime = 0.5f;
    public float activeTime = 0.1f;
    public float bufferStartTime = 0.2f;
    public float bufferDuration = 0.2f;
    public bool hasFollowAttack = false;
    public int followUpAttack = 0;
    public Vector3 localVelocity = Vector3.zero;
    public int voiceID = 0;
    public float voiceDelay = 0;
}