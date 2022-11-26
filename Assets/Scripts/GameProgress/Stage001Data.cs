using Dango.Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage001Data : StageData.IAddQuest, StageData.IPortrait
{
    public static Stage001Data Instance = new();

    private Stage001Data()
    {
    }

    public List<QuestData> QuestData = new();
    static readonly List<DangoColor> stageDangoColors = new() { DangoColor.Red, DangoColor.Orange, DangoColor.Yellow, DangoColor.Green, DangoColor.Cyan, DangoColor.Blue, DangoColor.Purple };

    public void AddQuest()
    {
        QuestManager questManager = QuestManager.Instance;

        List<QuestData> quest = new()
        {
            questManager.Creater.CreateQuestCreateRole(0, new QuestCreateRole.EstablishRole(true, false), 1, 0, "何らかの役を成立させる", 30f, false, false,new(new PortraitTextData.PTextData(0,"おぉ、役が揃ったな！",2f,PortraitTextData.FacePatturn.Normal),new(0,"この調子で頼むぜ！",2f,PortraitTextData.FacePatturn.Normal)), 2, 3),
            questManager.Creater.CreateQuestCreateRole(1, new QuestCreateRole.EstablishRole(false, false), 1, 0, "役を成立させずに団子を食べる", 15f, false, false,new(new PortraitTextData.PTextData(0,"やっぱ団子は美味いな！",2f,PortraitTextData.FacePatturn.Normal),new(0,"次はキレイな順番で食ってみるか…",2f,PortraitTextData.FacePatturn.Normal)), 2, 3),

            questManager.Creater.CreateQuestCreateRole(2, new QuestCreateRole.UseColorCount(2), 1, 0, "2色でできる役を作る", 15f, false, false,new(new PortraitTextData.PTextData()), 4, 5),
            questManager.Creater.CreateQuestCreateRole(3, new QuestCreateRole.UseColorCount(1), 1, 0, "1色でできる役を作る", 45f, false, false,new(new PortraitTextData.PTextData()), 4, 5),

            //D5上昇
            questManager.Creater.CreateQuestPlayAction(4, QuestPlayAction.PlayerAction.FallAttack, 3, "急降下刺しで3回刺す", 0f, true, false,new(new PortraitTextData.PTextData()), 6),
            questManager.Creater.CreateQuestEatDango(5, DangoColor.Red, 3, 0, true, true, "赤色の団子を3つ食べる", 15f, true, false,new(new PortraitTextData.PTextData()), 6),

            questManager.Creater.CreateQuestDestination(6, FloorManager.Floor.floor10, false, "城内の中層に向かえ", 30f, false, false,new(new PortraitTextData.PTextData()), 7, 8),

            questManager.Creater.CreateQuestCreateRole(7, new QuestCreateRole.SpecifyTheRole("隣色鏡面"), 2,0, "隣色鏡面を2回作れ", 60f, true, false,new(new PortraitTextData.PTextData()), 9, 10),
            questManager.Creater.CreateQuestCreateRole(8, new QuestCreateRole.SpecifyTheRole("輪廻転生"), 2, 0, "輪廻転生を2回作れ", 60f, true, false,new(new PortraitTextData.PTextData()), 9, 10),

            //D5上昇
            questManager.Creater.CreateQuestCreateRole(9, new QuestCreateRole.EstablishRole(true,false,DangoColor.Orange), 2, 0, "橙色の団子を含んで役を2回作れ", 30f, true, false,new(new PortraitTextData.PTextData()), 11),
            questManager.Creater.CreateQuestCreateRole(10, new QuestCreateRole.EstablishRole(true,false,DangoColor.Green), 2, 0, "緑色の団子を含んで役を2回作れ", 30f, true, false,new(new PortraitTextData.PTextData()), 11),

            questManager.Creater.CreateQuestDestination(11, FloorManager.Floor.floor11, false, "城内の最上層に向かえ", 0f, false, true,new(new PortraitTextData.PTextData()), 0),
        };

        QuestData.Clear();
        QuestData.AddRange(quest);
        questManager.ChangeQuest(quest[0], quest[1]);
    }

    public PortraitTextData GetPortraitQuest()
    {
        return new(new PortraitTextData.PTextData(0, "団道を始めるぜ！", 2f, PortraitTextData.FacePatturn.Normal), new(0, "まずはクエストの確認からだな…", 2f, PortraitTextData.FacePatturn.Normal), new(0, "Lボタンを押してみようぜ！", 10f, PortraitTextData.FacePatturn.Normal));
    }

    public bool IsContainsStageDangoColor(DangoColor color)
    {
        return stageDangoColors.Contains(color);
    }
}