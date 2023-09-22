using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // �� ���� ����� ����ϱ� ���� ���ӽ����̽� �߰�

public class PlayerTeleport : MonoBehaviour
{
    public TextMeshProUGUI textUI;          // �ؽ�Ʈ�� ǥ���� UI �ؽ�Ʈ
    public string[] textToDisplay;          // ����� �ؽ�Ʈ �迭
    public float delayBetweenText = 0.3f;   // �ؽ�Ʈ�� �Ѿ�� �� �ɸ��� �ð� ����

    public GameObject loadingObject;        // Loading ������Ʈ
    public Slider slider;                   // �����̴� ��
    public float animationDuration = 3.0f;  // ���� �� �̵��ϴµ� �ɸ��� �ð�

    public string toScene = "";             // �Ѿ �� �̸�
    public string teleportTargetName = "";  // ��� ������ ã�� TeleportTarget ������Ʈ�� �̸�

    private Coroutine textDisplayCoroutine; // ���� ���� ���� �ڷ�ƾ�� ������ ����
    private int currentTextIndex = 0;       // ���� �ؽ�Ʈ�� ����ϴ� �ε���
    private bool isTeleport = false;        // ���� �ڷ���Ʈ�� �ϰ� �ִ��� ����

    private void Start()
    {
        loadingObject.SetActive(false);     // ó������ �ε� UI�� ��µ��� �ʰ� ����
        slider.value = 0f;                  // �����̴� �� �ʱ�ȭ
    } 

    // �÷��̾ �ڷ���Ʈ�� ������ �� �����ϴ� �޼ҵ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            loadingObject.SetActive(true); // �÷��̾ Ʈ���ſ� �����ϸ� Loading ������Ʈ�� Ȱ��ȭ�մϴ�.
            isTeleport = true;

            // ������ ���� ���� �ؽ�Ʈ ǥ�� �ڷ�ƾ�� �����ϰ� �ٽ� ����
            if (textDisplayCoroutine != null)
                StopCoroutine(textDisplayCoroutine);

            // �����̴� �ڷ�ƾ�� �����ϰ� ���߿� �ߴܽ�Ű�� ���ؼ� ������ ����
            textDisplayCoroutine = StartCoroutine(AnimateSlider());
        }
    }

    // �÷��̾ �ڷ���Ʈ���� ���� ��� �����ϴ� �޼ҵ�
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTeleport = false;
            slider.value = 0f;                  // �����̴� �� �ʱ�ȭ

            loadingObject.SetActive(false);     // �÷��̾ Ʈ���ſ��� ������ Loading ������Ʈ�� ��Ȱ��ȭ�մϴ�.

            // �÷��̾ ���� �� �ؽ�Ʈ ǥ�� �ڷ�ƾ�� ����
            if (textDisplayCoroutine != null)
                StopCoroutine(textDisplayCoroutine);
        }
    }

    // �ε� �����̴��� õõ�� �����ϴ� �ִϸ��̼��� ����ϴ� �޼ҵ�
    private IEnumerator AnimateSlider()
    {
        // �ڷ���Ʈ�� ����� �ð�
        float timer = 0f;

        while (timer < animationDuration)
        {
            slider.value = timer / animationDuration;
            timer += Time.deltaTime;

            yield return null;
        }

        // �ִϸ��̼� ���� �� �����̴��� 1�� ����
        slider.value = 1f;

        // �ؽ�Ʈ ǥ�� �ڷ�ƾ ����
        textDisplayCoroutine = StartCoroutine(DisplayText());

        // �����̴��� 1�� �Ǿ��� �� �� �̵�
        if (slider.value >= 1f)
        {
            // ù��°�� �������� �� ��� ����Ʈ �ذ�
            if (toScene == "Library" && !(bool)PlayerQuest.quest.questList["visitLib"])
                PlayerQuest.quest.NextQuest();

            SceneManager.LoadScene(toScene);    // �ش� ������ �̵���
        }
    }

    // �ݺ������� �ؽ�Ʈ�� ����ϱ� ���� �޼ҵ�
    private IEnumerator DisplayText()
    {
        while (isTeleport) // �÷��̾ Ʈ���� ���ο� ���� ���� �ؽ�Ʈ�� ����մϴ�.
        {
            // �ε��� ��ȣ�� �´� �ؽ�Ʈ�� ����ϰ� ���� �ε����� �Ѿ
            textUI.text = textToDisplay[currentTextIndex];
            currentTextIndex++;

            // �迭�� ���� �����ϸ� ó������ ���ư��ϴ�.
            if (currentTextIndex >= textToDisplay.Length)
                currentTextIndex = 0;

            yield return new WaitForSeconds(delayBetweenText);
        }
    }

    private void MoveToTeleportTarget()
    {
        GameObject targetObject = GameObject.Find(teleportTargetName);  // ��� ������ TeleportTarget ������Ʈ�� ã��
        GameObject player = GameObject.Find("Model");                   // �ڷ���Ʈ�� �÷��̾� ��ü�� ������

        // ã�Ҵ��� Ȯ���ϰ� �÷��̾ �̵���Ŵ
        if (targetObject != null)
            player.transform.position = targetObject.transform.position;

        else
            Debug.Assert(false, "TeleportTarget�� �������� �ʽ��ϴ�.");
    }

    // ���� �ε�Ǹ� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��� ���� �ε�Ǹ� �÷��̾ teleportTarget ��ġ�� �̵�
        MoveToTeleportTarget();
    }

    private void OnEnable()
    {
        // �� �ε� �̺�Ʈ�� �̺�Ʈ �ڵ鷯 ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // �� �ε� �̺�Ʈ���� �̺�Ʈ �ڵ鷯 ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}