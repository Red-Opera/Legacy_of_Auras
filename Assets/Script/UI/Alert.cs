using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public GameObject alertUI;                              // �˶� UI

    public GameObject getGoldAlert;                         // ��� �˶� ������Ʈ
    public GameObject getEquipmentAlert;                    // ��� �˶� ������Ʈ

    public AudioClip alertSound;                            // UI �Ҹ�

    public float removeDuration = 0.5f;                     // �˶��� �����ϴµ� �ɸ��� �ð�
    public float scrollDuration = 0.5f;                     // �˶��� ��ũ���ϴµ� �ɸ��� �ð�
    public float displayTime = 5.0f;                        // �˶� ����ϴ� �ð�
    public int maxAlertCount = 5;                           // �ִ� �˶� ����

    private Dictionary<GameObject, float> remainAlertTime;  // �ش� ������Ʈ�� ������� �ð��� �����ϱ� ���� ����
    private Queue<GameObject> alerts;                       // �˶��� �����ϴ� �迭
    private AudioSource alertSource;                        // UI ����� �ҽ�

    public void Start()
    {
        Debug.Assert(alertUI != null, "���� (Null Reference): �˸� UI�� �������� �ʽ��ϴ�.");
        Debug.Assert(getGoldAlert != null, "���� (Null Reference): ��� ȹ�� �˸��� �������� �ʽ��ϴ�.");
        Debug.Assert(getEquipmentAlert != null, "���� (Null Reference): ��� ȹ�� �˸��� �������� �ʽ��ϴ�.");

        alertSource = GetComponent<AudioSource>();
        Debug.Assert(alertSource != null, "���� (Null Reference): UI�� ����� �ҽ��� �������� �ʽ��ϴ�.");

        remainAlertTime = new Dictionary<GameObject, float>();
        alerts = new Queue<GameObject>();

        GameManager.info.alert = this;
    }

    public void Update()
    {
        // ��½ð��� ���� ��� �˶��� ����
        foreach (GameObject alert in alerts)
        {
            if (Time.time - remainAlertTime[alert] > displayTime)
            {
                StartCoroutine(DeleteAlert());
                break;
            }
        }
    }

    // �˶��� ȣ��
    public void PushAlert(string content, bool isItem = false)
    {
        // ���� �̹� �˸��� 5���� ��� ����
        if (alerts.Count >= maxAlertCount)
            StartCoroutine(DeleteAlert());


        alertSource.PlayOneShot(alertSound);
        StartCoroutine(CreateAlert(content, isItem));
    }

    // �˶��� �����ϴ� �޼ҵ�
    private IEnumerator CreateAlert(string content, bool isItem)
    {
        // ���ο� �˶��� �޴� ����
        GameObject newAlert = null;

        // ������ �˶��� ���
        if (isItem)
            newAlert = Instantiate(getEquipmentAlert, alertUI.transform.GetChild(0));

        else
            newAlert = Instantiate(getGoldAlert, alertUI.transform.GetChild(0));

        // ����� �ؽ�Ʈ�� �Է���
        newAlert.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = content;

        alerts.Enqueue(newAlert);
        remainAlertTime.Add(newAlert, Time.time);

        // ���� �˶����� ��ĭ ���� �̵���Ŵ
        StartCoroutine(ScrollAlert());

        float elapsedTime = 0f;

        while (elapsedTime < removeDuration)
        {
            // ũ�Ⱑ 0���� ���� 1�� Ŀ������ ����
            newAlert.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsedTime / removeDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ȯ�������� ũ�⸦ 1�� ����
        newAlert.transform.localScale = Vector3.one;
    }

    // �˶��� ����� �޼ҵ�
    private IEnumerator DeleteAlert()
    {
        GameObject alert = alerts.Dequeue();
        remainAlertTime.Remove(alert);

        float elapsedTime = 0f;
        Vector3 initialScale = alert.transform.localScale;

        while (elapsedTime < removeDuration)
        {
            alert.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / removeDuration);

            // ���� �ð��� ��� ������
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ũ�⸦ 0���� �����ϰ� �˸��� ������ 
        alert.transform.localScale = Vector3.zero;

        Destroy(alert);
    }
    
    // �˶��� ��ũ���ϴ� �޼ҵ�
    private IEnumerator ScrollAlert()
    {
        // ��� �ð� �ʱ�ȭ
        float elapsedTime = 0f;
        float alertSize = alerts.Peek().GetComponent<RectTransform>().rect.height;

        while (elapsedTime < scrollDuration)
        {
            int index = alerts.Count - 1;

            // ��� �˶��� �̵��ؾ��ϴ� ��ġ�� õõ�� �̵���
            foreach (GameObject alert in alerts)
            {
                float normalizedTime = elapsedTime / scrollDuration;
                float angle = normalizedTime * Mathf.PI * 0.5f;
                float targetY = Mathf.Sin(angle) * alertSize * index;

                alert.transform.localPosition = new Vector3(alert.transform.localPosition.x, targetY, alert.transform.localPosition.z);

                elapsedTime += Time.deltaTime;
                index--;
            }

            yield return null;
        }

        // ��� �˶��� ����� �̵��ϵ��� ����
        int index2 = alerts.Count - 1;
        foreach (GameObject alert in alerts)
            alert.transform.localPosition = new Vector3(alert.transform.localPosition.x, alertSize * index2--, alert.transform.localPosition.z);
    }
}