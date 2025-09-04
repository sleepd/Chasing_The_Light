using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float switchSpeed = 5f;
    [SerializeField] float switchTime = 0.5f;
    [SerializeField] float switchDistance = 8f;
    [SerializeField] float gravity = 5f;
    [SerializeField] float jumpForce = 10f;
    enum TRACK
    {
        Left, Center, Right
    }

    enum STATE
    {
        Idle,
        Moving,
        Jumping,
        Dead
    }

    TRACK track;
    InputSystem_Actions input;
    // CharacterController characterController;
    bool isJumping = false;
    Vector3 velocity;
    Vector3 moveInput;
    STATE state;
    Vector3 moveStartPos;
    Vector3 moveTarget;
    float switchTimePassed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = new();
        // characterController = GetComponent<CharacterController>();
        input.Player.Enable();
        velocity = Vector3.zero;
        track = TRACK.Center;
        EnterIdle();
    }



    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.Idle:
                HandleIdle();
                break;
            case STATE.Jumping:
                HandleJumping();
                break;
            case STATE.Moving:
                HandleMoving();
                break;
            case STATE.Dead:
                HandleDead();
                break;
        }

        HandleMovement();
    }

    void Jump()
    {
        velocity.y += jumpForce;
    }

    void HandleMovement()
    {
        transform.localPosition += velocity * Time.deltaTime;
    }

    void GroundCheck()
    {
        if (velocity.y < 0 && transform.position.y < 0)
        {
            isJumping = false;
            velocity.y = 0;
            Vector3 pos = transform.position;
            pos.y = 0;
            transform.position = pos;
            ExitJumping();
            EnterIdle();
        }
    }

    void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
    }

    void EnterIdle()
    {
        state = STATE.Idle;
        input.Player.Left.performed += OnPressLeft;
        input.Player.Right.performed += OnPressRight;
        input.Player.Jump.performed += OnPressJump;

    }

    void HandleIdle()
    {

    }

    void ExitIdle()
    {
        input.Player.Left.performed -= OnPressLeft;
        input.Player.Right.performed -= OnPressRight;
        input.Player.Jump.performed -= OnPressJump;
    }

    void EnterJumping()
    {
        state = STATE.Jumping;
        Jump();
    }

    void HandleJumping()
    {
        ApplyGravity();
        GroundCheck();
    }

    void ExitJumping()
    {

    }

    void EnterMoving()
    {
        state = STATE.Moving;
        switchTimePassed = 0;
    }

    void HandleMoving()
    {
        switchTimePassed += Time.deltaTime;
        switchTimePassed = Mathf.Clamp(switchTimePassed, 0, switchTime);
        float t = switchTimePassed / switchTime;
        Vector3 pos = Vector3.Lerp(moveStartPos, moveTarget, t);
        transform.localPosition = pos;

        if (switchTimePassed == switchTime)
        {
            transform.localPosition = moveTarget;
            ExitMoving();
            EnterIdle();
        }
    }

    void ExitMoving()
    {

    }

    void OnPressLeft(InputAction.CallbackContext context)
    {
        switch (track)
        {
            case TRACK.Left:
                return;
            case TRACK.Center:
                track = TRACK.Left;
                break;
            case TRACK.Right:
                track = TRACK.Center;
                break;
        }
        ExitIdle();
        EnterMoving();
        moveStartPos = transform.localPosition;
        moveTarget = transform.localPosition;
        moveTarget.x -= 6;

    }

    void OnPressRight(InputAction.CallbackContext context)
    {
        switch (track)
        {
            case TRACK.Right:
                return;
            case TRACK.Center:
                track = TRACK.Right;
                break;
            case TRACK.Left:
                track = TRACK.Center;
                break;
        }
        ExitIdle();
        EnterMoving();
        moveStartPos = transform.localPosition;
        moveTarget = transform.localPosition;
        moveTarget.x += 6;
    }

    void OnPressJump(InputAction.CallbackContext context)
    {
        ExitIdle();
        EnterJumping();
    }

    void EnterDead()
    {
        state = STATE.Dead;
        AnchorController.Instance.Stop();
        velocity = Vector3.zero;
    }

    void HandleDead()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        ExitIdle();
        EnterDead();
    }
}
