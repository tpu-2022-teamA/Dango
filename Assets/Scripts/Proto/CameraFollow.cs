using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TM.Easing.Management;

public class CameraFollow : MonoBehaviour
{
    enum State
    {
        normal,//プレイヤーの後をラグなくついていきます
        Lerp,//プレイヤーの回転、移動に滑らかについていきます
        LerpMove,//プレイヤーの移動のみに滑らかについていきます
    }

    #region メンバ
    [SerializeField, Tooltip("追従したいターゲット")] Transform target = default!;
    [SerializeField] float MinAngle;
    [SerializeField] float MaxAngle;
    [SerializeField, Tooltip("どのくらいの時間で追いつくか"), Min(0)] float ratio = 1;

    //[SerializeField, Range(0.01f, 1f), Tooltip("カメラの追従度")]
    //private float smoothSpeed = 0.125f;

    [SerializeField] LayerMask wallLayer;//マップのレイヤーマスク

    private Vector3 _prebTargetPos = Vector3.zero;
    private GameObject _terminus = null;

    private PlayerData _playerData;

    private float _roteYSpeed = -100f;


    Vector3 _wallHitPos;//壁にぶつかった際の座標
    RaycastHit _hit;//壁を所得するRay
    float _time = 0f;

    [SerializeField] State state;

#endregion

    private void Start()
    {
        _playerData = target.GetComponent<PlayerData>();
        transform.parent = null;//動くものに乗るとそれに追従しだすから親子関係を無くす
        _terminus = new GameObject("cameraTermiusObject");
        _terminus.transform.position = transform.position;
        _prebTargetPos = target.position;
    }

    private void Update()
    {
        CameraRotate();
    }

    //Playerが動いたあとに実行するため、LateUpdateで行う。
    private void LateUpdate()
    {
        CameraSmoothMove();

        //カメラの位置が決定してからプレイヤーの向きを決めることで、カクつきをなくす。
        _playerData.PlayerRotate(Quaternion.Euler(0, transform.localEulerAngles.y, 0));

        _playerData.SetCameraForward(transform.forward);
    }

    private void CameraSmoothMove()
    {
        Vector3 currentTargetPos = target.position;

        //カメラの目標地点を変更する
        _terminus.transform.position += currentTargetPos - _prebTargetPos;

        if (WallHitCheck())
        {
            transform.position = _wallHitPos;//Lerpｗｐ使うと移動の際に壁の中が見えるため固定
            _time = 0;
        }
        else//カメラの移動
        {
            transform.position = state switch
            {
                State.normal => _terminus.transform.position,
                _ => Vector3.Lerp(transform.position, _terminus.transform.position, Time.deltaTime*ratio),
            };
        }

        _prebTargetPos = target.position;
    }

    private void CameraRotate()
    {
        //X軸回転の角度を所得
        float currentYAngle = transform.eulerAngles.x;

        //X軸が0〜360の値しか返さないので調整
        if (currentYAngle > 180)
        {
            currentYAngle -= 360;
        }

        switch (state)
        {
            case State.normal:
                Rote(_terminus, currentYAngle);
                transform.rotation = _terminus.transform.rotation;
                break;
            case State.Lerp:
                Rote(_terminus, currentYAngle);
                transform.rotation = _terminus.transform.rotation;
                break;
            case State.LerpMove:
                Rote(gameObject, currentYAngle);
                Rote(_terminus, currentYAngle);
                break;
        }

    }

    private void Rote(GameObject obj, float a)
    {
        //カメラをroteAxis.xに合わせて回転させる。
        obj.transform.RotateAround(target.position, Vector3.up, _playerData.GetRoteAxis().x * Time.deltaTime);

        //縦軸の制限
        if ((a >= MinAngle && _playerData.GetRoteAxis().y > 0) || (a <= MaxAngle && _playerData.GetRoteAxis().y < 0))
        {
            obj.transform.RotateAround(target.position, obj.transform.right, _playerData.GetRoteAxis().y * Time.deltaTime * _roteYSpeed);
        }

    }

    private bool WallHitCheck()
    {

        if (Physics.Raycast(_prebTargetPos, _terminus.transform.position - _prebTargetPos, out _hit, Vector3.Distance(_prebTargetPos, _terminus.transform.position), wallLayer, QueryTriggerInteraction.Ignore))
        {
            _wallHitPos = _hit.point * 0.95f;//当たった場所に飛ばすとカメラが壁の中に埋まるので調整。
            //Logger.Log(wallHitPos);
            return true;
        }
        else
        {
            return false;
        }
    }
}

