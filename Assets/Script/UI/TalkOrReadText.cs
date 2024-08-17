using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �÷��̾� �Ǵ� ���Ͱ� ��ǳ�� ���� ���� �� ����ϴ� ��ũ��Ʈ
// ���� �ʿ��� �� : UI�� ����ϱ� ���ؼ� PrintText �޼ҵ� ���, UI�� ���ֱ� ���ؼ� EndChat() �޼ҵ� ����
public class TalkOrReadText : MonoBehaviour
{
    public static TalkOrReadText instance;
    public bool isPrinting = false;         // ��ȭ ���� ��� ������ ����
    public bool isUIClose = true;           // UI �ʱ� ���� (����)

    [SerializeField] private GameObject dialog;         // ��ȭ ������Ʈ
    [SerializeField] private Text nameText;             // ��ȭ�� �ϴ� ������Ʈ�� �̸� �ؽ�Ʈ
    [SerializeField] private Text contentText;          // ��ȭ ���� �ؽ�Ʈ
    [SerializeField] private float printSpeed = 0.1f;   // �ؽ�Ʈ ��� �ӵ� (����/��)

    private string printString;             // ����� ���ڿ�
    private string currentContent = "";     // ���� ��� ���� ��ȭ ����
    private int currentContentIndex = 0;    // ��ȭ ���뿡�� ���� ��� ���� ������ �ε���

    private void Start()
    {
        instance = this;

        // ���� ������Ʈ�� �ڽ� ������Ʈ�� ��Ȱ��ȭ
        SetActive(false);
    }

    private void Update()
    {
        // �ؽ�Ʈ ��� ó��
        UpdateTextDisplay();
    }

    // �Ҹ��� �ѱ��ھ� ����ϴ� �޼ҵ�
    public static IEnumerator Talk(string name, List<string> content)
    {
        if (name == "")
            name = "���ΰ�";

        for (int i = 0; i < content.Count; i++)
        {
            instance.PrintText(name, content[i]);

            yield return null;
            bool isPress = false;

            // ��ȭ ���� ��� ���� ���
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

    // UI�� ����ϱ� ���� �޼ҵ�
    private void PrintText(string name, string text)
    {
        isUIClose = false;          // UI�� ������
        SetActive(true);            // UI�� Ŵ

        printString = text;

        nameText.text = name;       // �̸� ����ϴ� �ؽ�Ʈ�� ȿ�� ���� ���
        StartPrintingText(text);    // ����κ��� �ѱ��ھ� ����ϴ� ����Ʈ�ϸ鼭 ���
    }

    // UI�� ���� ���� �޼ҵ�
    private void EndChat()
    {
        // ��ȭ â ��
        SetActive(false);

        isUIClose = true;           // UI ����
        currentContent = "";        // ���� �ʱ�ȭ
        currentContentIndex = 0;

        isPrinting = false;         // ��� ���� ����
        contentText.text = "";      // �ؽ�Ʈ �ʱ�ȭ
    }

    // UI�� Ű�� ���� �޼ҵ�
    private static void SetActive(bool active)
    {
        instance.dialog.SetActive(active);
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