using System.Collections;
using TMPro;
using UnityEngine;

public class ChatNPC : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;         // 텍스트를 출력할 객체
    public string[] sentences;                  // 텍스트를 출력할 문장
    public float typingSpeed = 0.1f;            // 출력 속도 (단위 : 초)
    public static bool isEnd = true;            // 텍스트 출력 여부

    private GameObject player;                  // 플레이어 오브젝트
    private NPCLookAtPlayer npcLookAtPlayer;    // 플레이어를 보게하는 클래스
    private int currentSentenceIndex = 0;       // 현재 출력 중인 문장
    private int currentCharacterIndex = 0;      // 현재 출력 중인 단어
    private bool isTyping = false;              // 출력 여부

    private Animator animator;                  // 애니메이션을 실행할 객체

    private void Start()
    {
        animator = GetComponent<Animator>();
        npcLookAtPlayer = GetComponent<NPCLookAtPlayer>();
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이터가 존재하지 않습니다.");
        Debug.Assert(npcLookAtPlayer != null, "Error (Null Reference) : 플레이어를 보게하는 스크립트가 존재하지 않습니다.");

        player = GameObject.Find("Model");

        textDisplay.transform.gameObject.transform.parent.gameObject.SetActive(false);
        textDisplay.gameObject.SetActive(false);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (isEnd || distanceToPlayer > npcLookAtPlayer.interactionDistance)
            return;

        if (Input.GetMouseButtonDown(0)) // 마우스 클릭 감지
        {
            if (!isTyping)
                NextSentence();

            // 타이핑 중에 클릭하면 문장을 바로 출력합니다.
            else
                FinishTyping();
        }
    }

    public void StartChat() 
    {
        // 도서관에 있는 NPC와 대화 퀘스트를 통과할 경우
        if (gameObject.name.Equals("LibWoman") && !(bool)PlayerQuest.quest.questList["chatLibWoman"])
            PlayerQuest.quest.NextQuest();

        // 마을에 있는 NPC와 대화 퀘스트를 통과할 경우
        if (gameObject.name.Equals("NPC") && !(bool)PlayerQuest.quest.questList["chatNPC"])
                PlayerQuest.quest.NextQuest();

        StartCoroutine(TypeSentence()); 
    }

    public IEnumerator TypeSentence()
    {
        textDisplay.gameObject.SetActive(true);
        textDisplay.transform.gameObject.transform.parent.gameObject.SetActive(true);

        currentCharacterIndex = 0;

        isTyping = true;                                    // 텍스트 문장 실행
        textDisplay.text = "";                              // 텍스트 초기화
        string sentence = sentences[currentSentenceIndex];  // 출력할 문자열 가져옴

        // 이야기하는 애니메이션 실행
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Talking"))
            animator.SetTrigger("PlayerTalk");

        // 모든 문자를 출력함
        while (currentCharacterIndex < sentence.Length)
        {
            // 출력할 문자를 가져옴
            char currentChar = sentence[currentCharacterIndex];

            // 출력할 문자를 기존 문자열에 더함
            textDisplay.text += currentChar;
            currentCharacterIndex++;

            // .문자가 올경우 엔터키를 누름
            if (currentChar == '.')
            {
                textDisplay.text += '\n';
                currentCharacterIndex++;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        // 텍스트를 모두 출력할 경우 중지
        isTyping = false;
    }

    private void NextSentence()
    {
        if (currentSentenceIndex < sentences.Length - 1)
        {
            // 다음 문장의 인덱스로 넘어가고 첫단어로 이동
            currentSentenceIndex++;
            currentCharacterIndex = 0;

            // 문장 출력
            StartCoroutine(TypeSentence());
        }

        else
        {
            // 텍스트 모두 출력 여부를 참으로 설정
            isEnd = true;
            currentSentenceIndex = 0;

            animator.SetTrigger("isEnd");

            // 모든 문장이 출력되면 텍스트 이미지를 비활성화
            textDisplay.transform.gameObject.transform.parent.gameObject.SetActive(false);
            textDisplay.gameObject.SetActive(false);
        }
    }

    private void FinishTyping()
    {
        // 타이핑 중인 텍스트를 즉시 완료
        StopAllCoroutines();

        string originalSentence = sentences[currentSentenceIndex];
        string[] sentenceParts = originalSentence.Split('.');

        for (int i = 0; i < sentenceParts.Length; i++)
            sentenceParts[i] = sentenceParts[i].Trim();

        string formattedSentence = string.Join(".\n", sentenceParts);

        textDisplay.text = formattedSentence;
        isTyping = false;
    }
}