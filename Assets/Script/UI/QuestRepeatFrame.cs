using TMPro;
using UnityEngine;

public class QuestRepeatFrame : MonoBehaviour
{
    [SerializeField] private QuestInfo questInfo;               // ����Ʈ ����
    [SerializeField] private TextMeshProUGUI goldText;          // ��� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI expText;           // ����ġ �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI contentText;       // ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI detailTitleText;   // ���� Ÿ��Ʋ �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI overallTitleText;  // ��� Ÿ��Ʋ �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI shortContentText;  // ��� ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI targetText;        // ��ǥ �ؽ�Ʈ

    private int currentQuest = 0;
    private bool isClear = false;
    
    private void SetInfo()
    {
        goldText.text = questInfo.questTitleContents[currentQuest].getCost.ToString("#,##0");
        expText.text = questInfo.questTitleContents[currentQuest].getEXP.ToString("#,##0");
        detailTitleText.text = questInfo.questTitleContents[currentQuest].questTitle + "\n[�ݺ� ����Ʈ]";
        overallTitleText.text = questInfo.questTitleContents[currentQuest].questTitle;
        shortContentText.text = questInfo.questTitleContents[currentQuest].questShortContent;
        contentText.text = questInfo.questTitleContents[currentQuest].questContent;

        if (questInfo.name.Contains("Level"))
            targetText.text = questInfo.questTitleContents[currentQuest].clearCondition.ToString("#,##0") + " ����";

        else if (questInfo.name.Contains("Kill"))
            targetText.text = questInfo.questTitleContents[currentQuest].clearCondition.ToString("#,##0") + " ų";

        else if (questInfo.name.Contains("Money"))
            targetText.text = questInfo.questTitleContents[currentQuest].clearCondition.ToString("#,##0") + " ���";
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