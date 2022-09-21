using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TM.Entity.Player;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

/// 
/// ＜実装すること＞
/// ・接地判定をまともに
/// ・移動の摩擦を加える。壁にあたったときに摩擦が存在するとその場にとどまってしまうから自力実装
/// 

[RequireComponent(typeof(Rigidbody), typeof(RideMoveObj))]
class PlayerData : MonoBehaviour
{
    #region inputSystem
    //スコアと満腹度のレート
    const float SCORE_TIME_RATE = 0.2f;

    private bool _hasAttacked = false;
    private bool _hasFalled = false;
    private bool _hasStayedEat = false;

    [SerializeField] private Rigidbody rb = default!;
    [SerializeField] private CapsuleCollider capsuleCollider = default!;
    [SerializeField] private GameObject playerCamera = default!;
    private Camera _cameraComponent = default!;

    readonly DangoRole dangoRole = DangoRole.instance;

    readonly PlayerAttackAction _playerAttack = new();

    //生成はAwakeで行っています。
    PlayerMove _playerMove;
    PlayerJump _playerJump;
    PlayerRemoveDango _playerRemoveDango;
    PlayerFallAction _playerFall;


    public Rigidbody Rb => rb;
    public PlayerFallAction PlayerFall => _playerFall;

    const float EVENTTEXT_FLASH_TIME = 0.4f;
    const float EVENTTEXT_PRINT_TIME = 2.4f;

    //突き刺しボタン降下時処理
    private async void OnAttack()
    {
        //落下アクション中受け付けない。
        if (_playerFall.IsFallAction) return;

        //地上なら確実に通常攻撃
        if (_isGround)
        {
            _hasAttacked = true;
            return;
        }

        //Yの加速度が負になるまで待機
        await WaitVelocityYLessZero();

        //急降下するか判定、しないなら通常刺しに移行
        _hasFalled = _playerFall.ToFallAction(transform.position, _isGround);
        _hasAttacked = !_hasFalled;
    }

    private async UniTask WaitVelocityYLessZero()
    {
        while (rb.velocity.y > 0)
        {
            await UniTask.Yield();
            if (!InputSystemManager.Instance.IsPressAttack) break;
        }
    }

    //食べる
    //将来的にここから移動させたい
    private void OnEatDango()
    {
        //串に刺さってなかったら実行しない。
        if (_dangos.Count == 0) return;

        //何らかの動作中で食べようとしても実行しない。
        if (_currentState is not (IState.E_State.Control or IState.E_State.StayEatDango))
        {
            _playerUIManager.EventText.TextData.SetText("食べられないよ！");
            _playerUIManager.EventText.TextData.FlashAlpha(EVENTTEXT_PRINT_TIME, EVENTTEXT_FLASH_TIME, 0);
            return;
        }

        //限界まで団子が刺さっていなかったら実行しない。
        if (_dangos.Count != _currentStabCount)
        {
            _playerUIManager.EventText.TextData.SetText("食べられないよ！");
            _playerUIManager.EventText.TextData.FlashAlpha(EVENTTEXT_PRINT_TIME, EVENTTEXT_FLASH_TIME, 0);
            return;
        }

        SoundManager.Instance.PlaySE(UnityEngine.Random.Range((int)SoundSource.VOISE_PRINCE_STAYEAT01, (int)SoundSource.VOISE_PRINCE_STAYEAT02 + 1));
        _hasStayedEat = true;
    }

    //将来的にここから移動させたいというかここでやる意味がない
    private void OnChangeToUIAction()
    {
        //InputSystemManager.Instance.Input.SwitchCurrentActionMap("UI");
        //_optionCanvas.enabled = true;
    }

