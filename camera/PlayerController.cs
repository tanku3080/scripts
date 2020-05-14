using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Transform cameraTrans = null;

    [SerializeField]
    float moveSpeed = 100.0f;

    [SerializeField]
    float rotationSpeed = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1.0f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveDir.z = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir.z = -1.0f;
        }


        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(cameraTrans.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * moveDir.z + cameraTrans.right * moveDir.x;

        transform.localPosition += moveForward * Time.deltaTime * moveSpeed;

        if (moveForward.sqrMagnitude > 0.0)
        {
            // 方向を移動方向に合わせる
            transform.rotation = Quaternion.LookRotation(moveForward);
        }
    }
}
