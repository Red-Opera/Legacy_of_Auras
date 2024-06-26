using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] public static bool isQuestUIOn = false;

    [SerializeField] private GameObject questUI;
    [SerializeField] List<QuestRepeatFrame> repeatFrames;

    public void Start()
    {
        Debug.Assert(questUI != null, "오류 (Null Reference): 퀘스트 UI가 존재하지 않습니다.");
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
            Cursor.visible = false;                     // 커서를 감춤
            Cursor.lockState = CursorLockMode.Locked;   // 커서가 움직이지 않도록 설정

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
