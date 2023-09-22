using UnityEngine;
using UnityEngine.UI;

// 플레이어 또는 몬스터가 말풍선 없이 말할 때 사용하는 스크립트
// 사용시 필요한 것 : UI를 출력하기 위해서 PrintText 메소드 출력, UI를 없애기 위해서 EndChat() 메소드 실행
public class TalkOrReadText : MonoBehaviour
{
    public Text nameText;                   // 대화를 하는 오브젝트의 이름 텍스트
    public Text contentText;                // 대화 내용 텍스트
    public float printSpeed = 0.01f;        // 텍스트 출력 속도 (글자/초)
    public bool isPrinting = false;         // 대화 내용 출력 중인지 여부

    private string printString;             // 출력할 문자열
    private string currentContent = "";     // 현재 출력 중인 대화 내용
    private int currentContentIndex = 0;    // 대화 내용에서 현재 출력 중인 문자의 인덱스
    private bool isOff = true;              // UI 초기 상태 (꺼짐)

    void Start()
    {
        // 게임 오브젝트의 자식 오브젝트를 비활성화
        SetChildObjectsActive(false);
    }

    void Update()
    {
        // 대화 내용 출력 중인 경우
        if (isPrinting)
        {
            // Space 키 또는 마우스 클릭으로 텍스트 출력을 바로 완료
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                contentText.text = printString;
        }

        // 텍스트 출력 처리
        UpdateTextDisplay();
    }

    // UI를 출력하기 위한 메소드
    public void PrintText(string name, string text)
    {
        isOff = false;                      // UI를 실행함
        SetChildObjectsActive(true);        // UI를 킴

        printString = text;

        nameText.text = name;               // 이름 출력하는 텍스트는 효과 없이 출력
        StartPrintingText(text);            // 내용부분은 한글자씩 출력하는 이펙트하면서 출력
    }

    // UI를 끄기 위한 메소드
    public void EndChat()
    {
        // 만약 UI가 켜져있다면 종료
        if (!isOff)
            SetChildObjectsActive(false);

        isOff = true;               // UI 종료
        currentContent = "";        // 내용 초기화
        currentContentIndex = 0;

        isPrinting = false;         // 출력 여부 종료
        contentText.text = "";      // 텍스트 초기화
    }

    // UI를 키기 위한 메소드
    private void SetChildObjectsActive(bool active)
    {
        // 자식 오브젝트 활성화/비활성화
        foreach (Transform child in transform)
            child.gameObject.SetActive(active);
    }

    // 텍스트를 출력하기 위해 초기화하는 메소드
    private void StartPrintingText(string text)
    {
        currentContent = text;
        currentContentIndex = 0;
        isPrinting = true;
    }

    // 일정한 시간 간격으로 출력하기 위한 메소드
    private void UpdateTextDisplay()
    {
        // 현재 출력중이 아닌 경우 중지
        if (!isPrinting)
            return;

        // 모든 텍스트가 출력될 때까지 출력
        if (currentContentIndex < currentContent.Length)
        {
            // 한 글자씩 텍스트에 추가
            int charactersToAdd = Mathf.CeilToInt(printSpeed * Time.deltaTime);
            currentContentIndex += charactersToAdd;
            currentContentIndex = Mathf.Clamp(currentContentIndex, 0, currentContent.Length);

            contentText.text = currentContent.Substring(0, currentContentIndex);
        }

        // 텍스트 출력이 완료된 경우
        else
            isPrinting = false;
    }
}