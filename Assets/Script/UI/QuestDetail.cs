using UnityEngine;

public class QuestDetail : MonoBehaviour
{
    [SerializeField] private GameObject questClear; // ����Ʈ Ŭ���� �̹���
    [SerializeField] private string questName;      // ����Ʈ �̸�
    
    private static QuestComplete questCompleteList; // ����Ʈ �Ϸ� ����Ʈ
    private static GameObject playerQusetObj;       // �÷��̾�
    private static PlayerQuest playerQuest;         // �÷��̾� ����Ʈ ���� ��ũ��Ʈ

    public void OnEnable()
    {
        if (questCompleteList == null)
        {
            Debug.Assert(questClear != null, "���� (Null Reference): üũ ǥ�� ������Ʈ�� �������� �ʽ��ϴ�.");

            questCompleteList = Resources.Load<QuestComplete>("Quest/QuestData");
            Debug.Assert(questCompleteList != null, "���� (Null Reference): ����Ʈ �Ϸ� ������ �������� �ʽ��ϴ�.");

            playerQusetObj = GameObject.Find("PlayerQuest");
            Debug.Assert(playerQusetObj != null, "���� (Null Reference): �÷��̾ �������� �ʽ��ϴ�.");

            playerQuest = playerQusetObj.GetComponent<PlayerQuest>();
            Debug.Assert(playerQuest != null, "���� (Null Reference): �÷��̾� ����Ʈ�� �������� �ʽ��ϴ�.");
        }
    }

    // ����Ʈ ���� ������ �� ��� �����ϴ� �޼ҵ�
    public void RenewalDetail()
    {
        // ����Ʈ �Ϸ� ���� ������
        bool isClear = (bool)playerQuest.questList[questName];

        questClear.SetActive(isClear);
    }
}
