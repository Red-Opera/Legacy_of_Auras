using System.Collections;
using UnityEngine;

public class BossSceneFilm : MonoBehaviour
{
    public static bool isFilmEnd = true;                        // ��ȭ�� ���� ����

    [SerializeField] private GameObject boss;                   // ���� ���� ������Ʈ
    [SerializeField] private GameObject cameraPos;              // ��ȭ���� ����� ī�޶�
    [SerializeField] private AudioClip openningSound;           // ��ȭ ������ �� ���� �Ҹ�

    [SerializeField] private float delayTargetLookPlayer;       // �÷��̾������� ���Ҷ� ����ϴ� �ð�
    [SerializeField] private float targetMovePlayer;            // �÷��̾������� ���� �� �ð�

    private Animator animator;                          // ���� ���� �ִϸ�����
    private AudioSource audioSource;                    // ����� �ҽ�
    private GameObject player;                          // �÷��̾� ������Ʈ
    private Vector3 openningMovingStart = Vector3.zero; // ������ �� ī�޶��� �ʱ� ��ġ

    private bool isIdle = false;                        // ���� ���� ������ Idle�� ���
    private bool isLookPlayer = false;                  // ī�޶� �÷��̾ ���� �ִ� ���
    private float nextCutRemain = 0.0f;                 // ���� �Ʊ��� ����� �ð�

    void Start()
    {
        Debug.Assert(boss != null, "Error (Null Reference) : ���� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(cameraPos != null, "Error (Null Reference) : ī�޶� �̵��� ��ġ�� �������� �ʽ��ϴ�.");

        animator = boss.GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : ���� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        audioSource = boss.GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "Error (Null Reference) : ���� ����� �ҽ��� �������� �ʽ��ϴ�.");

        transform.position = cameraPos.transform.GetChild(0).position;
        transform.rotation = cameraPos.transform.GetChild(0).rotation;

        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");

        player.GetComponent<PlayerWeaponChanger>().bowEquid = true;

        isFilmEnd = false;
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (isIdle)
            nextCutRemain += Time.deltaTime;

        // ó�� ������ �� ī�޶� ���Ʒ��� ���� ȿ���� ��Ÿ��
        if (state.IsName("Openning") && state.normalizedTime > 0.2f && state.normalizedTime < 0.6f)
        {
            if (openningMovingStart == Vector3.zero)
                openningMovingStart = transform.position;

            transform.position = openningMovingStart + new Vector3(0f, 2 * Mathf.Sin(Time.deltaTime * 2 * Mathf.PI * 60), 0f);
            
            // ������ ������ �Ҹ�
            audioSource.PlayOneShot(openningSound);
        }

        // ���� ��鸮�� ������ ������ �۾������� ����
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

            // ������ ����Ͽ� �ε巴�� �̵�
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

            yield return null;
        }

        // �̵��� �Ϸ�Ǹ� isLookPlayer �÷��׸� true�� ����
        isLookPlayer = true;

        // ���������� �÷��̾� �������� ���ϵ��� ����
        StartCoroutine(FightPlayer());
    }

    // ������ �÷��̾� ī�޶� �µ��� �����ϴ� �޼ҵ�
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