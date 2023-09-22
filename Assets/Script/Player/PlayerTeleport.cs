using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 관리 기능을 사용하기 위한 네임스페이스 추가

public class PlayerTeleport : MonoBehaviour
{
    public TextMeshProUGUI textUI;          // 텍스트를 표시할 UI 텍스트
    public string[] textToDisplay;          // 출력할 텍스트 배열
    public float delayBetweenText = 0.3f;   // 텍스트가 넘어가는 데 걸리는 시간 간격

    public GameObject loadingObject;        // Loading 오브젝트
    public Slider slider;                   // 슬라이더 바
    public float animationDuration = 3.0f;  // 다음 맵 이동하는데 걸리는 시간

    public string toScene = "";             // 넘어갈 씬 이름
    public string teleportTargetName = "";  // 대상 씬에서 찾을 TeleportTarget 오브젝트의 이름

    private Coroutine textDisplayCoroutine; // 현재 실행 중인 코루틴을 저장할 변수
    private int currentTextIndex = 0;       // 현재 텍스트를 출력하는 인덱스
    private bool isTeleport = false;        // 현재 텔레포트를 하고 있는지 여부

    private void Start()
    {
        loadingObject.SetActive(false);     // 처음에는 로딩 UI가 출력되지 않게 설정
        slider.value = 0f;                  // 슬라이더 바 초기화
    } 

    // 플레이어가 텔레포트에 들어왔을 때 실행하는 메소드
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            loadingObject.SetActive(true); // 플레이어가 트리거에 진입하면 Loading 오브젝트를 활성화합니다.
            isTeleport = true;

            // 기존에 실행 중인 텍스트 표시 코루틴을 중지하고 다시 시작
            if (textDisplayCoroutine != null)
                StopCoroutine(textDisplayCoroutine);

            // 슬라이더 코루틴을 시작하고 나중에 중단시키기 위해서 변수로 받음
            textDisplayCoroutine = StartCoroutine(AnimateSlider());
        }
    }

    // 플레이어가 텔레포트에서 나간 경우 실행하는 메소드
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTeleport = false;
            slider.value = 0f;                  // 슬라이더 바 초기화

            loadingObject.SetActive(false);     // 플레이어가 트리거에서 나가면 Loading 오브젝트를 비활성화합니다.

            // 플레이어가 나갈 때 텍스트 표시 코루틴을 중지
            if (textDisplayCoroutine != null)
                StopCoroutine(textDisplayCoroutine);
        }
    }

    // 로딩 슬라이더가 천천히 증가하는 애니메이션을 출력하는 메소드
    private IEnumerator AnimateSlider()
    {
        // 텔레포트에 대기한 시간
        float timer = 0f;

        while (timer < animationDuration)
        {
            slider.value = timer / animationDuration;
            timer += Time.deltaTime;

            yield return null;
        }

        // 애니메이션 종료 후 슬라이더를 1로 설정
        slider.value = 1f;

        // 텍스트 표시 코루틴 시작
        textDisplayCoroutine = StartCoroutine(DisplayText());

        // 슬라이더가 1이 되었을 때 씬 이동
        if (slider.value >= 1f)
        {
            // 첫번째로 도서관에 간 경우 퀘스트 해결
            if (toScene == "Library" && !(bool)PlayerQuest.quest.questList["visitLib"])
                PlayerQuest.quest.NextQuest();

            SceneManager.LoadScene(toScene);    // 해당 씬으로 이동함
        }
    }

    // 반복적으로 텍스트가 출력하기 위한 메소드
    private IEnumerator DisplayText()
    {
        while (isTeleport) // 플레이어가 트리거 내부에 있을 때만 텍스트를 출력합니다.
        {
            // 인덱스 번호에 맞는 텍스트를 출력하고 다음 인덱스로 넘어감
            textUI.text = textToDisplay[currentTextIndex];
            currentTextIndex++;

            // 배열의 끝에 도달하면 처음으로 돌아갑니다.
            if (currentTextIndex >= textToDisplay.Length)
                currentTextIndex = 0;

            yield return new WaitForSeconds(delayBetweenText);
        }
    }

    private void MoveToTeleportTarget()
    {
        GameObject targetObject = GameObject.Find(teleportTargetName);  // 대상 씬에서 TeleportTarget 오브젝트를 찾음
        GameObject player = GameObject.Find("Model");                   // 텔레포트할 플레이어 객체를 가져옴

        // 찾았는지 확인하고 플레이어를 이동시킴
        if (targetObject != null)
            player.transform.position = targetObject.transform.position;

        else
            Debug.Assert(false, "TeleportTarget이 존재하지 않습니다.");
    }

    // 씬이 로드되면 호출되는 이벤트 핸들러
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 대상 씬이 로드되면 플레이어를 teleportTarget 위치로 이동
        MoveToTeleportTarget();
    }

    private void OnEnable()
    {
        // 씬 로드 이벤트에 이벤트 핸들러 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트에서 이벤트 핸들러 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}