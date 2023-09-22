using System.Collections;
using UnityEngine;

// 주인공이 책을 읽는 장면을 묘사하기 위한 스크립트
public class ReadBookScenario : MonoBehaviour
{
    private GameObject player;          // 플레이어 오브젝트
    private TalkOrReadText readText;    // 대화창 컴포넌트
    private TypeStory typeStory;        // 책을 펼치기 위한 컴포넌트
    private bool firstRead = false;     // 처음으로 도서관에서 책을 읽었는지 확인하는 값

    private void Start()
    {
        player = GameObject.Find("player");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");

        readText = player.transform.GetChild(3).GetComponent<TalkOrReadText>();
        Debug.Assert(readText != null, "Error (Null Reference) : 대화창 컴포넌트가 존재하지 않습니다.");

        typeStory = GetComponent<TypeStory>();
    }

    void Update()
    {
        if (firstRead)
            return;

        if (!(bool)PlayerQuest.quest.questList["readBook"] || (typeStory != null && typeStory.hasActivatedCanvas))
            return;

        // 처음으로 도서관에 있는 책을 읽을 경우 (퀘스트를 클리어한 경우)
        firstRead = true;

        // 플레이어 독백 UI 실행
        StartCoroutine(Talk());
    }

    // 할말을 한글자씩 출력하는 메소드
    private IEnumerator Talk()
    {
        readText.PrintText("Player", "나는 플레이어이다. ~~~~~~");

        while (readText.isPrinting || !(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            yield return null;
        readText.PrintText("Player", "Show me the money");

        while (readText.isPrinting || !(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            yield return null;
        readText.EndChat();
    }
}