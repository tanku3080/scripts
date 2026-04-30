using UnityEngine;

/// <summary>
/// 人間型プレイヤーの基本操作（移動・ジャンプ）を担当するコントローラー
/// ※ PlayerCore の状態を参照して動作する
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class HumanController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private PlayerCore playerCore; // プレイヤーの状態管理

    private CharacterController controller;

    [Header("Movement Settings")]

    [SerializeField]
    private float moveSpeed = 5f; // 移動速度

    [SerializeField]
    private float jumpForce = 5f; // ジャンプ力

    [SerializeField]
    private float gravity = -9.81f; // 重力

    private Vector3 velocity; // Y方向の速度管理用

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // 未設定なら自動取得
        if (playerCore == null)
            playerCore = GetComponent<PlayerCore>();
    }

    private void Update()
    {
        // 操作不能なら何もしない
        if (playerCore == null || !playerCore.CanMove())
            return;

        Move();
        ApplyGravity();
    }

    /// <summary>
    /// WASDでの移動処理
    /// </summary>
    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // カメラ基準にしたい場合はここを調整可能
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // 移動状態更新
        if (move.magnitude > 0.1f)
            playerCore.SetState(PlayerCore.PlayerState.Moving);
        else
            playerCore.SetState(PlayerCore.PlayerState.Idle);

        // ジャンプ処理
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 地面に張り付ける
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded && playerCore.CanAction())
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    /// <summary>
    /// 重力適用
    /// </summary>
    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}