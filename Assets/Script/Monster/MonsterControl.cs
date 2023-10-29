using System.Collections;
using UnityEngine;

// ������ ���¸� �ٲٱ� ���� ��ũ��Ʈ
public class MonsterControl : MonoBehaviour
{
    public float lookAtDis = 25.0f;             // �÷��̾ �ν��� �� �ִ� �Ÿ�
    public float lookAtAngle = 120.0f;          // ���Ͱ� �ٶ󺸴� ���� ����
    public float attackDistance = 5.0f;         // ���� ������ �Ÿ�
    public float moveSpeed = 2.0f;              // ������ �̵� �ӵ�
    public float rotationSpeed = 5.0f;          // ������ ȸ�� �ӵ�
    public float waitMinTime = 1.0f;            // ���� �� �ּ� ���ð�
    public float waitMaxTime = 5.0f;            // ���� �� �ִ� ���ð�
    public int maxAttackType = 2;               // ���� Ÿ���� ����
    public int idleTurnAngleMin = -120;         // Idle ���¿��� ȸ���ϴ� ������ �ּ� �� (���� : ��)
    public int idleTurnAngleMax = 120;          // Idle ���¿��� ȸ���ϴ� ������ �ִ� �� (���� : ��)
    public int idleTurnAngleCurrent = 0;        // ���� Idle ���¿��� ȸ���ϴ� ����
    public float idleTurnWaitTimeMin = 5.0f;    // Idle ���¿��� ȸ���ϱ� ���� ����ؾ� �ϴ� �ּ� �ð�
    public float idleTurnWaitTimeMax = 15.0f;   // Idle ���¿��� ȸ���ϱ� ���� ����ؾ� �ϴ� �ִ� �ð�

    private GameObject player;              // �÷��̾� ������Ʈ
    private Animator animator;              // �� ������Ʈ�� �ִϸ�����

    private bool isAttacking = false;       // ���� ������ ���θ� ��Ÿ���� ����

    public void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference): �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference): �÷��̾ �������� �ʽ��ϴ�.");

        StartCoroutine(AttackCoroutine());          // ������ �� ���� �� ���ð��� ���� ������ �����ϱ� ���� �޼ҵ�
        StartCoroutine(IdleRotationCoroutine());    // Idle ���¿��� ������ �ð��� ������ ���� ���� �޼ҵ�
    }

    public void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            return;

        // �÷��̾�� ������ ��ġ�� ����
        Vector3 playerPosition = player.transform.position;
        Vector3 monsterPosition = transform.position;

        Vector3 directionToPlayer = playerPosition - monsterPosition;               // ���Ͱ� �÷��̾ �ٶ󺸴� ������ ����
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);  // ���Ͱ� �÷��̾ �ٶ󺸴� ������ ����մϴ�.

        // ���Ͱ� �÷��̾ �ٶ󺸴� ������ lookAtAngle �̳��̰�, �÷��̾�� ���� ������ �Ÿ��� lookAtDis �̳���� ���� ���·� �Ѿ
        if ((angleToPlayer <= lookAtAngle * 0.5f || isAttacking) && directionToPlayer.magnitude <= lookAtDis)
        {
            animator.SetBool("IsFindPlayer", true);

            // ���Ϳ� �÷��̾� ������ �Ÿ��� ���� ������ �Ÿ����� ������ AttackAble�� true�� ����
            if (directionToPlayer.magnitude <= attackDistance && angleToPlayer <= lookAtAngle * 0.5f)
            {
                // ������ �� �ִ� ���·� ���Ͽ� �÷��̾ �����ϵ��� ����
                animator.SetBool("AttackAble", true);
                isAttacking = true;
            }

            // �÷��̾ ������ �� ���� ���°� �� ���
            else
            {
                // �÷��̾ �������� ���ϵ��� ����
                animator.SetBool("AttackAble", false);

                // ���� ���� ���̾��� ���� ���� ��� ���� �� �÷��̾ ������ ����
                if (isAttacking || animator.GetCurrentAnimatorStateInfo(0).IsName("AttackWait"))
                {
                    // �÷��̾ ���� ȸ��
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

                    // �÷��̾ ���� ���� �̵�
                    Vector3 moveDirection = directionToPlayer.normalized;
                    transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                }

                else
                {
                    // attackDistance ���� �ȿ� ������ ��� ȸ���� ����
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }

        // �÷��̾ �ν��ϱ� ���� ������ ��� ���
        else
        {
            animator.SetBool("IsFindPlayer", false);
            animator.SetBool("AttackAble", false);
            isAttacking = false;
        }
    }

    // �����ð� ���� ����� �� ������ �� �� �ְ� ���ִ� �޼ҵ�
    public IEnumerator AttackCoroutine()
    {
        while (true)
        {
            // AttackAble�� true�̰� ���� �ִϸ��̼��� AttackWait�� �ƴ� ���
            if (animator.GetBool("AttackAble") && !animator.GetCurrentAnimatorStateInfo(0).IsName("AttackWait"))
            {
                // waitMinTime�ʿ��� waitMaxTime�� ������ ��� �ð� ����
                float waitTime = Random.Range(waitMinTime, waitMaxTime);

                // 0���� ���� ���� - 1������ ������ AttackType ����
                int randomAttackType = Random.Range(0, maxAttackType + 1);
                animator.SetInteger("AttackType", randomAttackType);

                // ���� ������ ���ؼ� ���� ��� ���� ��⸦ ��
                animator.SetBool("IsEndWait", false);
                yield return new WaitForSeconds(waitTime);
                animator.SetBool("IsEndWait", true);
            }
            yield return null;
        }
    }

    // Idle ���¿��� �÷��̾ ã�� �� �ֵ��� ȸ���� �ִ� �޼ҵ�
    public IEnumerator IdleRotationCoroutine(bool isHit = false)
    {
        while (true)
        {
            // IsEndIdle�� true�� ��� IdleTurnAngle ���� �����ϰ� ����
            if (animator.GetBool("IsEndIdle"))
            {
                float randomIdleTurnAngle = Random.Range(idleTurnAngleMin, idleTurnAngleMax);
                animator.SetFloat("IdleTurnAngle", randomIdleTurnAngle / 105 * 2);

                // Idle ���¿��� ȸ���ϱ� ���� ��� �ð� ����
                float idleTurnWaitTime = Random.Range(idleTurnWaitTimeMin, idleTurnWaitTimeMax);

                // ���������� ȸ���ϴ� ���� �����ϱ� ���ؼ� ��� �ð��� ����
                animator.SetBool("IsEndIdle", false);
                yield return new WaitForSeconds(idleTurnWaitTime);
                animator.SetBool("IsEndIdle", true);
                yield return null;
            }

            else if (isHit)
            {
                // �÷��̾ �ִ� ������ ���մϴ�.
                Vector3 targetDirection = player.transform.position - transform.position;
                targetDirection.y = 0f; // ȸ���� Y �����θ� �Ϸ��� �� �κ��� �����ϼ���.

                if (targetDirection != Vector3.zero)
                {
                    // �ε巴�� ȸ���ϱ� ���� Lerp �Լ��� ����մϴ�.
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 2 * Time.deltaTime);
                }

                Vector3 forwardDirection = transform.forward;

                // �� ���� ������ ���� ��� (acos �Լ��� ���� ������ ��ȯ�ϹǷ� ������ ��ȯ)
                float angle = Mathf.Acos(Vector3.Dot(targetDirection.normalized, forwardDirection)) * Mathf.Rad2Deg;

                if (angle <= 5.0f)
                    isHit = false;
            }

            yield return null;
        }
    }
}