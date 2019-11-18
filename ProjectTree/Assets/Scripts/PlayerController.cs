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
        Attack
    }
    public State curState = State.Normal;
    [Header ("Input")]
    public string horInput = "Horizontal";
    public string vertInput = "Vertical";
    public string jumpInput = "Jump";
    public string dashInput = "Dash";
    public string shootInput = "Shoot";
    [Header ("Stats")]
    public MoveStats[] moveStats;
    public int curMoveStats = 0;
    public bool isGrounded = true;
    bool wasGrounded = true;
    public float jumpStrength = 20;
    public float gravityStrength = -29.81f;
    [HideInInspector] public float willpower = 100;
    public float willpowerRefillSpeed = 10;
    [Header ("Dash")]
    [SerializeField] GameObject[] dashVisible;
    [SerializeField] GameObject[] dashInvisible;
    [SerializeField] float dashTime = 0.4f;
    [SerializeField] float dashSpeed = 4;
    bool canAirDash = true;
    [SerializeField] float dashWPCost = 10;
    public enum Weapon {
        Gun,
        Spear
    }

    [Header ("Weapon")] //here
    public Weapon curWeapon = Weapon.Gun;
    [SerializeField] AttackStats[] attacks;
    public bool canBuffer = false;
    int bufferedAttack = 0;
    [Header ("Shooting")]
    public GunWeapon gunWeapon;
    [SerializeField] float shootWPCost = 5;
    [Header ("Particles")]
    [SerializeField] GameObject stopDashParticle;
    [SerializeField] GameObject getHitParticle;
    [SerializeField] ParticleSystem walkParticles;

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
    [SerializeField] UIBar hpBar;
    Hitbox hitbox;
    float maxHP;

    void Start () {
        cc = GetComponent<CharacterController> ();
        angleGoal = transform.eulerAngles.y;
        playerCam = cameraTransform.GetComponent<PlayerCam> ();
        hitbox = GetComponent<Hitbox> ();
        maxHP = hitbox.hp;
        timescaleManager = FindObjectOfType<TimescaleManager> ();
    }

    void Update () {
        switch (curState) {
            case State.Normal:
                Jump (jumpStrength);
                isGrounded = IsGrounded ();
                SetAngle ();
                MoveForward ();
                Gravity ();
                FinalMove ();
                SetWillpowerBar ();
                SetHPBar ();
                DashInput ();
                if (textIcon.activeSelf == false) {
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
        }
    }

    // Inputs

    float GetHorInput () {
        return Input.GetAxis (horInput);
    }

    float GetVertInput () {
        return Input.GetAxis (vertInput);
    }

    // Movement

    float AnalAngle () {
        return Mathf.Atan2 (GetVertInput (), -GetHorInput ()) * Mathf.Rad2Deg;
    }

    float AnalMagnitude () {
        return Mathf.Min (1, new Vector2 (GetHorInput (), GetVertInput ()).sqrMagnitude);
    }

    float angleGoal = 0;
    void SetAngle () {
        if (AnalMagnitude () > 0) {
            angleGoal = AnalAngle () + cameraTransform.eulerAngles.y - 90;
        }
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z), Time.deltaTime * moveStats[curMoveStats].rotSpeed);
    }

    void Dash () {
        Vector3 helper = transform.TransformDirection (0, 0, dashSpeed);
        movev3.x = helper.x;
        movev3.z = helper.z;
        movev3.y = 0;
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

    void Jump (float strength) {
        if (Input.GetButtonDown (jumpInput) == true) {
            Invoke ("JumpBuffer", 0.2f);
        }
        if (isGrounded == true && IsInvoking ("JumpBuffer") == true) {
            movev3.y = strength;
            CancelInvoke ("JumpBuffer");
            CancelInvoke ("CayoteTime");
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

    bool IsGrounded () {
        RaycastHit hit;
        // Debug.DrawRay (transform.position + new Vector3 (0, 0.1f, 0), Vector3.down * 0.5f, Color.red, 0);
        if (Physics.Raycast (transform.position + new Vector3 (0, 0.1f, 0), Vector3.down, out hit, 0.3f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore) || cc.isGrounded == true) {
            if (movev3.y < 0) {
                CancelInvoke ("CayoteTime");
                Invoke ("CayoteTime", 0.05f);
            }
        }
        anim.SetBool ("grounded", IsInvoking ("CayoteTime"));
        if (IsInvoking ("CayoteTime") == true && wasGrounded == false) { }
        wasGrounded = IsInvoking ("CayoteTime");
        var emmision = walkParticles.emission;
        emmision.enabled = IsInvoking ("CayoteTime");
        return IsInvoking ("CayoteTime");
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
                willpower -= dashWPCost;
                Invoke ("StopDash", dashTime);
                curAccDec = 1;
                transform.eulerAngles = new Vector3 (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
                playerCam._enabled = false;

                SetDashInvisible (true);

                playerCam.SmallShake (dashTime);

            }
        }
    }

    void ShootInput () {
        curModeText.text = "Shoot Mode";
        if (Input.GetAxis (shootInput) != 0 && IsInvoking ("WaitShoot") == false) {
            if (willpower > shootWPCost) {
                willpower -= shootWPCost;
                gunWeapon.GetInput ();
                Invoke ("WaitShoot", 0.3f);
                playerCam.SmallShake (0.1f);
            }
        }
    }

    void SpearInput () {
        curModeText.text = "Spear Mode";
        if (Input.GetButtonDown (shootInput) == true && isGrounded == true) {
            GetAttackInput (0);
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
        curState = State.Attack; //here
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
        }
        Invoke ("IsStartingUp", at.startupTime);
    }

    void IsStartingUp () {

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

    void SetDashInvisible (bool isDash) {
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
        getHitPP.weight = 1;
        Invoke ("SetHitPPWeight", 0);
        playerCam.HardShake (0.3f);
        SetDashInvisible (false);
        movev3.x = -transform.forward.x * 30;
        movev3.z = -transform.forward.z * 30;
        Invoke ("StopKnockback", 0.1f);
        curAccDec = 0;
        playerCam._enabled = true;
        timescaleManager.SlowMo (0.3f, 0);
        playerCam.ripple.Emit ();
        anim.Play ("GetHit");
        Instantiate (getHitParticle, transform.position + transform.up, Quaternion.identity);

    }

    void StopKnockback () {
        curState = State.Normal;
    }

    void SetHitPPWeight () {
        getHitPP.weight = Mathf.MoveTowards (getHitPP.weight, 0, Time.deltaTime / 2);
        if (getHitPP.weight != 0) {
            Invoke ("SetHitPPWeight", 0);
        }
    }

    public void Die () {
        print ("die");
    }

    void FinalMove () {
        cc.Move (movev3 * Time.deltaTime);
    }

    //UI
    void InteractCheck () {
        textIcon.SetActive ((interactables.Count > 0));
        if (interactables.Count > 0) {
            curModeText.text = "Interact";
        }
        if (Input.GetButtonDown (shootInput) == true && interactables.Count > 0 && isGrounded == true) {
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

    void SetHPBar () {
        hpBar.curPercent = (hitbox.hp / maxHP) * 100;
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
}