    private void EatDango()
    {
        //SE
        //GameManager.SoundManager.PlaySE(SoundSource.SE_PLAYER_EATDANGO);

        _hasStayedEat = false;

        //食べた団子の点数を取得
        var score = dangoRole.CheckRole(_dangos);

        //演出関数の呼び出し
        _directing.Dirrecting(_dangos);

        _playerUIManager.EventText.TextData.SetText("食べた！" + (int)score + "点！");

        //残り時間の増加
        PlayerUIManager.time += score;

        //満腹度を上昇
        _satiety += score * SCORE_TIME_RATE;

        //スコアを上昇
        GameManager.GameScore += score * 100f;

        //串をクリア。
        _dangos.Clear();
        //UI更新
        _dangoUISC.DangoUISet(_dangos);
    }

    private void ResetSpit()
    {
        spitManager.isSticking = false;
        spitManager.gameObject.transform.localRotation = Quaternion.identity;
        spitManager.gameObject.transform.localPosition = new Vector3(0, 0.4f, 1.1f);
    }
    #endregion

    #region statePattern
    interface IState
    {
        public enum E_State
        {
            Control = 0,
            FallAction = 1,
            AttackAction = 2,
            StayEatDango = 3,
            EatDango = 4,
            GrowStab = 5,

            Max,

            Unchanged,
        }

        E_State Initialize(PlayerData parent);
        E_State Update(PlayerData parent);
        E_State FixedUpdate(PlayerData parent);
    }
    //状態管理
    private IState.E_State _currentState = IState.E_State.Control;
    private static readonly IState[] states = new IState[(int)IState.E_State.Max]
     {
        new ControlState(),
        new FallActionState(),
        new AttackActionState(),
        new StayEatDangoState(),
        new EatDangoState(),
        new GrowStabState(),
     };

