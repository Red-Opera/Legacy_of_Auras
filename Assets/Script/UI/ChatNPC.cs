using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatNPC : MonoBehaviour
{
    public static bool isEnd = true;                                // �ؽ�Ʈ ��� ����

    [SerializeField] private TextMeshProUGUI textDisplay;           // �ؽ�Ʈ�� ����� ��ü
    [SerializeField] private string[] sentences;                    // �ؽ�Ʈ�� ����� ����
    [SerializeField] private List<PlayerTalkList> playerTalkList; 
    [SerializeField] private float typingSpeed;                     // ��� �ӵ� (���� : ��)

    private GameObject player;                      // �÷��̾� ������Ʈ
    private NPCLookAtPlayer npcLookAtPlayer;        // �÷��̾ �����ϴ� Ŭ����
    private int currentSentenceIndex = 0;           // ���� ��� ���� ����
    private int currentCharacterIndex = 0;          // ���� ��� ���� �ܾ�
    private int currentPlayerTalkIndex = 0;         // �÷��̾� ��ȭ �ε���
    private bool isTyping = false;                  // ��� ����

    private Animator animator;                      // �ִϸ��̼��� ������ ��ü
    private Transform bubbleTransform;              // ��ǳ�� ��ġ

    private void Start()
    {
        animator = GetComponent<Animator>();
        npcLookAtPlayer = GetComponent<NPCLookAtPlayer>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
        Debug.Assert(npcLookAtPlayer != null, "Error (Null Reference) : �÷��̾ �����ϴ� ��ũ��Ʈ�� �������� �ʽ��ϴ�.");

        player = GameObject.Find("Model");
        bubbleTransform = textDisplay.transform.parent;

        SetActive(false);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (isEnd || distanceToPlayer > npcLookAtPlayer.interactionDistance)
            return;

        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ�� ����
        {
            if (!isTyping)
                StartCoroutine(NextSentence());

            // Ÿ���� �߿� Ŭ���ϸ� ������ �ٷ� ���
            else
                FinishTyping();
        }
    }

    public void StartChat() 
    {
        // �������� �ִ� NPC�� ��ȭ ����Ʈ�� ����� ���
        if (gameObject.name.Equals("LibWoman") && !(bool)PlayerQuest.quest.questList["chatLibWoman"])
        {
            GameManager.info.alert.PushAlert("\"������ �ֹΰ� ��ȭ\" ����Ʈ Ŭ����!", true);
            PlayerQuest.quest.NextQuest();
        }

        // ������ �ִ� NPC�� ��ȭ ����Ʈ�� ����� ���
        if (gameObject.name.Equals("NPC") && !(bool)PlayerQuest.quest.questList["chatNPC"])
        {
            GameManager.info.alert.PushAlert("\"�ֹΰ� ��ȭ\" ����Ʈ Ŭ����!", true);
            PlayerQuest.quest.NextQuest();
        }

        StartCoroutine(TypeSentence()); 
    }

    private IEnumerator TypeSentence()
    {
        SetActive(true);

        currentCharacterIndex = 0;

        // �α����� ��� ���ΰ��� �÷��̾� �̸����� ����
        if (Login.currentLoginName != "")
            sentences[currentSentenceIndex] = sentences[currentSentenceIndex].Replace("���ΰ�", Login.currentLoginName);

        isTyping = true;                                    // �ؽ�Ʈ ���� ����
        textDisplay.text = "";                              // �ؽ�Ʈ �ʱ�ȭ
        string sentence = sentences[currentSentenceIndex];  // ����� ���ڿ� ������

        // �̾߱��ϴ� �ִϸ��̼� ����
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Talking"))
            animator.SetTrigger("PlayerTalk");

        // ��� ���ڸ� �����
        while (currentCharacterIndex < sentence.Length)
        {
            // ����� ���ڸ� ������
            char currentChar = sentence[currentCharacterIndex];

            // ����� ���ڸ� ���� ���ڿ��� ����
            textDisplay.text += currentChar;
            currentCharacterIndex++;

            // .���ڰ� �ð�� ����Ű�� ����
            if (currentChar == '.' || currentChar == '~')
            {
                textDisplay.text += '\n';
                currentCharacterIndex++;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        // �ؽ�Ʈ�� ��� ����� ��� ����
        isTyping = false;
    }

    private void SetActive(bool active)
    {
        bubbleTransform.gameObject.SetActive(active);
        textDisplay.gameObject.SetActive(active);
    }

    private IEnumerator NextSentence()
    {
        if (currentSentenceIndex < sentences.Length - 1)
        {
            // ���� ������ �ε����� �Ѿ�� ù�ܾ�� �̵�
            currentSentenceIndex++;
            currentCharacterIndex = 0;

            // ���� ä���� �÷��̾��� ���
            if (playerTalkList.Count > currentPlayerTalkIndex && sentences[currentSentenceIndex] == "")
            {
                // ��ǳ�� ��Ȱ��ȭ
                SetActive(false);

                StartCoroutine(TalkOrReadText.Talk(Login.currentLoginName, playerTalkList[currentPlayerTalkIndex].sentance));

                // �÷��̾� ��ȭ�� ���������� ���
                while (!TalkOrReadText.instance.isUIClose)
                    yield return null;

                currentPlayerTalkIndex++;
            }

            // ���� ���
            else
            {
                SetActive(true);
                StartCoroutine(TypeSentence());
            }
        }

        else
        {
            // �ؽ�Ʈ ��� ��� ���θ� ������ ����
            isEnd = true;
            currentSentenceIndex = 0;
            currentPlayerTalkIndex = 0;
            isTyping = true;

            animator.SetTrigger("isEnd");

            // ��� ������ ��µǸ� �ؽ�Ʈ �̹����� ��Ȱ��ȭ
            SetActive(false);
        }
    }

    private void FinishTyping()
    {
        // Ÿ���� ���� �ؽ�Ʈ�� ��� �Ϸ�
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

[Serializable]
public class PlayerTalkList
{
    public List<string> sentance;
}