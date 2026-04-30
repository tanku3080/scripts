using UnityEngine;

/// <summary>
/// 乗り物タイプの共通コントローラー
/// ※ PlayerCore の状態を参照して動作する
/// ※ 戦車・船・車両などのベースとして使う想定
/// </summary>
public class VehicleController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private PlayerCore playerCore; // プレイヤー共通状態管理

    [Header("Movement Settings")]

    [SerializeField]
    private float maxForwardSpeed = 10f; // 前進最大速度

    [SerializeField]
    private float maxReverseSpeed = 4f; // 後退最大速度

    [SerializeField]
    private float acceleration = 6f; // 加速力

    [SerializeField]
    private float deceleration = 5f; // 減速力

    [SerializeField]
    private float turnSpeed = 80f; // 旋回速度

    [SerializeField]
    private bool canTurnInPlace = false; // 停止中に旋回できるか

    private float currentSpeed; // 現在速度

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
        TurnVehicle();
        MoveVehicle();
        UpdatePlayerState();
    }

    /// <summary>
    /// 前後入力から速度を更新する
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

        // 乗り物らしく、速度を徐々に目標値へ近づける
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            changeRate * Time.deltaTime
        );
    }

    /// <summary>
    /// 左右入力で車体を旋回させる
    /// </summary>
    private void TurnVehicle()
    {
        float horizontal = Input.GetAxis("Horizontal");

        // 停止中旋回不可の場合、速度がないと曲がらない
        if (!canTurnInPlace && Mathf.Abs(currentSpeed) < 0.1f)
            return;

        float speedRate = canTurnInPlace
            ? 1f
            : Mathf.InverseLerp(0f, maxForwardSpeed, Mathf.Abs(currentSpeed));

        transform.Rotate(
            Vector3.up,
            horizontal * turnSpeed * speedRate * Time.deltaTime
        );
    }

    /// <summary>
    /// 現在速度に応じて前後へ移動する
    /// </summary>
    private void MoveVehicle()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
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
    /// UIや外部制御で使用する
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

    /// <summary>
    /// 外部から速度を強制的に0にする
    /// 停止処理やイベント時に使用する
    /// </summary>
    public void StopVehicle()
    {
        currentSpeed = 0f;
    }
}