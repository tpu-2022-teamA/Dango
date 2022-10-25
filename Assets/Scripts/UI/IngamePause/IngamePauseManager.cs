using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngamePauseManager : MonoBehaviour
{
    enum IngameChoices
    {
        Settings,
        BackToGame,
        BackToMenu,

        Max,
    }

    enum WarningChoices
    {
        OK,
        NG,

        Max,
    }

    [SerializeField] Canvas _ingamePauseCanvas;
    [SerializeField] ImageUIData[] _ingameImages;
    [SerializeField] ImageUIData[] _warningImages;
    [SerializeField] GameObject _warning;

    IngameChoices _currentIngameChoice;
    WarningChoices _currentWarningChoice;

    bool IsPopup => _warning.activeSelf;
    bool isFirst = true;

    //��[���牺�[�Ɉړ����邩�ۂ�
    static readonly bool canMoveTopToBottom = true;

    private void OnEnable()
    {
        //TODO�FBGM��������
        //TODO�F�ڂ���������
        _currentIngameChoice = 0;
        _currentWarningChoice = 0;

        InputSystemManager.Instance.onNavigatePerformed += OnNavigate;
        InputSystemManager.Instance.onChoicePerformed += OnChoicePerformed;
        InputSystemManager.Instance.onBackPerformed += OnBack;

        _ingameImages[0].ImageData.SetColor(Color.red);
        _warningImages[0].ImageData.SetColor(Color.red);

        _warning.SetActive(false);
    }

    private void OnExit()
    {
        InputSystemManager.Instance.onNavigatePerformed -= OnNavigate;
        InputSystemManager.Instance.onChoicePerformed -= OnChoicePerformed;
        InputSystemManager.Instance.onBackPerformed -= OnBack;

        ResetImagesColor(_warningImages);
        ResetImagesColor(_ingameImages);
    }

    private void OnNavigate()
    {
        if (!gameObject.activeSelf) return;

        Vector2 axis = InputSystemManager.Instance.NavigateAxis;

        ChangeIngameChoices(axis);
        ChangeWarningChoice(axis);
    }

    private void ChangeWarningChoice(Vector2 axis)
    {
        if (!IsPopup) return;

        if (!ChangeChoiceUtil.Choice(axis, ref _currentWarningChoice, WarningChoices.Max, canMoveTopToBottom, ChangeChoiceUtil.OptionDirection.Horizontal)) return;

        ResetImagesColor(_warningImages);
        _warningImages[(int)_currentWarningChoice].ImageData.SetColor(Color.red);
        SoundManager.Instance.PlaySE(SoundSource.SE16_UI_SELECTION);
    }

    private void ChangeIngameChoices(Vector2 axis)
    {
        if (IsPopup) return;

        if (!ChangeChoiceUtil.Choice(axis, ref _currentIngameChoice, IngameChoices.Max, canMoveTopToBottom, ChangeChoiceUtil.OptionDirection.Horizontal)) return;

        ResetImagesColor(_ingameImages);
        _ingameImages[(int)_currentIngameChoice].ImageData.SetColor(Color.red);
        SoundManager.Instance.PlaySE(SoundSource.SE16_UI_SELECTION);
    }

    private void OnChoicePerformed()
    {
        if (!gameObject.activeSelf) return;

        EnterIngameChoice();
        EnterWarningChoice();

        SoundManager.Instance.PlaySE(SoundSource.SE17_UI_DECISION);
    }

    private void EnterIngameChoice()
    {
        if (IsPopup) return;

        switch (_currentIngameChoice)
        {
            case IngameChoices.Settings:
                SceneSystem.Instance.Load(SceneSystem.Scenes.Option);
                SceneSystem.Instance.UnLoad(SceneSystem.Scenes.InGamePause, false);
                OnExit();
                break;

            case IngameChoices.BackToGame:
                BackToGame();
                break;

            case IngameChoices.BackToMenu:
                _warning.SetActive(true);

                WarningPopup(true);
                break;
        }
    }

    private void EnterWarningChoice()
    {
        if (!IsPopup) return;

        //����̓|�b�v�A�b�v�Ɠ��t���[���ŗ��Ă��܂����ߒe��
        if (isFirst)
        {
            isFirst = false;
            return;
        }

        //NG�I���Ȃ�|�b�v�A�b�v�����B�܂���Back�L�[�������ꂽ�����
        if (_currentWarningChoice == WarningChoices.NG)
        {
            WarningPopup(false);
        }
        //OK�I���Ȃ�X�e�[�W�Ƃ��̃|�[�Y��ʂ�j�����A���j���[�ɖ߂�
        else if (_currentWarningChoice == WarningChoices.OK)
        {
            SceneSystem.Instance.UnLoad(SceneSystem.Scenes.InGamePause, true);
            SceneSystem.Instance.UnLoad(SceneSystem.Instance.CurrentIngameScene, true);

            SceneSystem.Instance.Load(SceneSystem.Scenes.Menu);
            ResetBGMAndBlur();
            OnExit();
        }

        isFirst = true;
    }

    private void OnBack()
    {
        if (IsPopup) WarningPopup(false);
        else BackToGame();
    }

    private void BackToGame()
    {
        InputSystemManager.Instance.Input.SwitchCurrentActionMap("Player");
        OnExit();

        //UnLoad���ƌ�����������canvas�̔�A�N�e�B�u�ɂ������B�������[�h�������Ȃ��B���Ċ���
        SceneSystem.Instance.UnLoad(SceneSystem.Scenes.InGamePause, false);

        ResetBGMAndBlur();
    }

    private void ResetImagesColor(ImageUIData[] images)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].ImageData.SetColor(Color.white);
        }
    }

    private void ResetBGMAndBlur()
    {
        //TODO�FBGM��߂�
        //TODO�F�ڂ�����߂�
    }

    private void WarningPopup(bool enable)
    {
        _warning.SetActive(enable);
    }
}