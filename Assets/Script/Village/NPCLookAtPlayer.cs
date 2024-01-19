using UnityEngine;

public class NPCLookAtPlayer : MonoBehaviour
{
    public Transform headTransform;                 // 주민의 머리를 나타내는 Transform
    public float rotationSpeed = 5.0f;              // 플레이어를 바라보는데 걸리는 시간
    public float resetLookDelay = 3.0f;             // 다시 원래대로 돌아가는데 걸리는 시간
    public float interactionDistance = 3.0f;        // 대화하기 위해 필요한 거리
    public bool isSitting = false;                  // 주민이 앉아 있는지 여부를 나타내는 변수

    public float distanceToPlayer;                  // 플레이어와 주민과의 거리

    private Transform player;                       // 플레이어의 위치
    private Transform originalLookDirection;        // 원래 바라보는 위치
    private ChatNPC chatNPC;                        // 대화하기 위한 클래스
    private Animator animator;                      // 애니메이션을 실행할 객체
    private bool isInteracting = false;             // 현재 주민과 상호작용 중인지 확안

    private void Start()
    {
        // 기존 회전 값을 새로 받기 위해서 설정
        originalLookDirection = new GameObject().transform;
        originalLookDirection.position = Vector3.zero;

        player = GameObject.Find("Model").transform;
        chatNPC = GetComponent<ChatNPC>();
        animator = GetComponent<Animator>();

        Debug.Assert(chatNPC != null, "Error (Null Reference) : 해당 객체에 chatNPC가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 애니메이터가 존재하지 않습니다.");

        if (isSitting)
            originalLookDirection.rotation = headTransform.localRotation;

        else
            originalLookDirection.rotation = transform.rotation;
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어와 일정 거리 안에 있고 E 키를 누르면 상호작용 시작
        if (distanceToPlayer < interactionDistance && ChatNPC.isEnd && Input.GetKeyDown(KeyCode.E) && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            ChatNPC.isEnd = false;

            chatNPC.StartChat();        // 플레이어와 대화
            StartInteraction();         // 플레이어와 상호작용하는 여부를 true로 설정
        }

        // 상호작용 중일 때
        if (isInteracting)
        {
            // NPC가 플레이어를 바라보도록 회전
            if (!ChatNPC.isEnd && !isSitting)
                RotateTowardsPlayer();

            // 앉아 있는 NPC인 경우 머리만 돌아감
            else if (!ChatNPC.isEnd)
                RotateHeadTowardsPlayer();

            if (ChatNPC.isEnd && isInteracting && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                ResetLookDirection();
        }
    }

    // 플레이어와 상호작용하도록 하는 메도스 
    private void StartInteraction()
    {
        isInteracting = true;
    }

    // 앉아 있는 NPC인 경우 플레이어와 대화할 때 머리만 돌아가도록 하는 메소드
    private void RotateHeadTowardsPlayer()
    {
        if (headTransform != null)
        {
            Vector3 directionToPlayer = (player.position - headTransform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

            headTransform.localRotation = Quaternion.Slerp(headTransform.localRotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // 서있는 NPC인 경우 플레이어와 대화할 때 몸 전체가 돌아가도록 하는 메소드
    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        lookRotation.z = lookRotation.x = 0.0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // NPC가 플레이어를 바라봤을 경우
    private void ResetLookDirection()
    {
        Quaternion targetRotation;

        if (isSitting)
        {
            targetRotation = originalLookDirection.localRotation;
            headTransform.localRotation = Quaternion.Slerp(headTransform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);

            // 원래 방향으로 회전했을 때 isInteracting을 false로 설정
            if (Quaternion.Angle(headTransform.localRotation, targetRotation) < 0.1f)
                isInteracting = false;
        }

        else
        {
            targetRotation = originalLookDirection.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // 원래 방향으로 회전했을 때 isInteracting을 false로 설정
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
                isInteracting = false;
        }
    }
}