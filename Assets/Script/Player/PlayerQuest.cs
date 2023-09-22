using System.Collections;
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

    public string nowQuest = "";                                    // ���� �����ϰ� �ִ� ����Ʈ �̸�
    private int nowQuestIndex = 0;                                  // ���� �����ϰ� �ִ� ����Ʈ�� �ε���

    private string[] questOrder = { "visitLib", "readBook" };       // ����Ʈ �̸�

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

        // ������ �����ϴ� bool���� �ʱ�ȭ ��
        questList = new Dictionary<string, object>
        {
            { nameof(visitLib), visitLib },
            { nameof(readBook), readBook },
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

        // ���� ����Ʈ�� �Ѿ
        if (questOrder.Length > nowQuestIndex + 1)
            nowQuestIndex++;
        nowQuest = questOrder[nowQuestIndex];
    }
}