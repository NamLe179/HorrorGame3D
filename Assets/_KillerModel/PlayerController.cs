using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Footstep Audio")]
    public AudioSource footstepAudio;  // Child AudioSource Player
    public AudioClip walkClip, runClip;
    public float walkInterval = 0.7f, runInterval = 0.4f;

    [Header("Movement")]
    public float walkSpeed = 3f, runSpeed = 6f, crouchSpeed = 1.5f;
    private CharacterController controller;
    private float nextStepTime, currentSpeed;
    private bool isCrouching;

    [Header("Input Keys")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Animator Parameters")]
    public string walkParam = "isWalking";     // bool
    public string runParam = "isRunning";      // bool
    public string crouchParam = "isCrouching"; // bool

    private Animator animator;
    private Vector3 lastPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
        controller = GetComponent<CharacterController>();
        if (footstepAudio == null) footstepAudio = GetComponentInChildren<AudioSource>();
        footstepAudio.spatialBlend = 1f;
    }

    void Update()
    {
        // ===  WASD ===
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(moveX, 0, moveZ).normalized;

        // === STATE ===
        bool isInput = input.magnitude > 0.1f;
        bool isRunning = isInput && Input.GetKey(runKey);
        bool isCrouching = Input.GetKey(crouchKey);

        // === Location  ===
        if (isCrouching)
        {
            currentSpeed = crouchSpeed; footstepAudio.volume = 0f; footstepAudio.maxDistance = 0f;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed; footstepAudio.clip = runClip; footstepAudio.volume = 1f; footstepAudio.maxDistance = 20f;
        }
        else
        {
            currentSpeed = walkSpeed; footstepAudio.clip = walkClip; footstepAudio.volume = 0.4f; footstepAudio.maxDistance = 8f;
        }

        controller.Move(input * currentSpeed * Time.deltaTime);

        if (input.magnitude > 0 && Time.time > nextStepTime)
        {
            footstepAudio.Play();
            nextStepTime = Time.time + (isRunning ? runInterval : walkInterval);
        }

        // ===  ANIMATOR PARAMETERS ===
        animator.SetBool(walkParam, isInput && !isRunning && !isCrouching);
        animator.SetBool(runParam, isRunning);
        animator.SetBool(crouchParam, isCrouching);
    }
}
