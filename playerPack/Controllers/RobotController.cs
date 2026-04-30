using UnityEngine;

/// <summary>
/// ロボット型プレイヤーの基本操作を担当するコントローラー
/// ※ PlayerCore の状態を参照して動作する
/// ※ 重量感のある移動・旋回・ブーストを想定
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class RobotController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private PlayerCore playerCore; // プレイヤー共通状態管理

    private CharacterController controller;

    [Header("Movement Settings")]

    [SerializeField]
    private float moveSpeed = 3f; // 通常移動速度

    [SerializeField]
    private float rotateSpeed = 120f; // 旋回速度

    [SerializeField]
    private float acceleration = 8f; // 加速の強さ

    [SerializeField]
    private float deceleration = 6f; // 減速の強さ

    [Header("Boost Settings")]

    [SerializeField]
    private float boostSpeed = 7f; // ブースト中の最大速度

    [SerializeField]
    private float boostEnergyMax = 100f; // ブーストエネルギー最大値

    [SerializeField]
    private float boostConsumeRate = 30f; // 1秒あたりの消費量

    [SerializeField]
    private float boostRecoverRate = 20f; // 1秒あたりの回復量

    private float currentBoostEnergy; // 現在のブーストエネルギー
    private float currentSpeed;       // 現在速度
    private Vector3 moveDirection;    // 移動方向

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // 未設定なら同じGameObjectから取得
        if (playerCore == null)
            playerCore = GetComponent<PlayerCore>();

        currentBoostEnergy = boostEnergyMax;
    }

    private void Update()
    {
        // PlayerCoreが無い、または操作不能なら処理しない
        if (playerCore == null || !playerCore.CanMove())
            return;

        Rotate();
        Move();
        RecoverBoostEnergy();
    }

    /// <summary>
    /// 左右入力でロボット本体を旋回させる
    /// </summary>
    private void Rotate()
    {
        float horizontal = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up * horizontal * rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 前後移動とブースト移動を行う
    /// </summary>
    private void Move()
    {
        float vertical = Input.GetAxis("Vertical");

        bool isBoosting =
            Input.GetButton("Fire3") &&
            currentBoostEnergy > 0f &&
            Mathf.Abs(vertical) > 0.1f;

        float targetSpeed = 0f;

        if (Mathf.Abs(vertical) > 0.1f)
        {
            targetSpeed = isBoosting ? boostSpeed : moveSpeed;
            targetSpeed *= Mathf.Sign(vertical);
        }

        // 重量感を出すため、速度を即時変更せず徐々に変化させる
        float speedChangeRate = Mathf.Abs(targetSpeed) > 0f ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            speedChangeRate * Time.deltaTime
        );

        moveDirection = transform.forward * currentSpeed;

        controller.Move(moveDirection * Time.deltaTime);

        // ブースト中ならエネルギー消費
        if (isBoosting)
            currentBoostEnergy -= boostConsumeRate * Time.deltaTime;

        currentBoostEnergy = Mathf.Clamp(currentBoostEnergy, 0f, boostEnergyMax);

        // 状態更新
        if (Mathf.Abs(currentSpeed) > 0.1f)
            playerCore.SetState(PlayerCore.PlayerState.Moving);
        else
            playerCore.SetState(PlayerCore.PlayerState.Idle);
    }

    /// <summary>
    /// ブーストしていない間、エネルギーを回復する
    /// </summary>
    private void RecoverBoostEnergy()
    {
        bool isBoosting = Input.GetButton("Fire3") && currentBoostEnergy > 0f;

        if (isBoosting)
            return;

        currentBoostEnergy += boostRecoverRate * Time.deltaTime;
        currentBoostEnergy = Mathf.Clamp(currentBoostEnergy, 0f, boostEnergyMax);
    }

    /// <summary>
    /// 現在のブースト残量を0〜1で取得する
    /// UI表示などで使用する
    /// </summary>
    public float GetBoostEnergyRate()
    {
        return currentBoostEnergy / boostEnergyMax;
    }
}