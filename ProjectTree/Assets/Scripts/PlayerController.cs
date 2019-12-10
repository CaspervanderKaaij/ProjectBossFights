using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
[RequireComponent (typeof (CharacterController))]

public class PlayerController : MonoBehaviour {

    CharacterController cc;
    public Vector3 movev3;
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
        Parry
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
    [SerializeField] AudioClip slamAttackAudio;
    [Header ("Hitboxes")]
    [SerializeField] GameObject hitboxParent;
    List<GameObject> hitboxes = new List<GameObject> ();
    [SerializeField] AudioClip hitEffectAudio;
    [Header ("Shooting")]
    public GunWeapon gunWeapon;
    [SerializeField] GameObject shootMagicCircle;
    [SerializeField] float shootWPCost = 5;
    [Header ("Particles")]
    [SerializeField] GameObject stopDashParticle;
    [SerializeField] GameObject getHitParticle;
    [SerializeField] AudioClip getHitAudio;
    [SerializeField] ParticleSystem walkParticles;
    [SerializeField] GameObject hitEffectParticle;
    [SerializeField] GameObject WdRipple;
    [Header ("Voice Lines")]
    public AudioClip[] voiceLines;

    [Header ("UI")]
    [Header ("HUD")]
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

    void Start () {
        if (FindObjectOfType<StartSaveInitializer> () != null) {
            FindObjectOfType<StartSaveInitializer> ().OnEnable ();
        }
        cc = GetComponent<CharacterController> ();
        angleGoal = transform.eulerAngles.y;
        playerCam = cameraTransform.GetComponent<PlayerCam> ();
        hitbox = GetComponent<Hitbox> ();
        // maxHP = hitbox.hp;
        timescaleManager = FindObjectOfType<TimescaleManager> ();
        for (int i = 0; i < hitboxParent.transform.childCount; i++) {
            hitboxes.Add (hitboxParent.transform.GetChild (i).gameObject);
        }
    }

    void Update () {
        anim.transform.localEulerAngles = new Vector3 (anim.transform.localEulerAngles.x, anim.transform.localEulerAngles.y, 0);
        shootMagicCircle.SetActive (false);
        switch (curState) {
            case State.Normal:
                Jump (jumpStrength);
                isGrounded = IsGrounded ();
                SetAngle ();
                MoveForward ();
                Gravity ();
                FinalMove ();
                IdleFidget ();
                SetWillpowerBar ();
                SetHPBar ();
                DashInput ();
                SetCurWeapon ();
                GetParryInput ();
                if (interactables.Count == 0) {
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
                    playerCam.MediumShake (0.1f);
                    // playerCam.ripple.Emit ();
                    Instantiate (WdRipple, hitboxes[1].transform.position, Quaternion.Euler (90, 0, 0));
                    for (int i = 0; i < 10; i++) {
                        Instantiate (stopDashParticle, transform.position, Quaternion.identity);
                    }
                    SpawnAudio.AudioSpawn (slamAttackAudio, 0.5f, Random.Range (4, 5), 0.5f);
                    timescaleManager.SlowMo (0.3f, 0.2f, 0.1f);

                } else {
                    DashInput ();
                }
                break;
            case State.Gun:
                ShootInput ();
                SetWillpowerBar ();
                Jump (jumpStrength);
                break;
        }
        lowHPPostProccesing.SetActive ((hitbox.hp / maxHP < 0.3f));
    }

    // Inputs

    float GetHorInput () {
        return Input.GetAxis (horInput);
    }

    float GetVertInput () {
        return Input.GetAxis (vertInput);
    }

    // Movement

    void GetParryInput () {
        if (Input.GetButtonDown (parryInput) == true && isGrounded == true && curWeapon == Weapon.Spear) {
            curState = State.Parry;
            movev3 = Vector3.zero;
            anim.Play ("Parry");
            playerCam.SmallShake (0.1f);
            Invoke ("StopAttack", 0.1f);
            isGrounded = true;
            wasGrounded = true;
            curAccDec = 0;
            movev3.x = 0;
            movev3.z = 0;

            SpawnAudio.SpawnVoice (voiceLines[6], 0, 1, 1, 0);
        }
    }

