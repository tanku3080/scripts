using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// このスクリプトではnavmeshを使わない動きを作る
/// </summary>
public class PlayerCon : MonoBehaviour
{
    public float spd = 10f, rotatSpd = 100f;

    public GameObject bom;

    bool autoPilot = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    /// <summary>
    /// このクラスは対象との距離を求めている
    /// </summary>
    void Fire()
    {
        Vector3 tankPos = this.gameObject.transform.position;
        Vector3 bomPos = bom.transform.position;
        ///<summary>このMathf.Sqrtは平方根Mathf.Powは指数のことを言っている</summary>
        ///これは√(tankPosx - bomPos)^2 + (tankPosy - bomPosy)^2の式でDistanceの役割を担っている
        float dis = Mathf.Sqrt(Mathf.Pow(tankPos.x - bomPos.x,2) + Mathf.Pow(tankPos.y - bomPos.y,2));
        //注意。原因は不明だがunityのDistanceは計算しなくてもいいz座標も計算している可能性がある。
        Debug.Log("Distance" + dis);
    }
    /// <summary>
    /// このクラスは内積を求めている例；視界内
    /// </summary>
    float PosAngle()
    {
        Vector3 tankM = this.transform.up;
        Vector3 tankP = bom.transform.position - this.transform.position;
        ///<summary>これは分子を求めている</summary>
        float si = (tankM.x * tankP.x) + (tankM.y * tankP.y);
        ///<summary>これはMathf.Acosでcosを、分母に当たる部分では二点間の掛け算が行われる</summary>
        float angle = Mathf.Acos(si / (tankM.magnitude * tankP.magnitude));

        Debug.Log("angle" + angle + Mathf.Rad2Deg);//Rad2Deg国際基準の度から親しまれている度に変換する
        Debug.Log("unity angle;" + Vector3.Angle(tankM,tankP)) ;

        //tankの向いている方向
        Debug.DrawRay(this.transform.position, tankM, Color.red, 2);
        //目標物の方向
        Debug.DrawRay(this.transform.position, tankP, Color.blue, 2);

        //原理は分からないが外積(クロス積)を求めて範囲外に出たら対象に一回向く動作をする
        int clockwise = 1;
        if (Cross(tankM,tankP).z < 0)
            clockwise = -1;
        /////<summary>SignedAngleは180～-180までの間の値を返す</summary>
        //float unityAngle = Vector3.SignedAngle(tankM, tankP, this.transform.forward);
        ///<summary>回転させる0.02fの値は回転のスムーズさを調整できる</summary>
        this.transform.Rotate(0, 0, (angle * clockwise * Mathf.Rad2Deg) * 0.02f);
        return (si);

    }

    Vector3 Cross(Vector3 v, Vector3 w)
    {
        float kon = v.y * w.z - v.z * w.y;
        float kin = v.z * w.x - v.x * w.z;
        float kan = v.x * w.y - v.y * w.x;

        Vector3 clossprod = new Vector3(kon, kin, kan);
        return clossprod;

    }

    float autoSpd = 0.1f;
    void AutoPilot()
    {
        PosAngle();
        this.transform.Translate(this.transform.up * autoSpd, Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Vertical") * spd;
        float rot = Input.GetAxis("Horizontal") * rotatSpd;

        if (Input.GetKey(KeyCode.Space))
        {
            Fire();
            PosAngle();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            autoPilot = !autoPilot;//初期設定とは逆にする
        }
        if (autoPilot)
        {
            if(PosAngle() > 5)
            AutoPilot();
        }

    }
}
