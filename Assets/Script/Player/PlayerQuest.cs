using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuest : MonoBehaviour
{
    private static PlayerQuest playerQuest;
    public static PlayerQuest quest { get { return playerQuest; } }

    public QuestComplete questData;                                 // 플레이어가 클리어한 퀘스트 목록

    public Dictionary<string, object> questList;                    // 변수명에 대응하는 bool값을 반환하는 배열
    private bool visitLib = false;                                  // 처음 도서관 방문 여부
    private bool readBook = false;                                  // 전설 서적 읽은 여부

    public string nowQuest = "";                                    // 현재 진행하고 있는 퀘스트 이름
    private int nowQuestIndex = 0;                                  // 현재 진행하고 있는 퀘스트의 인덱스

    private string[] questOrder = { "visitLib", "readBook" };       // 퀘스트 이름

    public void Awake()
    {
        if (playerQuest == null)
        {
            playerQuest = this;

            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(this);

        // 클리어한 퀘스트를 가져와서 퀘스트 반영
        questData = Resources.Load<QuestComplete>("QuestData");
        visitLib = questData.visitLib;
        readBook = questData.readBook;

        // 변수명에 대응하는 bool값을 초기화 함
        questList = new Dictionary<string, object>
        {
            { nameof(visitLib), visitLib },
            { nameof(readBook), readBook },
        };

        // 해당 맵에 퀘스트를 저장함
        foreach (string name in questList.Keys)
        {
            if (!(bool)questList[name])
            {
                nowQuest = questOrder[nowQuestIndex];
                break;
            }

            nowQuestIndex++;
        }
    }

    // 다음 퀘스트로 넘어가기 위한 메소드
    public void NextQuest()
    {
        // 현재 퀘스트를 true로 변겨
        questList[nowQuest] = true;

        if (nowQuest == "visitLib")
            questData.visitLib = true;

        else if (nowQuest == "readBook")
            questData.readBook = true;

        // 다음 퀘스트로 넘어감
        if (questOrder.Length > nowQuestIndex + 1)
            nowQuestIndex++;
        nowQuest = questOrder[nowQuestIndex];
    }
}