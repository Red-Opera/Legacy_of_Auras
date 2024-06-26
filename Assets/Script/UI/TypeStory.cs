using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypeStory : MonoBehaviour
{
    public GameObject lookAtTarget;             // 바라볼 대상
    public GameObject canvas;                   // 책 UI
    public GameObject endUI;                    // 끝나면 나타날 UI
    public float endUIMoveDistance = 10.0f;     // endUI의 이동 거리
    public float endUISinSpeed = 2.0f;          // endUI 이동 속도

    public Text textComponent;                      // 출력하기 위한 텍스트 컴포넌트
    public string[] sentences;                      // 텍스트를 출력할 문장 배열
    public float typeSpeed = 0.1f;                  // 텍스트를 출력하는 속도 (초 단위)
    public static bool hasActivatedCanvas = false;  // canvas가 활성화되었는지 여부

    public AudioClip openBookSound;             // 책 여는 소리
    public AudioClip closeBookSound;            // 책 닫는 소리

    public float rotationSpeed = 0.5f;          // 회전하는데 걸리는 시간
    private Vector2 startRotation;              // 회전 시작 시의 플레이어의 회전
    private Vector2 endRotation;                // 목표 회전
    private float rotationStartTime;            // 회전 시작 시간
    private bool isRotating = false;            // 회전 중인지 여부

    private int currentSentenceIndex = 0;       // 현재 출력 중인 문장의 인덱스
    private bool isTyping = false;              // 출력 중인지 여부

    private GameObject player;                  // 플레이어의 오브젝트
    private GameObject camera;                  // 플레이어의 카메라
    private bool isLookingAtTarget = false;     // 바라보고 있는지 여부

    private Vector3 endUIOriginalPosition;      // endUI의 초기 위치
    private float endUIStartTime;               // endUI 이동 시작 시간

    private ReadBookScenario scenario;          // 시나리오 스크립트
    private bool isScenarioStarting = false;    // 현재 책이 펴지고 있는지 확인

    private AudioSource audioSource;            // 스피커

    void Start()
    {
        player = GameObject.Find("Model");
        camera = GameObject.Find("MainCamera");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");

        endUIOriginalPosition = endUI.transform.position;

        endUI.SetActive(false);
        canvas.SetActive(false);

        scenario = GetComponent<ReadBookScenario>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // E 키를 누르면 lookAtTarget을 바라보도록 설정
        if (isLookingAtTarget && Input.GetKeyDown(KeyCode.E))
            if (!isScenarioStarting)
                StartCoroutine(IsStartStory());

        // canvas가 활성화되었다면 타이핑 시작
        if (hasActivatedCanvas)
        {
            // 문장이 모두 출력되지 않았고, 출력 중이 아니라면 타이핑을 시작합니다.
            if (currentSentenceIndex < sentences.Length && !isTyping)
            {
                if (currentSentenceIndex == 0)
                    StartTyping();

                else if (currentSentenceIndex != 0 && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space)))
                    StartTyping();
            }
        }

        if (!isTyping && Input.GetKeyDown(KeyCode.E))
        {
            // 이미 활성화된 경우, E 키를 누르면 UI를 다시 닫음
            hasActivatedCanvas = false;
            canvas.SetActive(false);
            endUI.SetActive(false);
        }

        // endUI 이동 처리
        if (!isTyping && canvas.activeSelf)
        {
            if (!endUI.activeSelf)
                endUI.SetActive(true);

            float t = (Time.time - endUIStartTime) * endUISinSpeed;
            float yOffset = Mathf.Sin(t) * endUIMoveDistance;
            endUI.transform.position = endUIOriginalPosition + new Vector3(0, yOffset, 0);
        }

        else if (isTyping && canvas.activeSelf)
        {
            if (endUI.activeSelf)
                endUI.SetActive(false);
        }
    }

    private IEnumerator IsStartStory()
    {
        isScenarioStarting = true;

        if (!hasActivatedCanvas)
        {
            currentSentenceIndex = 0;   // 문장 인덱스 초기화

            if (scenario != null)
            {
                scenario.animator.speed = 2;
                scenario.animator.SetTrigger("Exit");
                scenario.animator.Play("BookOpen", 0, 0.0f);
                scenario.animator.SetFloat("Speed", 1.0f);
                yield return new WaitForSeconds(0.1f);

                if (audioSource != null)
                    audioSource.PlayOneShot(openBookSound);
                
                while (scenario.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
                    yield return null;

                scenario.animator.speed = 0;
            }

            yield return null;

            hasActivatedCanvas = true;  // canvas가 활성화되었다고 표시
            canvas.SetActive(true);

            if (!(bool)PlayerQuest.quest.questList["readBook"])
            {
                GameManager.info.alert.PushAlert("\"책 읽기\" 퀘스트 클리어!", true);
                PlayerQuest.quest.NextQuest();
            }
        }

        LookAtTarget();

        player.GetComponent<Animator>().SetBool("isWalk", false);

        isScenarioStarting = false;
    }

    // 텍스트를 한 글자씩 출력하는 함수
    void StartTyping()
    {
        isTyping = true;
        textComponent.text = ""; // 텍스트 초기화

        StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
    }

    // 문장을 한 글자씩 출력하는 코루틴
    IEnumerator TypeSentence(string sentence)
    {
        for (int i = 0; i < sentence.Length; i++)
        {
            char letter = sentence[i];

            if (letter != ' ' || sentence[i - 1] != '.')
                textComponent.text += letter; // 한 글자씩 텍스트에 추가

            if (letter == '.')
                textComponent.text += '\n';

            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;           // 출력이 끝났음을 표시

        // 모든 글자가 출력되었다면 다음 문장으로 이동
        currentSentenceIndex++;

        // 모든 문장이 출력되었다면 게임 종료 또는 원하는 동작 수행
        if (currentSentenceIndex >= sentences.Length)
        {
            // 여기에서 원하는 동작을 수행하거나 게임 종료 등의 작업을 할 수 있습니다.
        }
    }

    // 플레이어와 충돌시 호출되는 함수
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했을 때, E 키를 누르면 lookAtTarget을 바라보도록 설정
        if (other.CompareTag("Player"))
            isLookingAtTarget = true;
    }

    // 플레이어와 충돌 종료시 호출되는 함수
    private void OnTriggerExit(Collider other)
    {
        // 플레이어와 충돌 종료시, E 키를 눌러도 더이상 바라보지 않도록 설정
        if (other.CompareTag("Player"))
            isLookingAtTarget = false;
    }

    // 마우스 왼쪽 클릭을 통해 UI를 닫을 수 있도록 합니다.
    void LateUpdate()
    {
        if (hasActivatedCanvas && !isTyping && currentSentenceIndex == sentences.Length && Input.GetMouseButtonDown(0))
        {
            hasActivatedCanvas = false;
            canvas.SetActive(false);
            endUI.SetActive(false);

            StartCoroutine(CloseBook());
        }
    }

    // lookAtTarget을 바라보도록 설정
    void LookAtTarget()
    {
        if (!isRotating)
        {
            PlayerRotate playerRotate = player.GetComponent<PlayerRotate>();
            startRotation = playerRotate.currentRotation;
            endRotation = Vector2.zero; // (0, 0)으로 회전
            rotationStartTime = Time.time;
            StartCoroutine(RotatePlayer(playerRotate));
        }
    }

    // RotatePlayer 코루틴 수정
    IEnumerator RotatePlayer(PlayerRotate playerRotate)
    {
        isRotating = true;

        while (Time.time - rotationStartTime < rotationSpeed)
        {
            float journeyTime = Time.time - rotationStartTime;
            float fractionOfJourney = Mathf.Clamp01(journeyTime / rotationSpeed);

            playerRotate.currentRotation = Vector2.Lerp(startRotation, endRotation + new Vector2(-90, 0), fractionOfJourney);

            // 카메라의 회전을 수정 (위아래 방향은 움직이지 않도록 함)
            Vector3 cameraEulerAngles = camera.transform.rotation.eulerAngles;
            camera.transform.rotation = Quaternion.Euler(0, cameraEulerAngles.y, cameraEulerAngles.z);

            yield return null;
        }

        playerRotate.currentRotation = endRotation + new Vector2(-90, 0); // 목표 회전값으로 설정
        isRotating = false;
    }

    // 책을 닫는 이펙트 메소드
    private IEnumerator CloseBook()
    {
        if (scenario != null)
        {
            scenario.animator.speed = 2;
            scenario.animator.SetFloat("Speed", -1.0f);
            scenario.animator.Play("BookOpen", 0, 0.0f);
            scenario.animator.SetTrigger("Exit");
            yield return new WaitForSeconds(0.1f);

            if (audioSource != null)
                audioSource.PlayOneShot(closeBookSound);

            while (scenario.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
                yield return null;

            scenario.animator.speed = 0;
        }
    }
}