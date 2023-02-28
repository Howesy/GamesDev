using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonControllerPrototype : MonoBehaviour
{
    [SerializeField] AudioSource _playerAudioSource;

    [SerializeField] private AudioClip[] _walkingClips;
    [SerializeField] private AudioClip[] _runningClips;

    public PlayerVitals playerVitals;

    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    bool isRunning;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    private float  _footstepTimer;
    public LayerMask playermask;


    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;
    private bool _isMoving = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerVitals = GetComponent<PlayerVitals>();
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _footstepTimer = 1.0f;
    }

    void Update()
    {

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);
        //Move depending on input
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        _isMoving = curSpeedX != 0 || curSpeedY != 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }


        if (!characterController.isGrounded && canMove)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    


        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        Handle_Footsteps();
    }
    private void Handle_Footsteps()
    {
        if(_isMoving)
        {
            FootStepPlayer(isRunning);
        }

    }

    private void FootStepPlayer(bool isRunning )
    {
        _footstepTimer -= Time.deltaTime;

        if (_footstepTimer <= 0)
        {

            if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit))
            {
                AudioClip clip;

                if(isRunning)
                {
                    _footstepTimer = .4f;
                    clip = GetRandomRunningClip();
                }
                else
                {
                    _footstepTimer = .6f;
                    clip = GetRandomWalkingClip();
                }

                _playerAudioSource.PlayOneShot(clip, 1);
            }
        }
    }

    private AudioClip GetRandomWalkingClip()
    {
        return _walkingClips[UnityEngine.Random.Range(0, _walkingClips.Length)];
    }

    private AudioClip GetRandomRunningClip()
    {
        return _runningClips[UnityEngine.Random.Range(0, _runningClips.Length)];
    }
}
