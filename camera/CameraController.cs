using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ制御クラス
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform targetTrans = null;       //< ターゲットのTransform

    [SerializeField]
    float distanceToTarget = 5.0f;      //< ターゲットからの距離

    [SerializeField]
    float heightToTarget = 3.0f;        //< ターゲットからの高さ

    [SerializeField]
    Vector3 lookAtOffset = new Vector3(0.0f, 2.0f, 0.0f);       //< みる位置のオフセット

    [SerializeField]
    float rotateSpeed = 5.0f;       //< 回転速度

    RaycastHit raycastHit = new RaycastHit();

    // Raycastでヒットして欲しいレイヤーマスク
    int raycastHitLayerMask = 0;

    private void Start()
    {
        raycastHitLayerMask = LayerMask.GetMask("Map");

        transform.localPosition = targetTrans.localPosition - (targetTrans.forward * distanceToTarget);
        transform.localPosition += Vector3.up * heightToTarget;
        transform.LookAt(targetTrans.localPosition + lookAtOffset);
    }

    /// <summary>
    /// ターゲットが動いた後に処理したいので、LateUpdateでやっている
    /// </summary>
    private void LateUpdate()
    {
        Vector3 lookTargetPos = targetTrans.localPosition + lookAtOffset;

        // 移動処理
        {
            // ターゲット座標 - (カメラの前方向 * 距離)
            Vector3 cameraPos = lookTargetPos - (transform.forward * distanceToTarget);

            // 壁や床にめり込ませないようにする処理
            {
                Vector3 targetDir = (transform.localPosition - lookTargetPos).normalized;
                float targetDist = distanceToTarget + 0.5f; //< 少し奥までRayを飛ばす

                // デバッグ表示（シーンビューで確認できる）
                Debug.DrawRay(lookTargetPos, targetDir * targetDist, Color.red);

                // Raycast
                bool isHit = Physics.Raycast(lookTargetPos, targetDir, out raycastHit, targetDist, raycastHitLayerMask);
                if (isHit)
                {
                    // 当たった座標にカメラ座標を上書きする。
                    cameraPos = raycastHit.point;
                }
            }

            transform.localPosition = cameraPos;
        }

        // プレイヤーの周りを回転する処理
        {
            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                dir.x = 1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                dir.x = -1;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                dir.y = -1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                dir.y = 1;
            }

            float rotX = dir.x * Time.deltaTime * rotateSpeed;
            float rotY = dir.y * Time.deltaTime * rotateSpeed;

            // 回転（横）
            transform.RotateAround(lookTargetPos, Vector3.up, rotX);

            // カメラがプレイヤーの真上や真下にあるときにそれ以上回転させないようにする
            if (transform.forward.y > 0.9f && rotY < 0)
            {
                rotY = 0;
            }
            if (transform.forward.y < -0.9f && rotY > 0)
            {
                rotY = 0;
            }
            // 回転（縦）
            transform.RotateAround(lookTargetPos, transform.right, rotY);
        }

        // LookAt
        transform.LookAt(lookTargetPos);
    }
}
