using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TM.Easing.Management;

public class PortraitScript : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TextUIData text;

    bool _isChangePortrait;

    const float OFFSET = -725f;
    const float SLIDETIME = 0.5f;

    //transformのインスタンス取得
    Transform _trans;

    private void Awake()
    {
        _trans = transform;
        _trans.localPosition = Vector3.zero.SetX(OFFSET);
    }

    public void ChangeText(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            text.TextData.SetText();
            return;
        }

        text.TextData.SetText(message);
    }

    public void ChangeImg(Sprite image)
    {
        img.sprite = image;
    }

    private async UniTask SlideIn()
    {
        float time = 0;

        //位置の初期化
        _trans.localPosition = Vector3.zero.SetX(OFFSET);

        while (time < SLIDETIME)
        {
            await UniTask.Yield();
            time += Time.deltaTime;

            _trans.localPosition = Vector3.zero.SetX(OFFSET * (1f - EasingManager.EaseProgress(TM.Easing.EaseType.OutQuart, time, SLIDETIME, 0f, 0)));
        }

        _trans.localPosition = Vector3.zero;
    }

    private async UniTask SlideOut()
    {
        float time = 0;

        //位置の初期化
        _trans.localPosition = Vector3.zero;

        while (time < SLIDETIME)
        {
            await UniTask.Yield();
            time += Time.deltaTime;

            _trans.localPosition = Vector3.zero.SetX(OFFSET * EasingManager.EaseProgress(TM.Easing.EaseType.InQuart, time, SLIDETIME, 0f, 0));
        }

        _trans.localPosition = Vector3.zero.SetX(OFFSET);
    }

    public async UniTask ChangePortraitText(PortraitTextData questTextData)
    {
        //イベント進行終了まで待機
        while (_isChangePortrait) await UniTask.Yield();

        if (questTextData.TextDataIndex == 0) return;

        //データ初期設定
        PortraitTextData.PTextData data = questTextData.GetQTextData(0);
        ChangeText(data.text);

        await SlideIn();

        //進行中フラグをオンにする
        _isChangePortrait = true;

        //イベント進行
        for (int i = 0; i < questTextData.TextDataIndex; i++)
        {
            data = questTextData.GetQTextData(i);

            ChangeText(data.text);
            await UniTask.Delay((int)(data.printTime * 1000));
        }

        await SlideOut();

        //進行中フラグをオフにする
        _isChangePortrait = false;
    }
}
