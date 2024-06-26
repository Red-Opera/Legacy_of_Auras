using UnityEngine;

public class QuestDetail : MonoBehaviour
{
    [SerializeField] private GameObject questClear; // 퀘스트 클리어 이미지
    [SerializeField] private string questName;      // 퀘스트 이름
    
    private static QuestComplete questCompleteList; // 퀘스트 완료 리스트
    private static GameObject playerQusetObj;       // 플레이어
    private static PlayerQuest playerQuest;         // 플레이어 퀘스트 관리 스크립트

    public void OnEnable()
    {
        if (questCompleteList == null)
        {
            Debug.Assert(questClear != null, "오류 (Null Reference): 체크 표시 오브젝트가 존재하지 않습니다.");

            questCompleteList = Resources.Load<QuestComplete>("Quest/QuestData");
            Debug.Assert(questCompleteList != null, "오류 (Null Reference): 퀘스트 완료 정보가 존재하지 않습니다.");

            playerQusetObj = GameObject.Find("PlayerQuest");
            Debug.Assert(playerQusetObj != null, "오류 (Null Reference): 플레이어가 존재하지 않습니다.");

            playerQuest = playerQusetObj.GetComponent<PlayerQuest>();
            Debug.Assert(playerQuest != null, "오류 (Null Reference): 플레이어 퀘스트가 존재하지 않습니다.");
        }
    }

    // 퀘스트 세부 정보를 열 경우 갱신하는 메소드
    public void RenewalDetail()
    {
        // 퀘스트 완료 정보 가져옴
        bool isClear = (bool)playerQuest.questList[questName];

        questClear.SetActive(isClear);
    }
}
