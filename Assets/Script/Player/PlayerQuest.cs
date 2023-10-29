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
    private bool chatLibWoman = false;
    private bool chatNPC = false;                                   // NPC�� ��ȭ�� ����

    public string nowQuest = "";                                    // ���� �����ϰ� �ִ� ����Ʈ �̸�
    private int nowQuestIndex = 0;                                  // ���� �����ϰ� �ִ� ����Ʈ�� �ε���

    private string[] questOrder = 
    { 
        "visitLib", "readBook", "chatLibWoman", "chatNPC"
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
        questData = Resources.Load<QuestComplete>("QuestData");
        visitLib = questData.visitLib;
        readBook = questData.readBook;
        chatLibWoman = questData.chatLibWoman;
        chatNPC = questData.chatNPC;

        // ������ �����ϴ� bool���� �ʱ�ȭ ��
        questList = new Dictionary<string, object>
        {
            { nameof(visitLib), visitLib },
            { nameof(readBook), readBook },
            { nameof(chatLibWoman),chatLibWoman },
            { nameof(chatNPC), chatNPC }
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

        // ���� ����Ʈ�� �Ѿ
        if (questOrder.Length > nowQuestIndex + 1)
            nowQuestIndex++;
        nowQuest = questOrder[nowQuestIndex];
    }
}