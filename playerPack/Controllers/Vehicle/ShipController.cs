using UnityEngine;

/// <summary>
/// 船タイプの基本操作を担当するコントローラー
/// ※ PlayerCore の状態を参照して動作する
/// ※ 水上での前進・後退・旋回・慣性移動を想定
/// </summary>
public class ShipController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private PlayerCore playerCore; // プレイヤー共通状態管理

    [Header("Movement Settings")]

    [SerializeField]
    private float maxForwardSpeed = 12f; // 前進最大速度

    [SerializeField]
    private float maxReverseSpeed = 4f; // 後退最大速度

    [SerializeField]
    private float acceleration = 4f; // 加速力

    [SerializeField]
    private float deceleration = 2f; // 減速力

    [SerializeField]
    private float turnSpeed = 45f; // 旋回速度

    [SerializeField]
    private float driftDamping = 1.5f; // 横滑り減衰

    private float currentSpeed; // 現在の前後速度
    private Vector3 velocity; // 実際の移動速度

    private void Awake()
    {
        // 未設定なら同じGameObjectから取得
        if (playerCore == null)
            playerCore = GetComponent<PlayerCore>();
    }

    private void Update()
    {
        // PlayerCoreが無い、または操作不能なら処理しない
        if (playerCore == null || !playerCore.CanMove())
            return;

        UpdateSpeed();
        TurnShip();
        MoveShip();
        UpdatePlayerState();
    }

    /// <summary>
    /// 前後入力から船の速度を更新する
    /// </summary>
    private void UpdateSpeed()
    {
        float input = Input.GetAxis("Vertical");

        float targetSpeed = 0f;

        if (input > 0.1f)
        {
            targetSpeed = input * maxForwardSpeed;
        }
        else if (input < -0.1f)
        {
            targetSpeed = input * maxReverseSpeed;
        }

        float changeRate = Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed)
            ? acceleration
            : deceleration;

        // 船らしく速度を急に変えず、ゆっくり目標速度へ近づける
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            changeRate * Time.deltaTime
        );
    }

    /// <summary>
    /// 左右入力で船を旋回させる
    /// 速度が出ているほど旋回しやすくする
    /// </summary>
    private void TurnShip()
    {
        float horizontal = Input.GetAxis("Horizontal");

        float speedRate = Mathf.InverseLerp(0f, maxForwardSpeed, Mathf.Abs(currentSpeed));

        transform.Rotate(
            Vector3.up,
            horizontal * turnSpeed * speedRate * Time.deltaTime
        );
    }

    /// <summary>
    /// 船を移動させる
    /// 前後方向は速度を反映し、横方向の滑りは徐々に減衰させる
    /// </summary>
    private void MoveShip()
    {
        Vector3 forwardVelocity = transform.forward * currentSpeed;

        // 横滑り成分を少しずつ減らす
        Vector3 sideVelocity = Vector3.Project(velocity, transform.right);
        sideVelocity = Vector3.Lerp(
            sideVelocity,
            Vector3.zero,
            driftDamping * Time.deltaTime
        );

        velocity = forwardVelocity + sideVelocity;

        transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// PlayerCoreへ現在状態を反映する
    /// </summary>
    private void UpdatePlayerState()
    {
        if (Mathf.Abs(currentSpeed) > 0.1f)
            playerCore.SetState(PlayerCore.PlayerState.Moving);
        else
            playerCore.SetState(PlayerCore.PlayerState.Idle);
    }

    /// <summary>
    /// 現在速度を取得する
    /// UI表示などで使用する
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    /// <summary>
    /// 現在速度を0〜1の割合で取得する
    /// UIゲージなどで使用する
    /// </summary>
    public float GetSpeedRate()
    {
        return Mathf.InverseLerp(0f, maxForwardSpeed, Mathf.Abs(currentSpeed));
    }
}