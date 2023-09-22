using System.Collections;
using UnityEngine;

// ���ΰ��� å�� �д� ����� �����ϱ� ���� ��ũ��Ʈ
public class ReadBookScenario : MonoBehaviour
{
    private GameObject player;          // �÷��̾� ������Ʈ
    private TalkOrReadText readText;    // ��ȭâ ������Ʈ
    private TypeStory typeStory;        // å�� ��ġ�� ���� ������Ʈ
    private bool firstRead = false;     // ó������ ���������� å�� �о����� Ȯ���ϴ� ��

    private void Start()
    {
        player = GameObject.Find("player");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");

        readText = player.transform.GetChild(3).GetComponent<TalkOrReadText>();
        Debug.Assert(readText != null, "Error (Null Reference) : ��ȭâ ������Ʈ�� �������� �ʽ��ϴ�.");

        typeStory = GetComponent<TypeStory>();
    }

    void Update()
    {
        if (firstRead)
            return;

        if (!(bool)PlayerQuest.quest.questList["readBook"] || (typeStory != null && typeStory.hasActivatedCanvas))
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