    public void ParrySuccess () {
        playerCam.HardShake (0.1f);
        playerCam.ripple.Emit ();
        timescaleManager.SlowMo (0.3f, 0.2f);
        Instantiate (hitEffectParticle, hitboxes[0].transform.position, Quaternion.identity);
        SpawnAudio.SpawnVoice (voiceLines[0], 0, 1, 1, 0);
        parryPP.weight = 1;
        Invoke ("SetParryPPWeight", 0);
        SpawnAudio.AudioSpawn (slamAttackAudio, 0.5f, Random.Range (4, 5), 0.5f);
    }

    float AnalAngle () {
        return Mathf.Atan2 (GetVertInput (), -GetHorInput ()) * Mathf.Rad2Deg;
    }

    float AnalMagnitude () {
        return Mathf.Min (1, new Vector2 (GetHorInput (), GetVertInput ()).sqrMagnitude);
    }

    float angleGoal = 0;
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

    Vector3 lastPos = Vector3.zero;
    void Dash () {
        Vector3 helper = transform.TransformDirection (0, 0, dashSpeed);
        movev3.x = helper.x;
        movev3.z = helper.z;
        movev3.y = 0;

        if (lastPos != Vector3.zero && Vector3.Distance (transform.position, lastPos) < 0.5f) {
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
            spear.SetActive (false);
            shootMagicCircle.SetActive (false);
            Instantiate (stopDashParticle, transform.position + transform.up, Quaternion.identity);
            playerCam.MediumShake (0.15f);

        }
        lastPos = transform.position;
    }

    float curAccDec = 0;
    void MoveForward () {

        if (AnalMagnitude () > 0) {
            curAccDec = Mathf.MoveTowards (curAccDec, AnalMagnitude (), Time.deltaTime * moveStats[curMoveStats].acceleration);
        } else {
            curAccDec = Mathf.MoveTowards (curAccDec, AnalMagnitude (), Time.deltaTime * moveStats[curMoveStats].deceleration);
        }

        Vector3 forwardhelper = transform.TransformDirection (0, 0, curAccDec * moveStats[curMoveStats].speed);
        movev3.x = forwardhelper.x;
        movev3.z = forwardhelper.z;

        anim.SetFloat ("curSpeed", AnalMagnitude ());
    }

    float jumpsLeft = 2;
    void Jump (float strength) {
        if (Input.GetButtonDown (jumpInput) == true) {
            Invoke ("JumpBuffer", 0.2f);
        }
        if (jumpsLeft > 0 && IsInvoking ("JumpBuffer") == true) {
            jumpsLeft--;
            if (jumpsLeft < maxJumps - 1 && (anim.GetCurrentAnimatorStateInfo (0).IsName ("Fall") == true || anim.GetCurrentAnimatorStateInfo (0).IsName ("ShootAir") == true || anim.GetCurrentAnimatorStateInfo (0).IsName ("DoubleJump") || anim.GetCurrentAnimatorStateInfo (0).IsName ("GetHit"))) {
                anim.Play ("DoubleJump");
            }
            movev3.y = strength;
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

        } else {
            movev3.y = Mathf.MoveTowards (movev3.y, gravityStrength, Time.deltaTime * 50);
        }
        if (isGrounded == true) {
            movev3.y = -1f;
        }
    }

