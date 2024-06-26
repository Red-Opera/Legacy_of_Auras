using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestTitleContent
{
    [SerializeField] public string questTitle;         // 퀘스트 타이틀
    [SerializeField] public string questShortContent;  // 퀘스트 내용 요약
    [SerializeField] public string questContent;       // 퀘스트 내용
    [SerializeField] public int getCost;               // 클리어시 획득 골드
    [SerializeField] public int getEXP;                // 클리어시 획득 경험치
    [SerializeField] public int clearCondition;        // 최소 클리어 조건
}

[CreateAssetMenu(fileName = "RepeatQuestInfo", menuName = "Quest System/Repeat Quest Information")]
public class QuestInfo : ScriptableObject
{
    public List<QuestTitleContent> questTitleContents;
}