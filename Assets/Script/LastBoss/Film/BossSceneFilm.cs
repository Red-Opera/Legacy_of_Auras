using System.Collections;
using UnityEngine;

public class BossSceneFilm : MonoBehaviour
{
    public static bool isFilmEnd = true;                        // 영화가 끝난 여부

    [SerializeField] private GameObject boss;                   // 최종 보스 오브젝트
    [SerializeField] private GameObject cameraPos;              // 영화에서 사용할 카메라
    [SerializeField] private AudioClip openningSound;           // 영화 시작할 때 나올 소리

    [SerializeField] private float delayTargetLookPlayer;       // 플레이어쪽으로 향할때 대기하는 시간
    [SerializeField] private float targetMovePlayer;            // 플레이어쪽으로 향할 때 시간

    private Animator animator;                          // 최종 보스 애니메이터
    private AudioSource audioSource;                    // 오디오 소스
    private GameObject player;                          // 플레이어 오브젝트
    private Vector3 openningMovingStart = Vector3.zero; // 시작할 때 카메라의 초기 위치

    private bool isIdle = false;                        // 현재 최종 보스가 Idle인 경우
    private bool isLookPlayer = false;                  // 카메라가 플레이어를 보고 있는 경우
    private float nextCutRemain = 0.0f;                 // 다음 컷까지 대기한 시간

    void Start()
    {
        Debug.Assert(boss != null, "Error (Null Reference) : 보스 오브젝트가 존재하지 않습니다.");
        Debug.Assert(cameraPos != null, "Error (Null Reference) : 카메라가 이동할 위치가 존재하지 않습니다.");

        animator = boss.GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 보스 애니메이터가 존재하지 않습니다.");

        audioSource = boss.GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "Error (Null Reference) : 보스 오디오 소스가 존재하지 않습니다.");

        transform.position = cameraPos.transform.GetChild(0).position;
        transform.rotation = cameraPos.transform.GetChild(0).rotation;

        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");

        player.GetComponent<PlayerWeaponChanger>().bowEquid = true;

        isFilmEnd = false;
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (isIdle)
            nextCutRemain += Time.deltaTime;

        // 처음 시작할 때 카메라를 위아래로 진동 효과를 나타냄
        if (state.IsName("Openning") && state.normalizedTime > 0.2f && state.normalizedTime < 0.6f)
        {
            if (openningMovingStart == Vector3.zero)
                openningMovingStart = transform.position;

            transform.position = openningMovingStart + new Vector3(0f, 2 * Mathf.Sin(Time.deltaTime * 2 * Mathf.PI * 60), 0f);
            
            // 괴성을 지르는 소리
            audioSource.PlayOneShot(openningSound);
        }

        // 점점 흔들리는 진동의 진폭이 작아지도록 설정
        else if (state.IsName("Openning") && state.normalizedTime >= 0.6f && state.normalizedTime <= 1.0f)
            transform.position = openningMovingStart + new Vector3(0f, 2 * Mathf.Sin(Time.deltaTime * 2 * Mathf.PI * (60 - (state.normalizedTime - 0.4f) * 60)), 0f);

        else if (state.IsName("Ready"))
        {
            transform.position = cameraPos.transform.GetChild(1).position;
            transform.rotation = cameraPos.transform.GetChild(1).rotation;
        }

        else if (state.IsName("Idle") && nextCutRemain <= delayTargetLookPlayer)
        {
            transform.position = cameraPos.transform.GetChild(1).position;
            transform.rotation = cameraPos.transform.GetChild(1).rotation;

            isIdle = true;
        }

        else if (nextCutRemain > delayTargetLookPlayer && !isLookPlayer)
            StartCoroutine(LookPlayer());

        else if (nextCutRemain > delayTargetLookPlayer && isLookPlayer && !isFilmEnd)
        {
            transform.position = cameraPos.transform.GetChild(2).position;
            transform.rotation = cameraPos.transform.GetChild(2).rotation;
        }
    }

    private IEnumerator LookPlayer()
    {
        Vector3 targetPosition = cameraPos.transform.GetChild(2).position;
        Quaternion targetRotation = cameraPos.transform.GetChild(2).rotation;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < targetMovePlayer)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Pow(Mathf.Sin((elapsedTime * (Mathf.PI / 2)) / targetMovePlayer), 1 / 2.0f);

            // 보간을 사용하여 부드럽게 이동
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

            yield return null;
        }

        // 이동이 완료되면 isLookPlayer 플래그를 true로 설정
        isLookPlayer = true;

        // 마지막으로 플레이어 시점으로 변하도록 설정
        StartCoroutine(FightPlayer());
    }

    // 마지막 플레이어 카메라에 맞도록 설정하는 메소드
    private IEnumerator FightPlayer()
    {
        GameObject playerCamera = GameObject.Find("Camera");

        yield return new WaitForSeconds(1.0f);

        Vector3 initPosition = transform.position;
        Quaternion initRotation = transform.rotation;

        float elapsedTime = 0.0f;

        while (elapsedTime < 2.0f)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(initPosition, playerCamera.transform.position, elapsedTime);
            transform.rotation = Quaternion.Slerp(initRotation, playerCamera.transform.rotation, elapsedTime);

            yield return null;
        }

        isFilmEnd = true;
        animator.SetBool("isAttackable", true);

        yield return null;

        gameObject.SetActive(false);
    }
}