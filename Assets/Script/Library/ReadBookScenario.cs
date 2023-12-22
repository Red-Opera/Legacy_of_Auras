using System.Collections;
using System.ComponentModel.Design;
using UnityEngine;

// ���ΰ��� å�� �д� ����� �����ϱ� ���� ��ũ��Ʈ
public class ReadBookScenario : MonoBehaviour
{
    public GameObject book;             // ������ å ������Ʈ
    public Animator animator;          // ������ å �ִϸ�����

    private GameObject player;          // �÷��̾� ������Ʈ
    private TalkOrReadText readText;    // ��ȭâ ������Ʈ
    private TypeStory typeStory;        // å�� ��ġ�� ���� ������Ʈ

    private bool firstRead = false;     // ó������ ���������� å�� �о����� Ȯ���ϴ� ��
    private bool isInit = false;        // å �ִϸ��̼� �ʱ�ȭ ����

    private void Start()
    {
        firstRead = !(bool)PlayerQuest.quest.questList["readBook"];

        player = GameObject.Find("player(Clone)");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");

        readText = player.transform.GetChild(3).GetComponent<TalkOrReadText>();
        Debug.Assert(readText != null, "Error (Null Reference) : ��ȭâ ������Ʈ�� �������� �ʽ��ϴ�.");

        typeStory = GetComponent<TypeStory>();

        animator = book.GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : ������ å�� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
    }

    void Update()
    {
        // å�� ó������ ���� �ִ� ���·� ����
        if (!isInit)
        {
            float progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (progress >= 0.01f)
            {
                isInit = true;
                animator.speed = 0;
            }

            else
                return;
        }

        if (firstRead)
            return;

        if (!(bool)PlayerQuest.quest.questList["readBook"] || (typeStory != null && TypeStory.hasActivatedCanvas))
            return;

        // ó������ �������� �ִ� å�� ���� ��� (����Ʈ�� Ŭ������ ���)
        firstRead = true;

        // �÷��̾� ���� UI ����
        StartCoroutine(Talk());
    }

    // �Ҹ��� �ѱ��ھ� ����ϴ� �޼ҵ�
    private IEnumerator Talk()
    {
        readText.PrintText("Player", "���� �÷��̾��̴�. ~~~~~~");

        while (readText.isPrinting || !(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            yield return null;
        readText.PrintText("Player", "Show me the money");

        while (readText.isPrinting || !(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            yield return null;
        readText.EndChat();
    }
}