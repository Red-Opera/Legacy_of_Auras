using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerTeleport : MonoBehaviour
{
    public TextMeshProUGUI textUI;              // �ؽ�Ʈ�� ǥ���� UI �ؽ�Ʈ
    public string[] textToDisplay;              // ����� �ؽ�Ʈ �迭
    public float delayBetweenText = 0.3f;       // �ؽ�Ʈ�� �Ѿ�� �� �ɸ��� �ð� ����

    public GameObject loadingObject;            // Loading ������Ʈ
    public Slider slider;                       // �����̴� ��
    public float animationDuration = 3.0f;      // ���� �� �̵��ϴµ� �ɸ��� �ð�
    private static float coolTime = 1.0f;       // ���� �ڷ���Ʈ���� �ɸ��� �ð�
    private static float currentCoolTime = 1.0f;// ���� ��Ÿ��

    public string toScene = "";                 // �Ѿ �� �̸�

    private Coroutine textDisplayCoroutine;     // ���� ���� ���� �ڷ�ƾ�� ������ ����
    private int currentTextIndex = 0;           // ���� �ؽ�Ʈ�� ����ϴ� �ε���
    private bool isTeleport = false;            // ���� �ڷ���Ʈ�� �ϰ� �ִ��� ����

    private GameObject targetObject = null;     // ��� ������ TeleportTarget ������Ʈ�� ã��
    private bool isEnter = false;               // ���� �ڷ���Ʈ �ȿ� �ִ��� Ȯ��

    private void Start()
    {
        loadingObject.SetActive(false);         // ó������ �ε� UI�� ��µ��� �ʰ� ����
        slider.value = 0f;                      // �����̴� �� �ʱ�ȭ

        currentCoolTime = coolTime;

        OnEnable();
    }

    private void Update()
    {
        if (currentCoolTime > 0)
            currentCoolTime -= Time.deltaTime;
    }

    // �÷��̾ �ڷ���Ʈ�� ������ �� �����ϴ� �޼ҵ�
    private void OnTriggerEnter(Collider other)
    {
        isEnter = true;

        if (other.CompareTag("Player"))
        {
            loadingObject.SetActive(true); // �÷��̾ Ʈ���ſ� �����ϸ� Loading ������Ʈ�� Ȱ��ȭ�մϴ�.
            isTeleport = true;

            // ������ ���� ���� �ؽ�Ʈ ǥ�� �ڷ�ƾ�� �����ϰ� �ٽ� ����
            if (textDisplayCoroutine != null)
            {
                StopCoroutine(textDisplayCoroutine);
                textDisplayCoroutine = null;
            }

            // �����̴� �ڷ�ƾ�� �����ϰ� ���߿� �ߴܽ�Ű�� ���ؼ� ������ ����
            textDisplayCoroutine = StartCoroutine(AnimateSlider());
        }
    }

    // �÷��̾ �ڷ���Ʈ���� ���� ��� �����ϴ� �޼ҵ�
    private void OnTriggerExit(Collider other)
    {
        isEnter = false;

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
        if (slider.value >= 1f && currentCoolTime <= 0)
        {
            // ù��°�� �������� �� ��� ����Ʈ �ذ�
            if (toScene == "Library" && !(bool)PlayerQuest.quest.questList["visitLib"])
            {
                GameManager.info.alert.PushAlert("\"������ �湮\" ����Ʈ Ŭ����!", true);
                PlayerQuest.quest.NextQuest();
            }

            slider.value = 0;

            currentCoolTime = coolTime;
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

    private void MoveToTeleportTarget(Scene scene)
    {
        GameObject player = GameObject.Find("Model");

        if (scene.name.Equals("Village"))
        {
            if (GameManager.info.beforeSceneName.Equals("Prologue"))
                targetObject = GameObject.Find("StartLocation");

            else if (GameManager.info.beforeSceneName.Equals("Library"))
                targetObject = GameObject.Find("LibExitLocation");

            else if (GameManager.info.beforeSceneName.Equals("Village") && gameObject.name == "InStoreTele" && isEnter)
                targetObject = GameObject.Find("OutStoreExit");

            else if (GameManager.info.beforeSceneName.Equals("Village") && gameObject.name == "OutStoreTele" && isEnter)
                targetObject = GameObject.Find("InStoreExit");
        }

        else if (scene.name.Equals("Library"))
            targetObject = GameObject.Find("StartLocation");

        else if (scene.name.Equals("Desert"))
        {
            if (GameManager.info.beforeSceneName.Equals("Village"))
                targetObject = GameObject.Find("StartLocation");

            else if (GameManager.info.beforeSceneName.Equals("Forest"))
                targetObject = GameObject.Find("ForestExit");
        }

        else if (scene.name.Equals("Forest"))
        {
            if (GameManager.info.beforeSceneName.Equals("Desert"))
                targetObject = GameObject.Find("StartLocation");

            else if (GameManager.info.beforeSceneName.Equals("Final"))
                targetObject = GameObject.Find("FinalExit");
        }

        else if (scene.name.Equals("Final"))
        {
            if (GameManager.info.beforeSceneName.Equals("Forest"))
                targetObject = GameObject.Find("StartLocation");
        }    

        if (scene.name.Equals("Forest"))
            GameObject.Find("Model").transform.localScale = new Vector3(30.0f, 30.0f, 30.0f);

        else
            GameObject.Find("Model").transform.localScale = new Vector3(75.0f, 75.0f, 75.0f);

        if (player != null && targetObject != null)
        {
            player.transform.position = targetObject.transform.position;
            player.transform.rotation = targetObject.transform.rotation;
        }

        GameManager.info.beforeSceneName = scene.name;
    }

    // ���� �ε�Ǹ� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��� ���� �ε�Ǹ� �÷��̾ teleportTarget ��ġ�� �̵�
        MoveToTeleportTarget(scene);
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