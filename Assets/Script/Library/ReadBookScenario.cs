using UnityEngine;

// ���ΰ��� å�� �д� ����� �����ϱ� ���� ��ũ��Ʈ
public class ReadBookScenario : MonoBehaviour
{
    public GameObject book;             // ������ å ������Ʈ
    public Animator animator;           // ������ å �ִϸ�����

    private GameObject player;          // �÷��̾� ������Ʈ
    private TypeStory typeStory;        // å�� ��ġ�� ���� ������Ʈ

    private bool firstRead = false;     // ó������ ���������� å�� �о����� Ȯ���ϴ� ��
    private bool isInit = false;        // å �ִϸ��̼� �ʱ�ȭ ����

    private void Start()
    {
        firstRead = !(bool)PlayerQuest.quest.questList["readBook"];

        player = GameObject.Find("player(Clone)");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");

        typeStory = GetComponent<TypeStory>();

        animator = book.GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : ������ å�� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
    }

    private void Update()
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
    }
}