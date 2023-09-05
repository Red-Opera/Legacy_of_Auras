using UnityEngine;

public class NPCLookAtPlayer : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5.0f;
    public float resetLookDelay = 3.0f;
    public static float interactionDistance = 3.0f;

    private Transform originalLookDirection;
    private ChatNPC chatNPC;
    private Animator animator;                  // 애니메이션을 실행할 객체
    private bool isInteracting = false;

    private void Start()
    {
        // 기존 회전 값을 새로 받기 위해서 설정
        originalLookDirection = new GameObject().transform;
        originalLookDirection.rotation = transform.rotation;
        originalLookDirection.position = Vector3.zero;

        chatNPC = GetComponent<ChatNPC>();
        animator = GetComponent<Animator>();

        Debug.Assert(chatNPC != null, "Error (Null Reference) : 해당 객체에 chatNPC가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 애니메이터가 존재하지 않습니다.");
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어와 일정 거리 안에 있고 E 키를 누르면 상호작용 시작
        if (distanceToPlayer < interactionDistance && ChatNPC.isEnd && Input.GetKeyDown(KeyCode.E) && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            ChatNPC.isEnd = false;

            chatNPC.StartChat();
            StartInteraction();
        }

        // 상호작용 중일 때
        if (isInteracting)
        {
            // NPC가 플레이어를 바라보도록 회전
            RotateTowardsPlayer();

            if (ChatNPC.isEnd && isInteracting && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                ResetLookDirection();
        }
    }

    private void StartInteraction()
    {
        isInteracting = true;
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        lookRotation.z = lookRotation.x = 0.0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void ResetLookDirection()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, originalLookDirection.rotation, Time.deltaTime * rotationSpeed);

        if (transform.rotation == originalLookDirection.rotation)
            isInteracting = false;
    }
}
