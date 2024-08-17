using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 플레이어 또는 몬스터가 말풍선 없이 말할 때 사용하는 스크립트
// 사용시 필요한 것 : UI를 출력하기 위해서 PrintText 메소드 출력, UI를 없애기 위해서 EndChat() 메소드 실행
public class TalkOrReadText : MonoBehaviour
{
    public static TalkOrReadText instance;
    public bool isPrinting = false;         // 대화 내용 출력 중인지 여부
    public bool isUIClose = true;           // UI 초기 상태 (꺼짐)

    [SerializeField] private GameObject dialog;         // 대화 오브젝트
    [SerializeField] private Text nameText;             // 대화를 하는 오브젝트의 이름 텍스트
    [SerializeField] private Text contentText;          // 대화 내용 텍스트
    [SerializeField] private float printSpeed = 0.1f;   // 텍스트 출력 속도 (글자/초)

    private string printString;             // 출력할 문자열
    private string currentContent = "";     // 현재 출력 중인 대화 내용
    private int currentContentIndex = 0;    // 대화 내용에서 현재 출력 중인 문자의 인덱스

    private void Start()
    {
        instance = this;

        // 게임 오브젝트의 자식 오브젝트를 비활성화
        SetActive(false);
    }

    private void Update()
    {
        // 텍스트 출력 처리
        UpdateTextDisplay();
    }

    // 할말을 한글자씩 출력하는 메소드
    public static IEnumerator Talk(string name, List<string> content)
    {
        if (name == "")
            name = "주인공";

        for (int i = 0; i < content.Count; i++)
        {
            instance.PrintText(name, content[i]);

            yield return null;
            bool isPress = false;

            // 대화 내용 출력 중인 경우
            while (instance.isPrinting || !isPress)
            {
                isPress = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);

                if (instance.isPrinting && isPress)
                {
                    instance.isPrinting = false;
                    instance.contentText.text = content[i];
                }

                yield return null;
            }

            instance.isPrinting = false;
        }

        instance.EndChat();
    }

    // UI를 출력하기 위한 메소드
    private void PrintText(string name, string text)
    {
        isUIClose = false;          // UI를 실행함
        SetActive(true);            // UI를 킴

        printString = text;

        nameText.text = name;       // 이름 출력하는 텍스트는 효과 없이 출력
        StartPrintingText(text);    // 내용부분은 한글자씩 출력하는 이펙트하면서 출력
    }

    // UI를 끄기 위한 메소드
    private void EndChat()
    {
        // 대화 창 끔
        SetActive(false);

        isUIClose = true;           // UI 종료
        currentContent = "";        // 내용 초기화
        currentContentIndex = 0;

        isPrinting = false;         // 출력 여부 종료
        contentText.text = "";      // 텍스트 초기화
    }

    // UI를 키기 위한 메소드
    private static void SetActive(bool active)
    {
        instance.dialog.SetActive(active);
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