using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public bool isGround = false;
    public Slider staminaBar;

    public Animator animator;

    [Header("Footstep Audio")]
    public AudioClip[] FootstepAudioClips;
    [Range(0f, 1f)] public float FootstepAudioVolume = 0.5f;
    [Header("Stamina Settings")]
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private float maxStamina = 100f;        // Player Max Stamina
    [SerializeField] private float stamina = 100f;           // Current Stamina
    [SerializeField] private float staminaDrain = 20f;       // tốc độ giảm khi chạy (mỗi giây)
    [SerializeField] private float staminaRegen = 10f;       // tốc độ hồi khi KHÔNG chạy
    public bool canRun = true; 
    public bool canMove = true; // lock move

    [Header("Movement Control")]
    PlayerMovementState currentState = PlayerMovementState.Idle;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;

    [Header("Get Input")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 movement;

    [Header("Rotation Control")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stopAngle = 2f;
    public static Action OnForceingLook;
    public event Action OnStopForceingLook;
    enum PlayerMovementState
    {
        Idle,
        Walking,
        Running,
        Crouching
    }


    void Start()
    {
        staminaBar.maxValue = maxStamina;
        staminaBar.value = stamina;
        PauseGameInput.Instance.OnGamePaused += OnPauseGame;
        PauseGameInput.Instance.OnGameResume += OnResumeGame;
        DialogueBox.OnDialogueStart += OnPauseGame;
        DialogueBox.OnDialogueEnd += OnResumeGame;
    }

    void Update()
    {
        if (!canMove)
        {
            SetState(PlayerMovementState.Idle);
            return;
        }
        HandleInputMovement();
        HandleStamina();
    }
    void FixedUpdate()
    {
        HandleMovement();
    }
    void SetState(PlayerMovementState state)
    {
        this.currentState = state;
        UpdateAnimation();
        UpdateSpeed();

    }

    private void UpdateSpeed()
    {
        switch (currentState)
        {
            case PlayerMovementState.Idle:
                currentSpeed = 0f;
                break;
            case PlayerMovementState.Walking:
                currentSpeed = walkingSpeed;
                break;
            case PlayerMovementState.Running:
                currentSpeed = runningSpeed;
                break;
            case PlayerMovementState.Crouching:
                currentSpeed = crouchSpeed;
                break;
        }
    }

    private void UpdateAnimation()
    {
        switch(currentState)
        {
            case PlayerMovementState.Idle:
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouching", false);
                break;
            case PlayerMovementState.Walking:
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouching", false);
                break;
            case PlayerMovementState.Running:
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                animator.SetBool("isCrouching", false);
                break;
            case PlayerMovementState.Crouching:
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouching", true);
                break;
        }
    }

    void HandleInputMovement()
    {
        float ipHorizontal = Input.GetAxis("Horizontal");

        float ipVertical = Input.GetAxis("Vertical");

        inputVector = transform.right * ipHorizontal + transform.forward * ipVertical;

        movement = inputVector.normalized;

        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (isShiftPressed && canRun)
        {
            SetState(PlayerMovementState.Running);
        } 
        else
        {
            if (movement.magnitude > 0)
            {
                SetState(PlayerMovementState.Walking);
            }
            else
            {
                SetState(PlayerMovementState.Idle);
            }
        }
        //HandleCrouch();

        //rb.MovePosition(transform.position + movement * currentSpeed * Time.deltaTime);

        //if (Input.GetKeyDown(KeyCode.Space) && isGround== true)
        //{
        //    rb.AddForce(new Vector3(0, 1, 0) * 5 , ForceMode.Impulse);
        //    isGround = false;
        //}
        ////HandleRotation(movement);

    }
    void HandleMovement()
    {
        if (movement.magnitude > 0)
        {

        }
        rb.MovePosition(transform.position + movement * currentSpeed * Time.fixedDeltaTime);
    }


    //void HandleCrouch()
    //{
    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        isCrouching = !isCrouching;
    //        animator.SetBool("isCrouching", isCrouching);

    //        CapsuleCollider col = GetComponent<CapsuleCollider>();
    //        if (col != null)
    //        {
    //            if (isCrouching)
    //            {
    //                col.height = 1.0f;
    //                col.center = new Vector3(0, 0.5f, 0);

    //                Vector3 pos = transform.position;
    //                pos.y += 0.4f; // nâng để chân chạm ground
    //                transform.position = pos;
    //            }
    //            else
    //            {
    //                col.height = 1.8f;
    //                col.center = new Vector3(0, 0.9f, 0);

    //                Vector3 pos = transform.position;
    //                pos.y -= 0.4f; // hạ về vị trí đứng bình thường
    //                transform.position = pos;
    //            }
    //        }
    //    }
    //}

    void HandleStamina()
    {
        staminaBar.value = stamina;

        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // Nếu đang chạy và còn stamina → giảm
        if (isShiftPressed && isMoving && canRun && !isCrouching)
        {
            stamina -= staminaDrain * Time.deltaTime;

            if (stamina <= 0)
            {
                stamina = 0;
                canRun = false;  
            }
        }
        else
        {
            // Hồi năng lượng khi không chạy
            stamina += staminaRegen * Time.deltaTime;
            if (stamina > maxStamina)
                stamina = maxStamina;

            // Cho phép chạy lại khi hồi > 30%
            if (stamina > maxStamina * 0.3f)
                canRun = true;
        }
    }

    public void LookAtTarget(Transform target)
    {
        StartCoroutine(RotateTowardsTarget(target));
    }

    IEnumerator RotateTowardsTarget(Transform target)
    {
        while (true)
        {
            Vector3 dir = target.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (angleDifference < stopAngle)
            {
                transform.rotation = targetRotation;
                break;
            }
            yield return null;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }


    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }
    }

    public void OnPauseGame()
    {
        canMove = false;
    }

    public void OnResumeGame()
    {
        canMove = true;
    }
    //public void HandleRotation(Vector3 playerMovementInput)
    //{
    //    Vector3 lookDirection = playerMovementInput;
    //    lookDirection.y = 0;

    //    if (lookDirection != Vector3.zero)
    //    {
    //        Quaternion rotation = Quaternion.LookRotation(lookDirection);
    //        transform.rotation = rotation;
    //    }
    //}
}
