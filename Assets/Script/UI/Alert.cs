using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public GameObject alertUI;                              // 알람 UI

    public GameObject getGoldAlert;                         // 골드 알람 오브젝트
    public GameObject getEquipmentAlert;                    // 장비 알람 오브젝트

    public AudioClip alertSound;                            // UI 소리

    public float removeDuration = 0.5f;                     // 알람을 삭제하는데 걸리는 시간
    public float scrollDuration = 0.5f;                     // 알람을 스크롤하는데 걸리는 시간
    public float displayTime = 5.0f;                        // 알람 출력하는 시간
    public int maxAlertCount = 5;                           // 최대 알람 개수

    private Dictionary<GameObject, float> remainAlertTime;  // 해당 오브젝트가 만들어진 시간을 저장하기 위한 변수
    private Queue<GameObject> alerts;                       // 알람을 저장하는 배열
    private AudioSource alertSource;                        // UI 오디오 소스

    public void Start()
    {
        Debug.Assert(alertUI != null, "오류 (Null Reference): 알림 UI가 존재하지 않습니다.");
        Debug.Assert(getGoldAlert != null, "오류 (Null Reference): 골드 획득 알림이 존재하지 않습니다.");
        Debug.Assert(getEquipmentAlert != null, "오류 (Null Reference): 장비 획득 알림이 존재하지 않습니다.");

        alertSource = GetComponent<AudioSource>();
        Debug.Assert(alertSource != null, "오류 (Null Reference): UI의 오디오 소스가 존재하지 않습니다.");

        remainAlertTime = new Dictionary<GameObject, float>();
        alerts = new Queue<GameObject>();

        GameManager.info.alert = this;
    }

    public void Update()
    {
        // 출력시간이 지난 모든 알람을 제거
        foreach (GameObject alert in alerts)
        {
            if (Time.time - remainAlertTime[alert] > displayTime)
            {
                StartCoroutine(DeleteAlert());
                break;
            }
        }
    }

    // 알람을 호출
    public void PushAlert(string content, bool isItem = false)
    {
        // 현재 이미 알림이 5개인 경우 제거
        if (alerts.Count >= maxAlertCount)
            StartCoroutine(DeleteAlert());


        alertSource.PlayOneShot(alertSound);
        StartCoroutine(CreateAlert(content, isItem));
    }

    // 알람을 생성하는 메소드
    private IEnumerator CreateAlert(string content, bool isItem)
    {
        // 새로운 알람을 받는 변수
        GameObject newAlert = null;

        // 아이템 알람인 경우
        if (isItem)
            newAlert = Instantiate(getEquipmentAlert, alertUI.transform.GetChild(0));

        else
            newAlert = Instantiate(getGoldAlert, alertUI.transform.GetChild(0));

        // 출력할 텍스트를 입력함
        newAlert.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = content;

        alerts.Enqueue(newAlert);
        remainAlertTime.Add(newAlert, Time.time);

        // 기존 알람들을 한칸 위로 이동시킴
        StartCoroutine(ScrollAlert());

        float elapsedTime = 0f;

        while (elapsedTime < removeDuration)
        {
            // 크기가 0에서 점점 1로 커지도록 설정
            newAlert.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsedTime / removeDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 확정적으로 크기를 1로 설정
        newAlert.transform.localScale = Vector3.one;
    }

    // 알람을 지우는 메소드
    private IEnumerator DeleteAlert()
    {
        GameObject alert = alerts.Dequeue();
        remainAlertTime.Remove(alert);

        float elapsedTime = 0f;
        Vector3 initialScale = alert.transform.localScale;

        while (elapsedTime < removeDuration)
        {
            alert.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / removeDuration);

            // 지난 시간을 계속 더해줌
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 크기를 0으로 설정하고 알림을 제거함 
        alert.transform.localScale = Vector3.zero;

        Destroy(alert);
    }
    
    // 알람을 스크롤하는 메소드
    private IEnumerator ScrollAlert()
    {
        // 경과 시간 초기화
        float elapsedTime = 0f;
        float alertSize = alerts.Peek().GetComponent<RectTransform>().rect.height;

        while (elapsedTime < scrollDuration)
        {
            int index = alerts.Count - 1;

            // 모든 알람이 이동해야하는 위치를 천천히 이동함
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

        // 모든 알람이 제대로 이동하도록 설정
        int index2 = alerts.Count - 1;
        foreach (GameObject alert in alerts)
            alert.transform.localPosition = new Vector3(alert.transform.localPosition.x, alertSize * index2--, alert.transform.localPosition.z);
    }
}