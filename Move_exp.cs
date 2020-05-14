using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_exp : MonoBehaviour
{
    /// <summary>
    /// これは敵のai管理スクリプトを
    /// </summary>
    public float Speed = 2.0f;
    public float limit = 0.01f;
    public Transform goal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dis = goal.position - this.transform.position;
        Debug.DrawRay(this.transform.position, dis, Color.red);
        if (dis.magnitude > limit)
        {
            this.transform.Translate(dis.normalized * Speed * Time.deltaTime,Space.World);//spaceは前の引数の範囲の事を言う。この場合は「世界から見た自分自身の事」を言う
        }
    }
}
