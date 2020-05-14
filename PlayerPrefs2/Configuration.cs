using System.Collections;
using System.Collections.Generic;
using System;   // Serializable を指定するために必要

/// <summary>
/// アプリの設定情報を格納するクラス
/// </summary>
[Serializable]  // JsonUtility を使うために必要
public class Configuration  // シリアライズするクラスは MonoBehaviour を継承してはいけない
{
    public float Volume;
    public int GraphicsQuality;
    public bool Is60Fps;
    public bool IsLeftHanded;
}
