using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TM.Input.KeyConfig;

public class OptionManager : MonoBehaviour
{
    /// <summary>オプション画面の選択肢</summary>
    enum OptionChoices
    {
        Option,

        KeyConfig,
        Operation,
        Sound,
        Other,

        Max,
    }

    enum OptionDirection
    {
        Vertical,
        Horizontal,
    }

    #region メンバ
    [SerializeField] KeyConfigManager _keyConfig = default!;
    [SerializeField] OperationManager _operationManager = default!;
    [SerializeField] SoundSettingManager _soundSettingManager = default!;
    [SerializeField] OtherSettingsManager _otherSettingsManager = default!;

    //現在未使用
    [SerializeField] Canvas _canvas = default!;

    [SerializeField] TextUIData[] _optionTexts;
    [SerializeField] FusumaManager _fusumaManager;

    /// <summary>
    /// 静的に取得出来るオプションキャンバス
    /// </summary>
    /// 単一シーンで実装するため、各画面間のやりとりの際に使用する静的なものです。
    public static Canvas OptionCanvas { get; private set; }

    //次に以降するオプション地点
    OptionChoices _nextChoice = OptionChoices.KeyConfig;

    //縦か横か
    static readonly OptionDirection direction = OptionDirection.Horizontal;
    static readonly Vector2[,] directionTable = { { Vector2.up, Vector2.down }, { Vector2.left, Vector2.right } };

    //上端から下端に移動するか否か
    static readonly bool canMoveTopToBottom = false;

    #endregion

    private void Awake()
    {
        OptionCanvas = _canvas;

        EnterNextChoice();
        SetFontSize();
    }

    private void Start()
    {
        OptionInputEnable();
        _fusumaManager.Open();
    }

    private void OptionInputEnable()
    {
        InputSystemManager.Instance.onBackPerformed += OnBack;
        InputSystemManager.Instance.onTabControlPerformed += ChangeChoice;
    }

    private void OptionInputDisable()
    {
        InputSystemManager.Instance.onBackPerformed -= OnBack;
        InputSystemManager.Instance.onTabControlPerformed -= ChangeChoice;
        _keyConfig.OnChangeScene();
    }

    private async void OnBack()
    {
        if (_keyConfig.IsPopup) return;
        if (!_keyConfig.CheckHasKeyAllActions()) return;

        OptionInputDisable();
        await _fusumaManager.UniTaskClose(1.5f);
        SceneSystem.Instance.Load(SceneSystem.Scenes.Menu);
        SceneSystem.Instance.UnLoad(SceneSystem.Scenes.Option);
    }

    private void ChangeChoice()
    {
        if (_keyConfig.IsPopup) return;
        if (!_keyConfig.CheckHasKeyAllActions()) return;

        Vector2 axis = InputSystemManager.Instance.TabControlAxis;

        //Up or Left
        if (axis == directionTable[(int)direction, 0])
        {
            _nextChoice--;

            if (_nextChoice == OptionChoices.Option) _nextChoice = canMoveTopToBottom ? OptionChoices.Max - 1 : OptionChoices.Option + 1;

            SetFontSize();
        }
        //Down or Right
        else if (axis == directionTable[(int)direction, 1])
        {
            _nextChoice++;

            if (_nextChoice == OptionChoices.Max) _nextChoice = canMoveTopToBottom ? OptionChoices.Option + 1 : OptionChoices.Max - 1;

            SetFontSize();
        }

        EnterNextChoice();
    }

    private void EnterNextChoice()
    {
        switch (_nextChoice)
        {
            case OptionChoices.Operation:
                EnterOperation();
                break;
            case OptionChoices.KeyConfig:
                EnterKeyConfig();
                break;
            case OptionChoices.Sound:
                EnterSound();
                break;
            case OptionChoices.Other:
                EnterOther();
                break;
        }
    }

    private void EnterKeyConfig()
    {
        _keyConfig.SetCanvasEnable(true);
        _soundSettingManager.SetCanvasEnable(false);
        _operationManager.SetCanvasEnable(false);
        _otherSettingsManager.SetCanvasEnable(false);
    }

    private void EnterOperation()
    {
        _operationManager.SetCanvasEnable(true);
        _keyConfig.SetCanvasEnable(false);
        _soundSettingManager.SetCanvasEnable(false);
        _otherSettingsManager.SetCanvasEnable(false);
    }

    private void EnterSound()
    {
        _operationManager.SetCanvasEnable(false);
        _keyConfig.SetCanvasEnable(false);
        _soundSettingManager.SetCanvasEnable(true);
        _otherSettingsManager.SetCanvasEnable(false);
    }

    private void EnterOther()
    {
        _operationManager.SetCanvasEnable(false);
        _keyConfig.SetCanvasEnable(false);
        _soundSettingManager.SetCanvasEnable(false);
        _otherSettingsManager.SetCanvasEnable(true);
    }

    private void SetFontSize()
    {
        for (int i = 0; i < _optionTexts.Length; i++)
        {
            float size = (int)_nextChoice - 1 == i ? 65f : 55.5f;

            _optionTexts[i].TextData.SetFontSize(size);
        }
    }
}