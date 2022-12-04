using Dango.Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageTutorialData : StageData
{
    static readonly List<DangoColor> stageDangoColors = new() { DangoColor.Beni, DangoColor.Shiratama, DangoColor.Yomogi };

    protected override void Start()
    {
        base.Start();
    }

    protected override void AddQuest()
    {
        QuestManager questManager = QuestManager.Instance;

        List<QuestData> quest = new()
        {
            questManager.Creater.CreateQuestPlayAction(0,QuestPlayAction.PlayerAction.Stab,1,"�c�q���h��",0,false,false,new(
                new PortraitTextData.PTextData(0,"�������B�h�����c�q�͉�ʉE�ɏo�Ă邩���",5f,PortraitTextData.FacePatturn.Normal),
                new(1,"���ɏW�߂��c�q��H�ׂĂ݂�B�c�q���W�߂āw�H�ׂ�x�{�^����",10f,PortraitTextData.FacePatturn.Normal)),
                1),

            questManager.Creater.CreateQuestEatDango(1, 3, 0, true, true, "�c�q��3�H�ׂ�", 0f, true, false,new(
                new PortraitTextData.PTextData(0,"�����[�������I�ǂ����q��",5f,PortraitTextData.FacePatturn.Normal),
                new(1,"���V���ȂāA�c�q��H���A���ꂪ�w�c���x��",5f,PortraitTextData.FacePatturn.Normal),
                new(2, "�w�c���x��B������ƁA�����c��邾������Ȃ��A�����L�т���A�F�X�Ɖ��b������", 10f, PortraitTextData.FacePatturn.Normal),
                new(3, "���āA�����H�ׂ邾�����Ă͖̂��C�˂��A���́w�c���x������Ă݂�", 10f, PortraitTextData.FacePatturn.Normal),
                new(4, "�w�c���x�ɂ͐F�X�Ǝ�ނ����邪�A��{�I�ɂ͋K�����������ׂĂ��΂���", 10f, PortraitTextData.FacePatturn.Normal)),
                2,3),

            questManager.Creater.CreateQuestCreateRole(2, new QuestCreateRole.EstablishRole(true, false), 1, 0, "�K�����������ׂāw�c���x�����", 0, false, false,new(
                new PortraitTextData.PTextData(0,"�����I�c�q�͊�{�I�Ɂw�c���x������ĐH�ׂ�����������������",10f,PortraitTextData.FacePatturn.Normal),
                new(1,"���́w�����сx�{�^���ō����Ƃ���ɍs���Ă݂邼",10f,PortraitTextData.FacePatturn.Normal)),
                4),
            
            questManager.Creater.CreateQuestCreateRole(3, new QuestCreateRole.EstablishRole(false, false), 1, 0, "���������̂������W�߂���A���Ώ̂�����Ă݂悤", 0, false, false,new(
                new PortraitTextData.PTextData(0,"�ɂ����ȁA����������ƋK�����������ׂĂ݂�I",8f,PortraitTextData.FacePatturn.Normal)),
                2, 3),

            questManager.Creater.CreateQuestDestination(4, FloorManager.Floor.floor2, false, "����Ɍ�����", 30f, true, false,new(
                new PortraitTextData.PTextData(0,"�ǂ�����˂����A���̒��q��",5f,PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(1,"�W�����v�̍����͋��̒����Ō��܂�B�����L�т�΂�荂���Ƃ���ɍs������Ċo���Ă���", 10f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(2,"���āA���͏����e�N�j�J���ɒc�q���h���Ă݂悤", 10f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(3,"�W�����v���э~��Ȃǂŋ󒆂ɂ����ԂŁw�˂��h���x�{�^���������Ă݂�", 10f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(4,"���̎��ɐ^���ɒc�q������Ύh�����Ƃ��ł��邺", 10f, PortraitTextData.FacePatturn.Normal)),
                5),

            questManager.Creater.CreateQuestPlayAction(5, QuestPlayAction.PlayerAction.FallAttack, 1, "�}�~���Œc�q���h��", 0f, true, true,new(
                new PortraitTextData.PTextData(0,"���˂��I��肢���I", 5f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(1,"�����炢���邺", 5f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(2,"�c�q���h���ďW�߂āw�c���x�����A���ꂪ��{", 10f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(3,"������J��Ԃ��Ȃ���A�w�c���x��B�����Ă����̂��V�ѕ���", 10f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(4,"���ɂ��w�c�q�O���x�{�^���ŋ�����c�q���O������", 10f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(5,"�w�g���\���x�{�^���Ō��݂̒c���̊m�F��������", 10f, PortraitTextData.FacePatturn.Normal),
                new PortraitTextData.PTextData(6,"�F�X�ł��邩����ۂɎ����Ă݂Ă���", 10f, PortraitTextData.FacePatturn.Normal),
           �@�@ new PortraitTextData.PTextData(7,"�ȏ�I", 5f, PortraitTextData.FacePatturn.Normal)),
            6),

            questManager.Creater.CreateQuestDestination(6, FloorManager.Floor.Max, false, "���S�Ҏw�슮���I", 0f, false, true,new(
                new PortraitTextData.PTextData()),
                0),
        };

        QuestData.Clear();
        QuestData.AddRange(quest);
        questManager.ChangeQuest(quest[0]);
    }

    protected override PortraitTextData StartPortraitText()
    {
        return new(new(0, "�c���̊�{���̊�{�́A�c�q���h���ďW�߂�Ƃ��납�炾�B", 2f, PortraitTextData.FacePatturn.Normal), new(0, "�w�˂��h���x�{�^���Œc�q���h���Ă݂�I", 2f, PortraitTextData.FacePatturn.Normal));
    }

    public override List<DangoColor> FloorDangoColors()
    {
        return stageDangoColors;
    }
}