    class ControlState : IState
    {
        public IState.E_State Initialize(PlayerData parent)
        {
            //串の位置をリセット
            parent.ResetSpit();
            return IState.E_State.Unchanged;
        }
        public IState.E_State Update(PlayerData parent)
        {
            return IState.E_State.Unchanged;
        }
        public IState.E_State FixedUpdate(PlayerData parent)
        {
            parent._animator.SetFloat("VelocityY", parent.rb.velocity.y);

            if (parent._playerJump.IsJumping) return IState.E_State.Unchanged;

            //プレイヤーを動かす処理
            parent._playerMove.Update(parent.rb, parent.playerCamera.transform);

            //満腹度（制限時間）減らす処理
            parent.DecreaseSatiety();

            //ジャンプ
            parent._playerJump.SetIsGround(parent._isGround);
            parent._playerJump.SetMaxStabCount(parent._currentStabCount);

            //ステートに移行。
            if (parent._hasStayedEat) return IState.E_State.StayEatDango;
            if (parent._hasAttacked) return IState.E_State.AttackAction;
            if (parent._hasFalled) return IState.E_State.FallAction;

            return IState.E_State.Unchanged;
        }
    }
    class FallActionState : IState
    {
        public IState.E_State Initialize(PlayerData parent)
        {
            parent._hasFalled = false;
            parent._playerFall.IsFallAction = true;
            return IState.E_State.Unchanged;
        }
        public IState.E_State Update(PlayerData parent)
        {
            return IState.E_State.Unchanged;
        }
        public IState.E_State FixedUpdate(PlayerData parent)
        {
            //移動
            parent._playerMove.Update(parent.rb, parent.playerCamera.transform);

            //制限時間減少
            parent.DecreaseSatiety();

            //途中で接地したらコントロールに戻る
            if (parent.IsGround) return IState.E_State.Control;

            //待機時間が終わったらアタックステートに移行
            return parent._playerFall.FixedUpdate(parent.rb, parent.spitManager)
                ? IState.E_State.AttackAction : IState.E_State.Unchanged;
        }
    }
    class AttackActionState : IState
    {
        public IState.E_State Initialize(PlayerData parent)
        {
            parent._hasAttacked = false;
            SoundManager.Instance.PlaySE(UnityEngine.Random.Range((int)SoundSource.VOISE_PRINCE_ATTACK01, (int)SoundSource.VOISE_PRINCE_ATTACK02 + 1));

            if (parent._playerFall.IsFallAction) return IState.E_State.Unchanged;
            if (!parent.CanStab()) return IState.E_State.Control;
            return IState.E_State.Unchanged;
        }
        public IState.E_State Update(PlayerData parent)
        {
            return IState.E_State.Unchanged;
        }
        public IState.E_State FixedUpdate(PlayerData parent)
        {
            parent.DecreaseSatiety();

            return parent._playerAttack.FixedUpdate() ? IState.E_State.Control : IState.E_State.Unchanged;
        }

    }
    class StayEatDangoState : IState
    {
        public IState.E_State Initialize(PlayerData parent)
        {
            parent._playerUIManager.EventText.TextData.SetText("食べチャージ中！");
            parent._playerStayEat.ResetCount();
            //SE推奨

            return IState.E_State.Unchanged;
        }
        public IState.E_State Update(PlayerData parent)
        {
            parent.OnChargeCameraMoving();

            return IState.E_State.Unchanged;
        }
        public IState.E_State FixedUpdate(PlayerData parent)
        {
            parent.DecreaseSatiety();

            //食べる待機が終わったら食べるステートに移行
            if (parent._playerStayEat.CanEat())
            {
                parent.StartCoroutine(parent.ResetCameraView());
                return IState.E_State.EatDango;
            }
            //待機をやめたらコントロールに戻る
            if (!parent._hasStayedEat)
            {
                parent.StartCoroutine(parent.ResetCameraView());
                return IState.E_State.Control;
            }
            return IState.E_State.Unchanged;
        }
    }
    class EatDangoState : IState
    {
        public IState.E_State Initialize(PlayerData parent)
        {
            if (parent._canGrowStab) return IState.E_State.GrowStab;

            parent.EatDango();
            return IState.E_State.Control;
        }
        public IState.E_State Update(PlayerData parent)
        {
            return IState.E_State.Unchanged;
        }
        public IState.E_State FixedUpdate(PlayerData parent)
        {
            return IState.E_State.Unchanged;
        }

    }
    class GrowStabState : IState
    {
        public IState.E_State Initialize(PlayerData parent)
        {
            parent._currentStabCount = parent._playerGrowStab.GrowStab(parent._currentStabCount);
            parent._playerUIManager.EventText.TextData.SetText("させる団子の数が増えた！(" + parent._currentStabCount + "個)");
            parent._canGrowStab = false;
            //刺せる範囲表示の拡大。今串が伸びないのでコメントアウトしてます。
            //parent.rangeUI.transform.localScale = new Vector3(parent.rangeUI.transform.localScale.x, parent.rangeUI.transform.localScale.y + 0.01f, parent.rangeUI.transform.localScale.z);
            return IState.E_State.EatDango;
        }
        public IState.E_State Update(PlayerData parent)
        {
            return IState.E_State.Unchanged;
        }
        public IState.E_State FixedUpdate(PlayerData parent)
        {
            return IState.E_State.Unchanged;
        }

    }

    private void InitState()
    {
        Logger.Assert(_currentState is >= 0 and < IState.E_State.Max);

        var nextState = states[(int)_currentState].Initialize(this);

        if (nextState != IState.E_State.Unchanged)
        {
            _currentState = nextState;
            InitState();//初期化で状態が変わるなら再帰的に初期化する。
        }
    }
    private void UpdateState()
    {
        Logger.Assert(_currentState is >= 0 and < IState.E_State.Max);

        var nextState = states[(int)_currentState].Update(this);

        if (nextState != IState.E_State.Unchanged)
        {
            //次に遷移
            _currentState = nextState;
            InitState();
        }
    }
    private void FixedUpdateState()
    {
        Logger.Assert(_currentState is >= 0 and < IState.E_State.Max);

        var nextState = states[(int)_currentState].FixedUpdate(this);

        if (nextState != IState.E_State.Unchanged)
        {
            //次に遷移
            _currentState = nextState;
            InitState();
        }
    }
    #endregion

