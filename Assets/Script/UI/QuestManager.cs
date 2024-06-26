using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] public static bool isQuestUIOn = false;

    [SerializeField] private GameObject questUI;
    [SerializeField] List<QuestRepeatFrame> repeatFrames;

    public void Start()
    {
        Debug.Assert(questUI != null, "���� (Null Reference): ����Ʈ UI�� �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TurnOnOffQusetUI();
    }

    private void TurnOnOffQusetUI()
    {
        for (int i = 0; i < repeatFrames.Count; i++)
            repeatFrames[i].UpdateQuest();

        if (questUI.activeSelf)
        {
            Cursor.visible = false;                     // Ŀ���� ����
            Cursor.lockState = CursorLockMode.Locked;   // Ŀ���� �������� �ʵ��� ����

            isQuestUIOn = false;
            questUI.SetActive(false);
        }

        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            isQuestUIOn = true;
            questUI.SetActive(true);
        }
    }
}
