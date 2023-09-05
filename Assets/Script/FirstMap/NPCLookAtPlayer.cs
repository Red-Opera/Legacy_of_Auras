using UnityEngine;

public class NPCLookAtPlayer : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5.0f;
    public float resetLookDelay = 3.0f;
    public static float interactionDistance = 3.0f;

    private Transform originalLookDirection;
    private ChatNPC chatNPC;
    private Animator animator;                  // �ִϸ��̼��� ������ ��ü
    private bool isInteracting = false;

    private void Start()
    {
        // ���� ȸ�� ���� ���� �ޱ� ���ؼ� ����
        originalLookDirection = new GameObject().transform;
        originalLookDirection.rotation = transform.rotation;
        originalLookDirection.position = Vector3.zero;

        chatNPC = GetComponent<ChatNPC>();
        animator = GetComponent<Animator>();

        Debug.Assert(chatNPC != null, "Error (Null Reference) : �ش� ��ü�� chatNPC�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �÷��̾�� ���� �Ÿ� �ȿ� �ְ� E Ű�� ������ ��ȣ�ۿ� ����
        if (distanceToPlayer < interactionDistance && ChatNPC.isEnd && Input.GetKeyDown(KeyCode.E) && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            ChatNPC.isEnd = false;

            chatNPC.StartChat();
            StartInteraction();
        }

        // ��ȣ�ۿ� ���� ��
        if (isInteracting)
        {
            // NPC�� �÷��̾ �ٶ󺸵��� ȸ��
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
