using UnityEngine;
using UnityEngine.UI;

// �÷��̾� �Ǵ� ���Ͱ� ��ǳ�� ���� ���� �� ����ϴ� ��ũ��Ʈ
// ���� �ʿ��� �� : UI�� ����ϱ� ���ؼ� PrintText �޼ҵ� ���, UI�� ���ֱ� ���ؼ� EndChat() �޼ҵ� ����
public class TalkOrReadText : MonoBehaviour
{
    public Text nameText;                   // ��ȭ�� �ϴ� ������Ʈ�� �̸� �ؽ�Ʈ
    public Text contentText;                // ��ȭ ���� �ؽ�Ʈ
    public float printSpeed = 0.01f;        // �ؽ�Ʈ ��� �ӵ� (����/��)
    public bool isPrinting = false;         // ��ȭ ���� ��� ������ ����

    private string printString;             // ����� ���ڿ�
    private string currentContent = "";     // ���� ��� ���� ��ȭ ����
    private int currentContentIndex = 0;    // ��ȭ ���뿡�� ���� ��� ���� ������ �ε���
    private bool isOff = true;              // UI �ʱ� ���� (����)

    void Start()
    {
        // ���� ������Ʈ�� �ڽ� ������Ʈ�� ��Ȱ��ȭ
        SetChildObjectsActive(false);
    }

    void Update()
    {
        // ��ȭ ���� ��� ���� ���
        if (isPrinting)
        {
            // Space Ű �Ǵ� ���콺 Ŭ������ �ؽ�Ʈ ����� �ٷ� �Ϸ�
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                contentText.text = printString;
        }

        // �ؽ�Ʈ ��� ó��
        UpdateTextDisplay();
    }

    // UI�� ����ϱ� ���� �޼ҵ�
    public void PrintText(string name, string text)
    {
        isOff = false;                      // UI�� ������
        SetChildObjectsActive(true);        // UI�� Ŵ

        printString = text;

        nameText.text = name;               // �̸� ����ϴ� �ؽ�Ʈ�� ȿ�� ���� ���
        StartPrintingText(text);            // ����κ��� �ѱ��ھ� ����ϴ� ����Ʈ�ϸ鼭 ���
    }

    // UI�� ���� ���� �޼ҵ�
    public void EndChat()
    {
        // ���� UI�� �����ִٸ� ����
        if (!isOff)
            SetChildObjectsActive(false);

        isOff = true;               // UI ����
        currentContent = "";        // ���� �ʱ�ȭ
        currentContentIndex = 0;

        isPrinting = false;         // ��� ���� ����
        contentText.text = "";      // �ؽ�Ʈ �ʱ�ȭ
    }

    // UI�� Ű�� ���� �޼ҵ�
    private void SetChildObjectsActive(bool active)
    {
        // �ڽ� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
        foreach (Transform child in transform)
            child.gameObject.SetActive(active);
    }

    // �ؽ�Ʈ�� ����ϱ� ���� �ʱ�ȭ�ϴ� �޼ҵ�
    private void StartPrintingText(string text)
    {
        currentContent = text;
        currentContentIndex = 0;
        isPrinting = true;
    }

    // ������ �ð� �������� ����ϱ� ���� �޼ҵ�
    private void UpdateTextDisplay()
    {
        // ���� ������� �ƴ� ��� ����
        if (!isPrinting)
            return;

        // ��� �ؽ�Ʈ�� ��µ� ������ ���
        if (currentContentIndex < currentContent.Length)
        {
            // �� ���ھ� �ؽ�Ʈ�� �߰�
            int charactersToAdd = Mathf.CeilToInt(printSpeed * Time.deltaTime);
            currentContentIndex += charactersToAdd;
            currentContentIndex = Mathf.Clamp(currentContentIndex, 0, currentContent.Length);

            contentText.text = currentContent.Substring(0, currentContentIndex);
        }

        // �ؽ�Ʈ ����� �Ϸ�� ���
        else
            isPrinting = false;
    }
}