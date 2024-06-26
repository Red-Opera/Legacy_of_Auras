using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestTitleContent
{
    [SerializeField] public string questTitle;         // ����Ʈ Ÿ��Ʋ
    [SerializeField] public string questShortContent;  // ����Ʈ ���� ���
    [SerializeField] public string questContent;       // ����Ʈ ����
    [SerializeField] public int getCost;               // Ŭ����� ȹ�� ���
    [SerializeField] public int getEXP;                // Ŭ����� ȹ�� ����ġ
    [SerializeField] public int clearCondition;        // �ּ� Ŭ���� ����
}

[CreateAssetMenu(fileName = "RepeatQuestInfo", menuName = "Quest System/Repeat Quest Information")]
public class QuestInfo : ScriptableObject
{
    public List<QuestTitleContent> questTitleContents;
}