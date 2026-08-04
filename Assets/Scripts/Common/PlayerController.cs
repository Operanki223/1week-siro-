//宣言や設定
using UnityEngine;
using UnityEngine.InputSystem;  //InputSystemを使うための宣言

//必要なコンポーネントの設定
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    //変数宣言
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 3.0f;
    [SerializeField] private float jumpPower = 3.0f;
    [SerializeField] GameObject camera_player;
    [SerializeField] private float drunkStrength = 1.5f;   // 横に流される強さ
    [SerializeField] private float drunkInterval = 0.5f;   // 何秒ごとに方向を変える
    private CharacterController characterController;
    private Vector3 moveVelocity;
    private InputAction move;
    private InputAction jump;
    private InputAction turn;
    private float cameraAngle = 0;
    private Vector3 drunkOffset = Vector3.zero;
    private float drunkTimer = 0f;
    private Vector3 targetDrunkOffset;
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

        ControllePlayer();

    }

    void ControllePlayer()
    {
        if (!GameManager.instance.playGame)
        {
            moveVelocity = Vector3.zero;
            return;
        }

        var moveValue = move.ReadValue<Vector2>();

        Vector3 moveDirection = transform.right * moveValue.x + transform.forward * moveValue.y;

        DrunkMove();

        // ランダム移動を加算(酔いの再現)
        moveDirection += drunkOffset;

        moveVelocity.x = moveDirection.x * moveSpeed;
        moveVelocity.z = moveDirection.z * moveSpeed;

        var turnValue = turn.ReadValue<Vector2>();
        this.gameObject.transform.Rotate(0, turnValue.x * turnSpeed * Time.deltaTime, 0);

        cameraAngle -= turnValue.y * turnSpeed * Time.deltaTime;
        cameraAngle = Mathf.Clamp(cameraAngle, -30f, 45f);

        camera_player.transform.localRotation =
            Quaternion.Euler(cameraAngle, 0, 0);

        if (characterController.isGrounded)
        {
            if (jump.WasPressedThisFrame())
            {
                moveVelocity.y = jumpPower;
            }
        }
        else
        {
            moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(moveVelocity * Time.deltaTime);
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

            targetDrunkOffset = dirs[Random.Range(0, dirs.Length)] * drunkStrength;
        }

        // 徐々に現在の方向を変える
        drunkOffset = Vector3.Lerp(drunkOffset, targetDrunkOffset, Time.deltaTime * 5f);
    }
}