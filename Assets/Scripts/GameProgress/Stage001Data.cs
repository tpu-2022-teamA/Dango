using Dango.Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage001Data
{
    public static Stage001Data Instance = new();

    private Stage001Data()
    {
    }

    QuestManager _questManager = QuestManager.Instance;

    public List<QuestData> QuestData = new();

    static readonly DangoColor[] dangoColors = { DangoColor.Red, DangoColor.Orange, DangoColor.Yellow, DangoColor.Green, DangoColor.Cyan, DangoColor.Blue, DangoColor.Purple };

    public void AddQuest()
    {
        List<QuestData> quest = new()
        {
            _questManager.Creater.CreateQuestCreateRole(0, dangoColors, true, false, 1, 0, 0, "何らかの役を成立させる", 30f, false, false, new int[] { 2, 3 }),
            _questManager.Creater.CreateQuestCreateRole(1, dangoColors, false, false, 1, 0, 0, "役を成立させずに団子を食べる", 15f, false, false, new int[] { 2, 3 }),

            _questManager.Creater.CreateQuestCreateRole(2, dangoColors, true, false, 1, 0, 2, "2色でできる役を作る", 30f, false, false, new int[] { 5 }),
            _questManager.Creater.CreateQuestCreateRole(3, dangoColors, true, false, 1, 0, 1, "1色でできる役を作る", 0f, true, false, new int[] { 4 }),

            _questManager.Creater.CreateQuestPlayAction(4, QuestPlayAction.PlayerAction.FallAttack, 3, "急降下刺しで3回刺す", 0f, true, false, new int[] { 6, 7 }),
            _questManager.Creater.CreateQuestEatDango(5, DangoColor.Red, 3, 0, true, true, "赤色の団子を3つ食べる", 15f, false, false, new int[] { 6, 7 }),

            //Cube001-20付近
            _questManager.Creater.CreateQuestDestination(6, false, "城の南西の中庭に向かえ", 30f, true, false, new int[] { 8, 9 }),

            //Cube001-13付近
            _questManager.Creater.CreateQuestDestination(7, false, "城の北側の中庭に向かえ", 30f, true, false, new int[] { 8, 9 }),

            _questManager.Creater.CreateQuestCreateRole(8, DangoColor.Orange, true, true, 3, 0, 0, "橙色の団子を含んで役を3回作れ", 30f, false, false, new int[] { 10 }),
            _questManager.Creater.CreateQuestCreateRole(9, DangoColor.Green, true, true, 3, 0, 0, "緑色の団子を含んで役を3回作れ", 30f, false, false, new int[] { 10 }),

            _questManager.Creater.CreateQuestDestination(10, false, "城の宝物庫へ向かえ", 0f, false, true, 0),




        };

        QuestData.AddRange(quest);
        _questManager.ChangeQuest(quest[0], quest[1]);
    }
}