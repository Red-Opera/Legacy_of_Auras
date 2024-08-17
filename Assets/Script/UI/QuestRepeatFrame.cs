using TMPro;
using UnityEngine;

public class QuestRepeatFrame : MonoBehaviour
{
    [SerializeField] private QuestInfo questInfo;               // 퀘스트 정보
    [SerializeField] private TextMeshProUGUI goldText;          // 골드 텍스트
    [SerializeField] private TextMeshProUGUI expText;           // 경험치 텍스트
    [SerializeField] private TextMeshProUGUI contentText;       // 내용 텍스트
    [SerializeField] private TextMeshProUGUI detailTitleText;   // 세부 타이틀 텍스트
    [SerializeField] private TextMeshProUGUI overallTitleText;  // 요약 타이틀 텍스트
    [SerializeField] private TextMeshProUGUI shortContentText;  // 요약 내용 텍스트
    [SerializeField] private TextMeshProUGUI targetText;        // 목표 텍스트

    private int currentQuest = 0;
    private bool isClear = false;
    
    private void SetInfo()
    {
        goldText.text = questInfo.questTitleContents[currentQuest].getCost.ToString("#,##0");
        expText.text = questInfo.questTitleContents[currentQuest].getEXP.ToString("#,##0");
        detailTitleText.text = questInfo.questTitleContents[currentQuest].questTitle + "\n[반복 퀘스트]";
        overallTitleText.text = questInfo.questTitleContents[currentQuest].questTitle;
        shortContentText.text = questInfo.questTitleContents[currentQuest].questShortContent;
        contentText.text = questInfo.questTitleContents[currentQuest].questContent;

        if (questInfo.name.Contains("Level"))
            targetText.text = questInfo.questTitleContents[currentQuest].clearCondition.ToString("#,##0") + " 레벨";

        else if (questInfo.name.Contains("Kill"))
            targetText.text = questInfo.questTitleContents[currentQuest].clearCondition.ToString("#,##0") + " 킬";

        else if (questInfo.name.Contains("Money"))
            targetText.text = questInfo.questTitleContents[currentQuest].clearCondition.ToString("#,##0") + " 골드";
    }

    private void OnEnable()
    {
        if (!isClear)
        {
            int clearCondition = questInfo.questTitleContents[currentQuest].clearCondition;

            if (questInfo.name.Contains("Level"))
            {
                if (currentQuest < questInfo.questTitleContents.Count - 1)
                    

                while (clearCondition <= GameManager.info.playerState.Level && currentQuest < questInfo.questTitleContents.Count - 1)
                {
                    currentQuest++;
                    clearCondition = questInfo.questTitleContents[currentQuest].clearCondition;
                }
            }

            else if (questInfo.name.Contains("Kill"))
            {
                while (clearCondition <= GameManager.info.playerState.kills && currentQuest < questInfo.questTitleContents.Count - 1)
                {
                    currentQuest++;
                    clearCondition = questInfo.questTitleContents[currentQuest].clearCondition;
                }
            }

            else if (questInfo.name.Contains("Money"))
            {
                while (clearCondition <= GameManager.info.playerState.money && currentQuest < questInfo.questTitleContents.Count - 1)
                {
                    currentQuest++;
                    clearCondition = questInfo.questTitleContents[currentQuest].clearCondition;
                }
            }

            SetInfo();

            isClear = true;
        }
    }

    public void UpdateQuest()
    {
        isClear = false;
        OnEnable();
    }
}