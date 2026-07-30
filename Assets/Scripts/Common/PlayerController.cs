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
    [SerializeField] private float jumpPower = 3.0f;
    private CharacterController characterController;
    private Vector3 moveVelocity;
    private InputAction move;
    private InputAction jump;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        var input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();

        move = input.currentActionMap.FindAction("move");
        jump = input.currentActionMap.FindAction("jump");
    }
    void Update()
    {
        var moveValue = move.ReadValue<Vector2>();
        moveVelocity.x = moveValue.x * moveSpeed;
        moveVelocity.z = moveValue.y * moveSpeed;

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
}