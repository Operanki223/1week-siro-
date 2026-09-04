using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 3.0f;
    [SerializeField] private float jumpPower = 3.0f;
    [SerializeField] private GameObject camera_player;

    [SerializeField] private float drunkStrength = 1.5f;
    [SerializeField] private float drunkInterval = 0.5f;

    private CharacterController characterController;

    private Vector3 moveVelocity;

    private InputAction move;
    private InputAction jump;
    private InputAction turn;

    private float cameraAngle = 0;

    private Vector3 drunkOffset = Vector3.zero;
    private float drunkTimer = 0f;
    private Vector3 targetDrunkOffset;
    private bool canMove = true;

    public static PlayerController instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        var input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();

        move = input.currentActionMap.FindAction("move");
        jump = input.currentActionMap.FindAction("jump");
        turn = input.currentActionMap.FindAction("turn");
    }

    void Update()
    {
        if (!canMove)
        {
            moveVelocity = Vector3.zero;
            return;
        }

        if (!GameManager.instance.playGame)
        {
            StopPlayer();
            return;
        }

        ControllePlayer();
    }

    void ControllePlayer()
    {
        var moveValue = move.ReadValue<Vector2>();

        Vector3 moveDirection =
            transform.right * moveValue.x +
            transform.forward * moveValue.y;

        DrunkMove();

        moveDirection += drunkOffset;

        moveVelocity.x = moveDirection.x * moveSpeed;
        moveVelocity.z = moveDirection.z * moveSpeed;

        var turnValue = turn.ReadValue<Vector2>();

        transform.Rotate(
            0,
            turnValue.x * turnSpeed * Time.deltaTime,
            0
        );

        cameraAngle -=
            turnValue.y * turnSpeed * Time.deltaTime;

        cameraAngle = Mathf.Clamp(
            cameraAngle,
            -30f,
            45f
        );

        camera_player.transform.localRotation =
            Quaternion.Euler(cameraAngle, 0, 0);

        if (characterController.isGrounded)
        {
            if (jump.WasPressedThisFrame())
            {
                moveVelocity.y = jumpPower;
            }
            else
            {
                moveVelocity.y = -0.1f;
            }
        }
        else
        {
            moveVelocity.y +=
                Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(
            moveVelocity * Time.deltaTime
        );
    }

    public void StopPlayer()
    {
        canMove = false;

        moveVelocity = Vector3.zero;

        drunkOffset = Vector3.zero;
        targetDrunkOffset = Vector3.zero;
        drunkTimer = 0f;

        //Debug.Log("★★★ Player STOP ★★★");
    }
    public void StartPlayer()
    {
        canMove = true;

        moveVelocity = Vector3.zero;

        drunkOffset = Vector3.zero;
        targetDrunkOffset = Vector3.zero;
        drunkTimer = 0f;
    }

    public void ResetVelocity()
    {
        moveVelocity = Vector3.zero;
    }

    public void ResetCamera()
    {
        cameraAngle = 0;

        camera_player.transform.localRotation =
            Quaternion.Euler(0, 0, 0);
    }

    void DrunkMove()
    {
        drunkTimer += Time.deltaTime;

        if (drunkTimer >= drunkInterval)
        {
            drunkTimer = 0f;

            Vector3[] dirs =
            {
                transform.forward,
                -transform.forward,
                transform.right,
                -transform.right
            };

            targetDrunkOffset =
                dirs[Random.Range(0, dirs.Length)]
                * drunkStrength;
        }

        drunkOffset = Vector3.Lerp(
            drunkOffset,
            targetDrunkOffset,
            Time.deltaTime * 5f
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Car"))
        {
            //Debug.Log("a");
            GameManager.instance.GameOver();
        }
    }
}