    #region メンバ変数
    //プレイヤーの能力
    [SerializeField] private SpitManager spitManager = default!;
    [SerializeField] private GameObject makerUI = default!;
    [SerializeField] private GameObject rangeUI = default!;

    //UI関連
    [SerializeField] RoleDirectingScript _directing;
    [SerializeField] PlayerUIManager _playerUIManager;
    [SerializeField] DangoUIScript _dangoUISC = default!;

    [SerializeField] Animator _animator = default!;

    //串を伸ばす処理
    const int MAX_DANGO = 7;
    const int GROW_STAB_FRAME = 500;
    readonly PlayerGrowStab _playerGrowStab = new(MAX_DANGO, GROW_STAB_FRAME);
    bool _canGrowStab = false;

    //食べる処理
    const int STAY_FRAME = 100;
    readonly PlayerStayEat _playerStayEat = new(STAY_FRAME);

    const float DEFAULT_CAMERA_VIEW = 60f;
    const float CAMERA_REMOVETIME = 0.3f;

    /// <summary>
    /// 満腹度、制限時間の代わり（単位:[sec]）
    /// </summary>
    /// フレーム数で管理しますが、ここでは秒管理で構いません。
    private float _satiety = 100f;

    /// <summary>
    /// 串、持ってる団子
    /// </summary>
    /// 今まではnew List<DangoColor>()としなければなりませんでしたが
    /// C#9.0以降はこのように簡素化出来るそうです。
    readonly List<DangoColor> _dangos = new();

    /// <summary>
    /// 刺せる数、徐々に増える
    /// </summary>    
    private int _currentStabCount = 3;

    private bool _isGround = false;

    public bool IsGround
    {
        get => _isGround;
        private set
        {
            if (value)
            {
                if (_playerFall.IsFallAction)
                {
                    //AN7Bの再生もここ
                    SoundManager.Instance.PlaySE(SoundSource.SE11_FALLACTION_LANDING);

                    _playerFall.IsFallAction = false;
                }
            }

            _isGround = value;
        }
    }


    public Vector3 MoveVec { get; private set; }
    #endregion

    private void Awake()
    {
        _playerFall = new(capsuleCollider, OnJump, OnJumpExit);
        _playerRemoveDango = new(_dangos, _dangoUISC);
        _playerMove = new(_animator);
        _playerJump = new(rb, OnJump, OnJumpExit, _animator);
    }

    private void OnEnable()
    {
        InitDangos();
    }

    private void Start()
    {
        InputSystemManager.Instance.onFirePerformed += _playerRemoveDango.Remove;
        InputSystemManager.Instance.onAttackPerformed += OnAttack;
        InputSystemManager.Instance.onEatDangoPerformed += OnEatDango;
        InputSystemManager.Instance.onEatDangoCanceled += () => _hasStayedEat = false;
        InputSystemManager.Instance.onPausePerformed += OnChangeToUIAction;
        InputSystemManager.Instance.onJumpPerformed += _playerJump.Jump;
        _cameraComponent = playerCamera.GetComponent<Camera>();
        makerUI.SetActive(false);
    }

    private void Update()
    {
        IsGrounded();
        UpdateState();
        FallActionMaker();
        RangeUI();
    }

    private void FixedUpdate()
    {
        _canGrowStab = _playerGrowStab.CanGrowStab(_currentStabCount);
        FixedUpdateState();
    }

    private void OnChangeScene()
    {
        InputSystemManager.Instance.onFirePerformed -= _playerRemoveDango.Remove;
        InputSystemManager.Instance.onAttackPerformed -= OnAttack;
        InputSystemManager.Instance.onEatDangoPerformed -= OnEatDango;
        InputSystemManager.Instance.onEatDangoCanceled -= () => _hasStayedEat = false;
        InputSystemManager.Instance.onPausePerformed -= OnChangeToUIAction;
        InputSystemManager.Instance.onJumpPerformed -= _playerJump.Jump;
    }

