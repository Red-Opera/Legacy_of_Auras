using System.Collections;
using TMPro;
using UnityEngine;

public class ChatNPC : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;         // �ؽ�Ʈ�� ����� ��ü
    public string[] sentences;                  // �ؽ�Ʈ�� ����� ����
    public float typingSpeed = 0.1f;            // ��� �ӵ� (���� : ��)
    public static bool isEnd = true;            // �ؽ�Ʈ ��� ����

    private GameObject player;                  // �÷��̾� ������Ʈ
    private NPCLookAtPlayer npcLookAtPlayer;    // �÷��̾ �����ϴ� Ŭ����
    private int currentSentenceIndex = 0;       // ���� ��� ���� ����
    private int currentCharacterIndex = 0;      // ���� ��� ���� �ܾ�
    private bool isTyping = false;              // ��� ����

    private Animator animator;                  // �ִϸ��̼��� ������ ��ü

    private void Start()
    {
        animator = GetComponent<Animator>();
        npcLookAtPlayer = GetComponent<NPCLookAtPlayer>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
        Debug.Assert(npcLookAtPlayer != null, "Error (Null Reference) : �÷��̾ �����ϴ� ��ũ��Ʈ�� �������� �ʽ��ϴ�.");

        player = GameObject.Find("Model");

        textDisplay.transform.gameObject.transform.parent.gameObject.SetActive(false);
        textDisplay.gameObject.SetActive(false);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (isEnd || distanceToPlayer > npcLookAtPlayer.interactionDistance)
            return;

        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ�� ����
        {
            if (!isTyping)
                NextSentence();

            // Ÿ���� �߿� Ŭ���ϸ� ������ �ٷ� ����մϴ�.
            else
                FinishTyping();
        }
    }

    public void StartChat() 
    {
        // �������� �ִ� NPC�� ��ȭ ����Ʈ�� ����� ���
        if (gameObject.name.Equals("LibWoman") && !(bool)PlayerQuest.quest.questList["chatLibWoman"])
            PlayerQuest.quest.NextQuest();

        // ������ �ִ� NPC�� ��ȭ ����Ʈ�� ����� ���
        if (gameObject.name.Equals("NPC") && !(bool)PlayerQuest.quest.questList["chatNPC"])
                PlayerQuest.quest.NextQuest();

        StartCoroutine(TypeSentence()); 
    }

    public IEnumerator TypeSentence()
    {
        textDisplay.gameObject.SetActive(true);
        textDisplay.transform.gameObject.transform.parent.gameObject.SetActive(true);

        currentCharacterIndex = 0;

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
            if (currentChar == '.')
            {
                textDisplay.text += '\n';
                currentCharacterIndex++;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        // �ؽ�Ʈ�� ��� ����� ��� ����
        isTyping = false;
    }

    private void NextSentence()
    {
        if (currentSentenceIndex < sentences.Length - 1)
        {
            // ���� ������ �ε����� �Ѿ�� ù�ܾ�� �̵�
            currentSentenceIndex++;
            currentCharacterIndex = 0;

            // ���� ���
            StartCoroutine(TypeSentence());
        }

        else
        {
            // �ؽ�Ʈ ��� ��� ���θ� ������ ����
            isEnd = true;
            currentSentenceIndex = 0;

            animator.SetTrigger("isEnd");

            // ��� ������ ��µǸ� �ؽ�Ʈ �̹����� ��Ȱ��ȭ
            textDisplay.transform.gameObject.transform.parent.gameObject.SetActive(false);
            textDisplay.gameObject.SetActive(false);
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