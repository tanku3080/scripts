using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPDataManager2 : MonoBehaviour
{
    [SerializeField] float m_volume = 1.0f;
    [SerializeField] int m_graphicsQuality = 5;
    [SerializeField] bool m_is60fps = true;
    Configuration m_config;
    string m_key = "Config";

    void Start()
    {
        m_config = new Configuration();
    }

    /// <summary>メモリ上に設定情報を保存する</summary>
    public void SetValues()
    {
        m_config.Volume = m_volume;
        m_config.GraphicsQuality = m_graphicsQuality;
        m_config.Is60Fps = m_is60fps;
    }

    /// <summary>JSON にシリアライズして保存する</summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(m_config, true);
        Debug.Log("シリアライズされた JSON データ: " + json);
        PlayerPrefs.SetString(m_key, json);
    }

    /// <summary>保存したデータを読み出す</summary>
    public void Load()
    {
        string json = PlayerPrefs.GetString(m_key);
        m_config = JsonUtility.FromJson<Configuration>(json);
    }

    /// <summary>設定情報を出力する</summary>
    public void ShowConfiguration()
    {
        string message = "Volume: " + m_config.Volume + "\r\n";
        message += "GraphicsQuality: " + m_config.GraphicsQuality + "\r\n";
        message += "Is60Fps: " + m_config.Is60Fps + "\r\n";
        message += "IsLeftHanded: " + m_config.IsLeftHanded;
        Debug.Log(message);
    }
}
