using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (CharacterController))]

public class PlayerController : MonoBehaviour {

    CharacterController cc;
    public Vector3 movev3;
    public Transform cameraTransform;
    PlayerCam playerCam;
    public Animator anim;
    public enum State {
        Normal,
        Dash
    }
    public State curState = State.Normal;
    [Header ("Input")]
    public string horInput = "Horizontal";
    public string vertInput = "Vertical";
    public string jumpInput = "Jump";
    public string dashInput = "Dash";
    [Header ("Stats")]
    public MoveStats[] moveStats;
    public int curMoveStats = 0;
    public bool isGrounded = true;
    public float jumpStrength = 20;
    public float gravityStrength = -29.81f;
    [Header ("Dash")]
    [SerializeField] GameObject[] dashVisible;
    [SerializeField] GameObject[] dashInvisible;
    [SerializeField] float dashTime = 0.4f;
    [SerializeField] float dashSpeed = 4;
    bool canAirDash = true;

    void Start () {
        cc = GetComponent<CharacterController> ();
        angleGoal = transform.eulerAngles.y;
        playerCam = cameraTransform.GetComponent<PlayerCam> ();
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
                DashInput ();
                break;
            case State.Dash:
                isGrounded = false;
                Dash ();
                FinalMove ();
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
        return IsInvoking ("CayoteTime");
    }

    void CayoteTime () {

    }

    void DashInput () {
        if (isGrounded == true) {
            canAirDash = true;
        }
        if (Input.GetButtonDown (dashInput) == true && IsInvoking ("IgnoreDashInput") == false) {
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
                Invoke ("StopDash", dashTime);
                curAccDec = 1;
                transform.eulerAngles = new Vector3 (transform.eulerAngles.x, angleGoal, transform.eulerAngles.z);
                playerCam._enabled = false;

                for (int i = 0; i < dashInvisible.Length; i++) {
                    dashInvisible[i].SetActive (false);
                }
                for (int i = 0; i < dashVisible.Length; i++) {
                    dashVisible[i].SetActive (true);
                }

                playerCam.SmallShake (dashTime);

            }
        }
    }

    void IgnoreDashInput () {

    }

    void StopDash () {
        if (curState == State.Dash) {
            curState = State.Normal;
            playerCam._enabled = true;
            for (int i = 0; i < dashInvisible.Length; i++) {
                dashInvisible[i].SetActive (true);
            }
            for (int i = 0; i < dashVisible.Length; i++) {
                dashVisible[i].SetActive (false);
            }
            playerCam.MediumShake (0.2f);
            Invoke ("IgnoreDashInput", 0.2f);

        }
    }

    void FinalMove () {
        cc.Move (movev3 * Time.deltaTime);
    }

}

[System.Serializable]
public class MoveStats {
    public float speed = 5;
    public float acceleration = 5;
    public float deceleration = 8;
    public float rotSpeed = 10;
}