    float groundedTime = 0;
    bool IsGrounded () {

        RaycastHit hit;
        // Debug.DrawRay (transform.position + new Vector3 (0, 0.1f, 0), Vector3.down * 0.5f, Color.red, 0);
        if (Physics.Raycast (transform.position + new Vector3 (0, 0.1f, 0), Vector3.down, out hit, 0.4f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) || cc.isGrounded == true) {
            //if (Vector3.Angle (hit.normal, transform.up) <= cc.slopeLimit) {
            if (groundHit == null || (Vector3.Angle (groundHit.normal, transform.up) <= cc.slopeLimit && groundHit.transform.tag != "NoFloor")) {
                if (groundedTime > Time.deltaTime / 2) {
                    groundHit = null;
                    if (movev3.y < 0) {
                        CancelInvoke ("CayoteTime");
                        Invoke ("CayoteTime", 0.1f);
                    }
                    if (hit.transform != null && hit.transform.GetComponent<LandAction> () != null) {
                        hit.transform.GetComponent<LandAction> ().Activate ();
                        footstep.curSoundType = hit.transform.GetComponent<LandAction> ().footstepSoundID;
                    } else {
                        footstep.curSoundType = 0;
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
                cc.Move (transform.forward * Time.deltaTime * Mathf.Abs (movev3.y / 2));
                transform.eulerAngles = oldEuler;
                curAccDec = 0;
                movev3.x = 0;
                movev3.z = 0;
                wasGrounded = false;
            }

        } else {
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

    void DashInput () {
        if (isGrounded == true) {
            canAirDash = true;
        }
        if (Input.GetButtonDown (dashInput) == true && IsInvoking ("IgnoreDashInput") == false && willpower > dashWPCost) {
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

                curState = State.Dash;
                SpawnAudio.AudioSpawn (dashAudio[0], 0f, Random.Range (4.5f, 4.5f), 1);
                SpawnAudio.AudioSpawn (dashAudio[1], 0.4f, Random.Range (0.75f, 1.25f), 1);
                willpower -= dashWPCost;
                Invoke ("StopDash", dashTime);
                curAccDec = 1;
                anim.SetFloat ("curSpeed", 0);
                transform.eulerAngles = new Vector3 (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
                playerCam._enabled = false;
                anim.Play ("Fall");

                SetDashInvisible (true);

                playerCam.SmallShake (dashTime);
                Invoke ("WaitShoot", dashTime + 0.2f);

            }
        }
    }

    void ShootInput () {
        spear.SetActive (false);
        curModeText.text = "Shoot Mode";
        shootMagicCircle.SetActive ((curState == State.Gun));
        if (curState == State.Gun) {
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z), Time.deltaTime * moveStats[curMoveStats].rotSpeed * 2);
            if (Input.GetButtonDown (shootInput) == true) {
                Invoke ("ShootInputBuffer", 0.3f);
            }
            if ((Input.GetAxis (shootInput) != 0 || IsInvoking ("ShootInputBuffer")) && IsInvoking ("WaitShoot") == false) {
                if (willpower > shootWPCost) {
                    CancelInvoke ("ShootBufferInput");
                    anim.Play ("GunShot", 0, 0f);
                    willpower -= shootWPCost;
                    gunWeapon.GetInput ();
                    Invoke ("WaitShoot", 0.3f);
                    timescaleManager.SlowMo (0.2f, 0.05f);
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
                anim.Play ("StartShoot", 0, 0f);
                Invoke ("WaitShoot", 0.3f);
                curAccDec = 0;
            } else if (Input.GetAxis (shootInput) != 0 && IsInvoking ("WaitShoot") == false && willpower > shootWPCost) {
                anim.Play ("ShootAir");
                movev3.y = jumpStrength / 1.5f;
                Invoke ("WaitShoot", 0.6f);
                timescaleManager.SlowMo (0.2f, 0.05f, 0.1f);
                Instantiate (hitEffectParticle, gunWeapon.spawnPoint.position, Quaternion.identity, gunWeapon.spawnPoint);
                SpawnAudio.AudioSpawn (dashAudio, 0, Random.Range (2.5f, 3), 1);
                playerCam.SmallShake (0.2f);
                gunWeapon.GetInput (new Vector3 (0, 0, -90));
                willpower -= shootWPCost;
                Invoke ("IgnoreDashInput", 0.2f);
            }
        }
    }

    void ShootInputBuffer () {

    }

    void SpearInput () {
        curModeText.text = "Spear Mode";
        spear.SetActive (true);
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
    }

    void NoSwitchWeapon () {

    }

    void SlamAttack () {
        if (IsInvoking ("CantAttack") == false) {
            transform.rotation = Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
            curState = State.SlamAttack;
            anim.Play ("JumpAttackStart");
            movev3 = transform.forward * moveStats[0].speed * curAccDec;
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
    }

    public void ActivateHitbox (int num) {
        hitboxes[num].SetActive (true);
        SpawnAudio.AudioSpawn (dashAudio[0], 0f, Random.Range (0.75f, 1.25f), 2);
        playerCam.SmallShake (0.2f);
        // Instantiate(hitEffectParticle,hitboxes[num].transform.position,Quaternion.identity);
    }

    public void HitSuccessEffects (int hitter) {
        playerCam.MediumShake (0.3f);
        Instantiate (hitEffectParticle, hitboxes[hitter].transform.position, Quaternion.identity);
        timescaleManager.SlowMo (0.05f, 0f);
        SpawnAudio.AudioSpawn (hitEffectAudio, 1f, Random.Range (0.95f, 1.45f), 0.1f);
        SpawnAudio.AudioSpawn (dashAudio[1], 0.3f, Random.Range (4.5f, 5.5f), 1);

    }

    public void SetDashInvisible (bool isDash) {
        for (int i = 0; i < dashInvisible.Length; i++) {
            dashInvisible[i].SetActive (!isDash);
        }
        for (int i = 0; i < dashVisible.Length; i++) {
            dashVisible[i].SetActive (isDash);
        }
    }

    void WaitShoot () {

    }

    void IgnoreDashInput () {

    }

    void StopDash () {
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

        }
    }

    public void GetHit () {
        curState = State.Knockback;
        if (hitbox.hp / maxHP < 0.3f && lowHPPostProccesing.activeSelf == false) {
            getHitPP.weight = 1;
            Invoke ("SetHitPPWeight", 0);
            playerCam.HardShake (0.3f);
            timescaleManager.SlowMo (0.5f, 0);
            SpawnAudio.AudioSpawn (getHitAudio, 0, 1, 2);
            playerCam.ripple.Emit ();
        } else {
            playerCam.MediumShake (0.3f);
            timescaleManager.SlowMo (0.2f, 0f);
        }
        Instantiate (getHitParticle, transform.position + transform.up, Quaternion.identity);
        SetDashInvisible (false);
        movev3.x = -transform.forward.x * 10;
        movev3.z = -transform.forward.z * 10;
        Invoke ("StopKnockback", 0.2f);
        curAccDec = 0;
        playerCam._enabled = true;
        anim.Play ("GetHit");
        SpawnAudio.SpawnVoice (voiceLines[Random.Range (6, 9)], 0, 1, 1, 0f);
        spear.SetActive (false);

    }

    public void Die () {
        curState = State.Death;
        anim.Play ("Death", 0);
        Camera.main.cullingMask = (1 << 9) | (1 << 10);
        playerCam.HardShake (0.2f);
        playerCam._enabled = false;
        movev3 = Vector3.zero;
        SpawnAudio.SpawnVoice (voiceLines[6], 0, 1f, 1, 1.5f);
        SpawnAudio.AudioSpawn (slamAttackAudio, 0.5f, 1, 1);
        SetDashInvisible (false);
        spear.SetActive (false);
        hitbox.enabled = false;
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

    void FinalMove () {
        // cc.Move (new Vector3 (movev3.x, 0, 0) * Time.deltaTime);
        //  cc.Move (new Vector3 (0, movev3.y, 0) * Time.deltaTime);
        //  cc.Move (new Vector3 (0, 0, movev3.z) * Time.deltaTime);
        cc.Move (movev3 * Time.deltaTime);
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
        diaUI.GetComponent<Animator> ().Play (0);

    }

    void SetWillpowerBar () {
        willpower = Mathf.MoveTowards (willpower, 100, Time.deltaTime * willpowerRefillSpeed);
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
        if (spear.activeSelf == true) {
            anim.SetLayerWeight (1, 0);
        } else {
            anim.SetLayerWeight (1, 1);
        }

    }

    void SetHPBar () {
        hpBar.curPercent = (hitbox.hp / maxHP) * 100;
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