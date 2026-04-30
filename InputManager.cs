using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// プレイヤーが現在使用している操作を判定する
/// </summary>
public class InputManager : MonoBehaviour
{
    public enum DeviceType
    {
        Unknown,
        KeyboardMouse,
        Gamepad
    }

    /// <summary>
    /// 現在の入力がコントローラーかキーボードかを判定する
    /// 使用条件：InputSystemをpackage managerからインストールする
    /// </summary>
    public DeviceType CurrentDevice { get; private set; } = DeviceType.Unknown;

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // 状態変化イベント以外は無視
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        // 実際に値が変わったControlだけ見る
        foreach (var control in eventPtr.EnumerateChangedControls(device))
        {
            // ノイズっぽい入力は無視
            if (control.noisy)
                continue;

            if (device is Keyboard || device is Mouse)
            {
                SetDevice(DeviceType.KeyboardMouse);
                return;
            }

            if (device is Gamepad)
            {
                SetDevice(DeviceType.Gamepad);
                return;
            }
        }
    }

    private void SetDevice(DeviceType device)
    {
        if (CurrentDevice == device)
            return;

        CurrentDevice = device;
        Debug.Log($"現在の入力デバイス: {CurrentDevice}");
    }
}