    private void OnJump()
    {
        InputSystemManager.Instance.onFirePerformed -= _playerRemoveDango.Remove;
        InputSystemManager.Instance.onEatDangoPerformed -= OnEatDango;
        InputSystemManager.Instance.onEatDangoCanceled -= () => _hasStayedEat = false;
    }

    private void OnJumpExit()
    {
        InputSystemManager.Instance.onFirePerformed += _playerRemoveDango.Remove;
        InputSystemManager.Instance.onEatDangoPerformed += OnEatDango;
        InputSystemManager.Instance.onEatDangoCanceled += () => _hasStayedEat = false;
    }

#if UNITY_EDITOR
    //デバッグ終わりに削除
    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 100, 50), "" + _currentState);
    }
#endif

    private void InitDangos()
    {
        if (_dangos == null) return;

        //初期化
        _dangos.Clear();
    }

    private void FallActionMaker()
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            makerUI.transform.position = hit.point + new Vector3(0, 0.01f, 0);
            //突き刺しできるようになったら有効化
            if (!Physics.Raycast(ray, capsuleCollider.height + capsuleCollider.height / 2f))
                makerUI.SetActive(true);
            else
                makerUI.SetActive(false);
        }

    }

    private void RangeUI()
    {
        var ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            rangeUI.transform.position = new Vector3(rangeUI.transform.position.x, hit.point.y + 0.01f, rangeUI.transform.position.z);
        }

        //空中でも出てると違和感あったので消します
        if (_isGround)
        {
            rangeUI.SetActive(true);
        }
        else
        {
            rangeUI.SetActive(false);
        }
    }

    private bool CanStab()
    {
        //団子がこれ以上させないなら実行しない
        if (_dangos.Count >= _currentStabCount)
        {
            Logger.Warn("突き刺せる数を超えています");
            _playerUIManager.EventText.TextData.SetText("それ以上させないよ！");

            return false;
        }

        //刺す時間の内部でカウントしているTimeをリセット
        _playerAttack.ResetTime();

        //突き刺せる状態にして
        spitManager.isSticking = true;

        //串の位置を変更（アニメーション推奨）
        if (_playerFall.IsFallAction)
        {
            spitManager.gameObject.transform.localPosition = new Vector3(0, -2f, 0);
            spitManager.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            spitManager.gameObject.transform.localPosition = new Vector3(0, 0, 2.2f);
            spitManager.gameObject.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        }

        return true;
    }

    private void IsGrounded()
    {
        var ray = new Ray(transform.position, Vector3.down);
        IsGround = Physics.Raycast(ray, capsuleCollider.height / 1.999f);
    }

    /// <summary>
    /// 満腹度をへらす関数、fixedUpdateに配置。
    /// </summary>
    private void DecreaseSatiety()
    {
        //満腹度を0.02秒(fixedUpdateの呼ばれる秒数)減らす
        _satiety -= Time.fixedDeltaTime;
    }

    private void OnChargeCameraMoving()
    {
        _cameraComponent.fieldOfView -= 10f * Time.deltaTime;
    }

    private IEnumerator ResetCameraView()
    {
        float view = _cameraComponent.fieldOfView;
        float hokann = DEFAULT_CAMERA_VIEW - view;
        while (_cameraComponent.fieldOfView <= DEFAULT_CAMERA_VIEW)
        {
            _cameraComponent.fieldOfView += (hokann / CAMERA_REMOVETIME) * Time.deltaTime;
            yield return null;
        }
        _cameraComponent.fieldOfView = DEFAULT_CAMERA_VIEW;
    }

    #region GetterSetter
    public int GetMaxDango() => _currentStabCount;
    public List<DangoColor> GetDangos() => _dangos;
    public void AddDangos(DangoColor d) => _dangos.Add(d);
    public float GetSatiety() => _satiety;
    public DangoUIScript GetDangoUIScript() => _dangoUISC;

    #endregion
}