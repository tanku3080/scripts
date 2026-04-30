using UnityEngine;

/// <summary>
/// 戦闘機・航空機タイプの基本操作を担当するコントローラー
/// ※ PlayerCore の状態を参照して動作する
/// ※ 簡易的な前進・旋回・上昇下降・加減速を想定
/// </summary>
public class AircraftController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private PlayerCore playerCore; // プレイヤー共通状態管理

    [Header("Flight Settings")]

    [SerializeField]
    private float minSpeed = 10f; // 最低速度

    [SerializeField]
    private float maxSpeed = 40f; // 最大速度

    [SerializeField]
    private float acceleration = 15f; // 加速力

    [SerializeField]
    private float deceleration = 10f; // 減速力

    [SerializeField]
    private float pitchSpeed = 60f; // 機首の上下回転速度

    [SerializeField]
    private float yawSpeed = 45f; // 左右旋回速度

    [SerializeField]
    private float rollSpeed = 90f; // ロール回転速度

    private float currentSpeed; // 現在速度

    private void Awake()
    {
        // 未設定なら同じGameObjectから取得
        if (playerCore == null)
            playerCore = GetComponent<PlayerCore>();

        currentSpeed = minSpeed;
    }

    private void Update()
    {
        // PlayerCoreが無い、または操作不能なら処理しない
        if (playerCore == null || !playerCore.CanMove())
            return;

        UpdateSpeed();
        RotateAircraft();
        MoveForward();
        UpdatePlayerState();
    }

    /// <summary>
    /// 加速・減速を行う
    /// Fire3で加速、Fire2で減速
    /// </summary>
    private void UpdateSpeed()
    {
        if (Input.GetButton("Fire3"))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (Input.GetButton("Fire2"))
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }

    /// <summary>
    /// 機体の回転処理
    /// Vertical でピッチ、Horizontal でヨー、Jump でロールを行う
    /// </summary>
    private void RotateAircraft()
    {
        float pitch = -Input.GetAxis("Vertical") * pitchSpeed * Time.deltaTime;
        float yaw = Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;

        float roll = 0f;

        // 簡易ロール操作
        if (Input.GetButton("Jump"))
            roll = -rollSpeed * Time.deltaTime;

        if (Input.GetButton("Fire1"))
            roll = rollSpeed * Time.deltaTime;

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    /// <summary>
    /// 機体の前方へ常に進む
    /// </summary>
    private void MoveForward()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    /// <summary>
    /// PlayerCoreへ現在状態を反映する
    /// </summary>
    private void UpdatePlayerState()
    {
        if (currentSpeed > 0.1f)
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
        return Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
    }
}