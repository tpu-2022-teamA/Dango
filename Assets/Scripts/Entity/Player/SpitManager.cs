using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitManager : MonoBehaviour
{
    [SerializeField] PlayerData player = default!;
    [SerializeField] CapsuleCollider _capsuleCollider = default!;
    DangoUIScript DangoUISC;

    [SerializeField] FloorManager _floorManager;
    [SerializeField] PlayerKusiScript kusiScript;

    //ヒットストップの停止フレームです。左から3d5,4d5...7d5です
    static readonly List<int> hitStopFrameTable = new() { 30, 30, 30, 30, 30 };

    private void Awake()
    {
        DangoUISC = player.GetDangoUIScript();
        _capsuleCollider.enabled = false;
    }

    private bool _isSticking;
    private bool _isInWall;
    private bool _isHitStop;

    /// <summary>
    /// 突き刺しボタンが押されたときにtrueになる。
    /// </summary>
    public bool IsSticking
    {
        get => _isSticking;
        set
        {
            _capsuleCollider.enabled = value;
            if (player.IsGround) _capsuleCollider.radius = 0.1f;
            else _capsuleCollider.radius = 0.5f;

            _isSticking = value;
        }
    }

    public bool IsHitStop => _isHitStop;

    private async void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DangoData>() == null) return;

        //刺せる状態ではないなら実行しない
        if (!IsSticking) return;
        if (_isInWall) return;

        if (LayerMask.LayerToName(other.gameObject.layer) == "Map")
        {
            _isInWall = true;
            return;
        }

        if (player.GetDangos().Count >= player.GetMaxDango())
        {
            if (!player.PlayerFall.IsFallAction) return;

            //急降下中なら団子を弾く
            Logger.Log("ぽよーん");
            other.GetComponent<Rigidbody>().AddForce((other.transform.position - player.transform.position) * 50f, ForceMode.Impulse); ;
            return;
        }

        if (other.gameObject.TryGetComponent(out DangoData dango))
        {
            //SE
            SoundManager.Instance.PlaySE(SoundSource.SE14_STAB_DANGO);

            //刺せ無くする
            IsSticking = false;

            //落下アクション中に行う処理・ヒットストップ前でないと急降下の難易度が上がってしまう
            OnFallAction();

            //ヒットストップ
            await HitStop(dango);

            //団子を刺す
            player.AddDangos(dango.GetDangoColor());

            //フィールドにある団子を消す
            dango.ReleaseDangoPool(player.GetCurrentStabCount());

            //UIの更新
            DangoUISC.DangoUISet(player.GetDangos());
            DangoUISC.AddDango(player.GetDangos());

            //串の団子変更
            kusiScript.SetDango(player.GetDangos());
        }
        else
        {
            SoundManager.Instance.PlaySE(SoundSource.SE13_ATTACK);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isInWall) return;

        if (LayerMask.LayerToName(other.gameObject.layer) == "Map")
        {
            _isInWall = false;
            return;
        }
    }

    private async UniTask HitStop(DangoData dango)
    {
        _isHitStop = true;

        float pSpeed = player.GetAnimator().speed;
        float dSpeed = dango.Animator.speed;

        Vector3 playerVelocity = player.Rb.velocity;

        //アニメーションを一時停止
        player.GetAnimator().speed = 0f;
        dango.Animator.speed = 0f;

        //移動を一時停止
        player.Rb.velocity = Vector3.zero;
        player.Rb.isKinematic = true;
        player.SetIsMoveable(false);
        dango.Rb.velocity = Vector3.zero;
        dango.Rb.isKinematic = true;
        dango.SetIsMoveable(false);

        await UniTask.DelayFrame(hitStopFrameTable[player.GetCurrentStabCount() - 3], PlayerLoopTiming.FixedUpdate);

        //移動やアニメーションを元に戻す
        player.GetAnimator().speed = pSpeed;
        player.Rb.isKinematic = false;
        player.SetIsMoveable(true);
        player.Rb.velocity = playerVelocity;
        dango.Animator.speed = dSpeed;
        dango.Rb.isKinematic = false;

        _isHitStop = false;
    }

    private void OnFallAction()
    {
        if (!player.PlayerFall.IsFallAction) return;

        //落下アクション○回しろ系クエストの判定
        QuestManager.Instance.SucceedChecker.CheckQuestPlayActionSucceed(QuestManager.Instance, Dango.Quest.QuestPlayAction.PlayerAction.FallAttack);

        Logger.Log("落下アクション中に刺した！");
    }
}
