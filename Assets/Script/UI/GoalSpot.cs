using UnityEngine;
using UnityEngine.UI;

public class GoalSpot : MonoBehaviour
{
    [SerializeField] private float amplitude = 1.0f;      // ��� ���� (����)
    [SerializeField] private float frequency = 1.0f;      // ��� ���ļ� (�ݺ� Ƚ��)
    [SerializeField] private float speed = 1.0f;          // �̵� �ӵ�

    [SerializeField] private string whatQuest = "";       // ȭ��ǥ�� �����ϴµ� ��� ������ �ʿ����� �˷��ִ� ����

    private Vector3 initialPosition;    // ó�� ��ġ
    private Image image;                // �����ϰ� �ٲٱ� ���� ����

    void Start()
    {
        image = GetComponent<Image>();
        Debug.Assert(image != null, "Error (Null Reference) : �ִϸ��̼� ������Ʈ�� �������� �ʽ��ϴ�.");

        // ���� ��ġ�� ������
        initialPosition = transform.position;

        // �ش� ����Ʈ�� ����ϰų� �̹� ����Ʈ�� �ƴ� ��� ���� ȭ��ǥ�� ����
        if ((bool)PlayerQuest.quest.questList[whatQuest] || !PlayerQuest.quest.nowQuest.Equals(whatQuest))
        {
            Color toColor = image.color;
            toColor.a = 0.0f;

            image.color = toColor;
        }
    }

    void Update()
    {
        bool isClear = (bool)PlayerQuest.quest.questList[whatQuest];        // �ش� ����Ʈ�� Ŭ�����ߴ��� ����
        bool isThisQuest = PlayerQuest.quest.nowQuest.Equals(whatQuest);    // �̹� ����Ʈ�� �� ����Ʈ���� ����

        // ����Ʈ�� Ŭ�����ϰų� �̹� ����Ʈ�� �ƴϰ� ����Ʈ�� ���� �ִ� ���
        if ((isClear || !isThisQuest) && image.color.a > 0.9f)
        {
            Color toColor = image.color;
            toColor.a = 0.0f;

            image.color = toColor;
        }

        // �̹� ����Ʈ�� �ƴ� ���
        else if (isClear || !isThisQuest)
            return;

        // ����Ʈ�� Ŭ���� ���� �ʾҰ� �̹� ����Ʈ�̸� ���� ����Ʈ�� Ű�� ���� ���
        if (!isClear && isThisQuest && image.color.a < 0.9f)
        {
            Color toColor = image.color;
            toColor.a = 1.0f;

            image.color = toColor;
        }

        // ���� �ð��� ���� y�� ��ġ�� ����
        float newY = initialPosition.y + Mathf.Sin(Time.time * speed * frequency) * amplitude;

        // ���ο� ��ġ�� ����
        Vector3 newPosition = transform.position;
        newPosition.y = newY;

        transform.position = newPosition;
    }
}
