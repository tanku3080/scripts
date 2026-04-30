using System;
using UnityEngine;

/// <summary>
/// プレイヤーの共通的な状態・制御フラグを管理するコアコンポーネント
/// ※移動や入力処理は持たない（各Controllerに任せる）
/// </summary>
public class PlayerCore : MonoBehaviour
{
    /// <summary>
    /// プレイヤーの状態定義
    /// </summary>
    public enum PlayerState
    {
        Idle,       // 待機状態
        Moving,     // 移動中
        Action,     // 行動中（攻撃・操作など）
        Stunned,    // スタン（行動不可）
        Dead,       // 死亡
        Disabled    // 完全停止（強制的に操作不能）
    }

    [Header("Core Settings")]

    [SerializeField]
    private bool canControl = true; // 操作可能かどうか（外部制御用）

    [SerializeField]
    private bool isAlive = true;    // 生存状態

    /// <summary>
    /// 現在のプレイヤー状態
    /// </summary>
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    /// <summary>
    /// 実際に操作可能かどうか
    /// （複数条件をまとめた最終判定）
    /// </summary>
    public bool CanControl => canControl && isAlive && CurrentState != PlayerState.Disabled;

    /// <summary>
    /// 生存状態取得
    /// </summary>
    public bool IsAlive => isAlive;

    // =========================
    // イベント
    // =========================

    /// <summary>
    /// 状態変更時に呼ばれる
    /// </summary>
    public event Action<PlayerState> OnStateChanged;

    /// <summary>
    /// 死亡時に呼ばれる
    /// </summary>
    public event Action OnPlayerDead;

    /// <summary>
    /// 操作可能になった時
    /// </summary>
    public event Action OnControlEnabled;

    /// <summary>
    /// 操作不能になった時
    /// </summary>
    public event Action OnControlDisabled;

    // =========================
    // 状態制御
    // =========================

    /// <summary>
    /// プレイヤー状態を変更する
    /// </summary>
    public void SetState(PlayerState newState)
    {
        // 同じ状態なら何もしない
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        // 状態変更イベント通知
        OnStateChanged?.Invoke(CurrentState);
    }

    /// <summary>
    /// 操作を有効化する
    /// </summary>
    public void EnableControl()
    {
        if (canControl)
            return;

        canControl = true;
        OnControlEnabled?.Invoke();
    }

    /// <summary>
    /// 操作を無効化する
    /// </summary>
    public void DisableControl()
    {
        if (!canControl)
            return;

        canControl = false;
        OnControlDisabled?.Invoke();
    }

    /// <summary>
    /// プレイヤーを死亡状態にする
    /// </summary>
    public void Kill()
    {
        if (!isAlive)
            return;

        isAlive = false;

        // 状態をDeadに変更
        SetState(PlayerState.Dead);

        // 死亡イベント通知
        OnPlayerDead?.Invoke();
    }

    /// <summary>
    /// プレイヤーを復活させる
    /// </summary>
    public void Revive()
    {
        if (isAlive)
            return;

        isAlive = true;

        // 初期状態に戻す
        SetState(PlayerState.Idle);
    }

    // =========================
    // 判定系（Controller用）
    // =========================

    /// <summary>
    /// 移動可能かどうか
    /// Controller側で使用する
    /// </summary>
    public bool CanMove()
    {
        return CanControl &&
               CurrentState != PlayerState.Stunned &&
               CurrentState != PlayerState.Dead;
    }

    /// <summary>
    /// 行動可能かどうか（攻撃など）
    /// </summary>
    public bool CanAction()
    {
        return CanControl &&
               CurrentState != PlayerState.Stunned &&
               CurrentState != PlayerState.Dead;
    }
}