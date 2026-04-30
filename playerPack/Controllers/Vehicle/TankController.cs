using UnityEngine;

/// <summary>
/// 戦車タイプの基本操作を担当するコントローラー
/// ※ PlayerCore の状態を参照して動作する
/// ※ 車体移動・車体旋回・砲塔旋回・砲身上下を想定
/// </summary>
public class TankController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private PlayerCore playerCore; // プレイヤー共通状態管理

    [SerializeField]
    private Transform turret; // 砲塔オブジェクト

    [SerializeField]
    private Transform barrel; // 砲身オブジェクト

    [Header("Movement Settings")]

    [SerializeField]
    private float moveSpeed = 5f; // 前進速度

    [SerializeField]
    private float reverseSpeed = 3f; // 後退速度

    [SerializeField]
    private float turnSpeed = 80f; // 車体旋回速度

    [SerializeField]
    private float acceleration = 6f; // 加速力

    [SerializeField]
    private float deceleration = 5f; // 減速力

    [Header("Turret Settings")]

    [SerializeField]
    private float turretRotateSpeed = 90f; // 砲塔旋回速度

    [SerializeField]
    private float barrelPitchSpeed = 45f; // 砲身上下速度

    [SerializeField]
    private float minBarrelAngle = -5f; // 砲身の最低角度

    [SerializeField]
    private float maxBarrelAngle = 25f; // 砲身の最高角度

    private float currentSpeed; // 現在速度
    private float currentBarrelAngle; // 現在の砲身角度

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

        MoveTank();
        RotateTurret();
        PitchBarrel();
        UpdatePlayerState();
    }

    /// <summary>
    /// 戦車本体の前後移動と旋回を行う
    /// </summary>
    private void MoveTank()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        float targetSpeed = 0f;

        if (vertical > 0.1f)
        {
            targetSpeed = vertical * moveSpeed;
        }
        else if (vertical < -0.1f)
        {
            targetSpeed = vertical * reverseSpeed;
        }

        float changeRate = Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed)
            ? acceleration
            : deceleration;

        // 戦車らしく急に速度を変えず、徐々に加減速する
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            changeRate * Time.deltaTime
        );

        // 前後移動
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        // 旋回
        // 停止中でもその場旋回できる想定
        transform.Rotate(
            Vector3.up,
            horizontal * turnSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// 砲塔を左右に旋回させる
    /// Fire1 / Fire2 を仮の左右旋回入力として使用
    /// </summary>
    private void RotateTurret()
    {
        if (turret == null)
            return;

        float input = 0f;

        if (Input.GetButton("Fire1"))
            input = -1f;

        if (Input.GetButton("Fire2"))
            input = 1f;

        turret.Rotate(
            Vector3.up,
            input * turretRotateSpeed * Time.deltaTime,
            Space.Self
        );
    }

    /// <summary>
    /// 砲身を上下に動かす
    /// Fire3 で上げる、Jump で下げる
    /// </summary>
    private void PitchBarrel()
    {
        if (barrel == null)
            return;

        float input = 0f;

        if (Input.GetButton("Fire3"))
            input = 1f;

        if (Input.GetButton("Jump"))
            input = -1f;

        currentBarrelAngle += input * barrelPitchSpeed * Time.deltaTime;
        currentBarrelAngle = Mathf.Clamp(
            currentBarrelAngle,
            minBarrelAngle,
            maxBarrelAngle
        );

        barrel.localRotation = Quaternion.Euler(
            currentBarrelAngle,
            0f,
            0f
        );
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
        return Mathf.InverseLerp(0f, moveSpeed, Mathf.Abs(currentSpeed));
    }
}