using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPDataManager : MonoBehaviour
{
    [SerializeField] float m_floatValue = Mathf.PI;
    [SerializeField] string m_key = "Data";

    /// <summary>PlayerPrefs にデータを保存する</summary>
    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat(m_key, m_floatValue);
    }

    /// <summary>PlayerPrefs からデータを読み出す</summary>
    public void LoadFromPlayerPrefs()
    {
        float v = PlayerPrefs.GetFloat(m_key);
        string message = m_key + " からデータ " + v.ToString() + " を読み込みました。";
        Debug.Log(message);
    }
}
