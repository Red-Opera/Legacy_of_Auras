using System.Collections.Generic;
using UnityEngine;

public class PlayerQuest : MonoBehaviour
{
    private static PlayerQuest playerQuest;
    public static PlayerQuest quest { get { return playerQuest; } }

    public QuestComplete questData;                                 // �÷��̾ Ŭ������ ����Ʈ ���

    public Dictionary<string, object> questList;                    // ������ �����ϴ� bool���� ��ȯ�ϴ� �迭
    private bool visitLib = false;                                  // ó�� ������ �湮 ����
    private bool readBook = false;                                  // ���� ���� ���� ����
    private bool chatLibWoman = false;                              // ������ NPC�� ��ȭ
    private bool chatNPC = false;                                   // NPC�� ��ȭ�� ����
    private bool visitDesert = false;                               // �縷 �������� �̵��� ����
    private bool getGun = false;                                    // ���� ���� ���
    private bool visitForest = false;                               // �� �������� �̵��� ����

    public string nowQuest = "";                                    // ���� �����ϰ� �ִ� ����Ʈ �̸�
    private int nowQuestIndex = 0;                                  // ���� �����ϰ� �ִ� ����Ʈ�� �ε���

    private string[] questOrder = 
    { 
        "visitLib", "readBook", "chatLibWoman", "chatNPC", "visitDesert", "getGun", "visitForest"
    };       // ����Ʈ �̸�

    public void Awake()
    {
        if (playerQuest == null)
        {
            playerQuest = this;

            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(this);

        // Ŭ������ ����Ʈ�� �����ͼ� ����Ʈ �ݿ�
        questData = Resources.Load<QuestComplete>("Quest/QuestData");
        visitLib = questData.visitLib;
        readBook = questData.readBook;
        chatLibWoman = questData.chatLibWoman;
        chatNPC = questData.chatNPC;
        visitDesert = questData.visitDesert;
        getGun = questData.getGun;
        visitForest = questData.visitForest;

        // ������ �����ϴ� bool���� �ʱ�ȭ ��
        questList = new Dictionary<string, object>
        {
            { nameof(visitLib), visitLib },
            { nameof(readBook), readBook },
            { nameof(chatLibWoman),chatLibWoman },
            { nameof(chatNPC), chatNPC },
            { nameof(visitDesert), visitDesert },
            { nameof(getGun), getGun },
            { nameof(visitForest), visitForest },
        };

        // �ش� �ʿ� ����Ʈ�� ������
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

    // ���� ����Ʈ�� �Ѿ�� ���� �޼ҵ�
    public void NextQuest()
    {
        // ���� ����Ʈ�� true�� ����
        questList[nowQuest] = true;

        if (nowQuest == "visitLib")
            questData.visitLib = true;

        else if (nowQuest == "readBook")
            questData.readBook = true;

        else if (nowQuest == "chatLibWoman")
            questData.chatLibWoman = true;

        else if (nowQuest == "chatNPC")
            questData.chatNPC = true;

        else if (nowQuest == "visitDesert")
            questData.visitDesert = true;

        else if (nowQuest == "getGun")
            questData.getGun = true;

        else if (nowQuest == "visitForest")
            questData.getGun = true;

        // ���� ����Ʈ�� �Ѿ
        if (questOrder.Length > nowQuestIndex + 1)
            nowQuestIndex++;
        nowQuest = questOrder[nowQuestIndex];
    }
}