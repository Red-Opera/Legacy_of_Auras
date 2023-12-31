using System.Collections;
using UnityEngine;

// 몬스터의 상태를 바꾸기 위한 스크립트
public class MonsterControl : MonoBehaviour
{
    public float lookAtDis = 25.0f;             // 플레이어를 인식할 수 있는 거리
    public float lookAtAngle = 120.0f;          // 몬스터가 바라보는 각도 범위
    public float attackDistance = 5.0f;         // 공격 가능한 거리
    public float moveSpeed = 2.0f;              // 몬스터의 이동 속도
    public float rotationSpeed = 5.0f;          // 몬스터의 회전 속도
    public float waitMinTime = 1.0f;            // 공격 후 최소 대기시간
    public float waitMaxTime = 5.0f;            // 공격 후 최대 대기시간
    public int maxAttackType = 2;               // 공격 타입의 개수
    public int idleTurnAngleMin = -120;         // Idle 상태에서 회전하는 방향의 최소 값 (단위 : 도)
    public int idleTurnAngleMax = 120;          // Idle 상태에서 회전하는 방향의 최대 값 (단위 : 도)
    public int idleTurnAngleCurrent = 0;        // 현재 Idle 상태에서 회전하는 방향
    public float idleTurnWaitTimeMin = 5.0f;    // Idle 상태에서 회전하기 위해 대기해야 하는 최소 시간
    public float idleTurnWaitTimeMax = 15.0f;   // Idle 상태에서 회전하기 위해 대기해야 하는 최대 시간

    private GameObject player;              // 플레이어 오브젝트
    private Animator animator;              // 이 오브젝트의 애니메이터

    private bool isAttacking = false;       // 공격 중인지 여부를 나타내는 변수

    public void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference): 애니메이터가 존재하지 않습니다.");

        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference): 플레이어가 존재하지 않습니다.");

        StartCoroutine(AttackCoroutine());          // 공격할 수 있을 때 대기시간에 걸쳐 공격을 실행하기 위한 메소드
        StartCoroutine(IdleRotationCoroutine());    // Idle 상태에서 일정한 시간이 지나면 돌기 위한 메소드
    }

    public void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            return;

        // 플레이어와 몬스터의 위치를 얻음
        Vector3 playerPosition = player.transform.position;
        Vector3 monsterPosition = transform.position;

        Vector3 directionToPlayer = playerPosition - monsterPosition;               // 몬스터가 플레이어를 바라보는 방향을 얻음
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);  // 몬스터가 플레이어를 바라보는 각도를 계산합니다.

        // 몬스터가 플레이어를 바라보는 각도가 lookAtAngle 이내이고, 플레이어와 몬스터 사이의 거리가 lookAtDis 이내라면 공격 상태로 넘어감
        if ((angleToPlayer <= lookAtAngle * 0.5f || isAttacking) && directionToPlayer.magnitude <= lookAtDis)
        {
            animator.SetBool("IsFindPlayer", true);

            // 몬스터와 플레이어 사이의 거리가 공격 가능한 거리보다 작으면 AttackAble을 true로 설정
            if (directionToPlayer.magnitude <= attackDistance && angleToPlayer <= lookAtAngle * 0.5f)
            {
                // 공격할 수 있는 상태로 변하여 플레이어를 공격하도록 설정
                animator.SetBool("AttackAble", true);
                isAttacking = true;
            }

            // 플레이어를 공격할 수 없는 상태가 된 경우
            else
            {
                // 플레이어를 공격하지 못하도록 설정
                animator.SetBool("AttackAble", false);

                // 현재 공격 중이었고 다음 공격 대기 중일 때 플레이어를 보도록 설정
                if (isAttacking || animator.GetCurrentAnimatorStateInfo(0).IsName("AttackWait"))
                {
                    // 플레이어를 향해 회전
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

                    // 플레이어를 향해 선형 이동
                    Vector3 moveDirection = directionToPlayer.normalized;
                    transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                }

                else
                {
                    // attackDistance 범위 안에 들어왔을 경우 회전만 수행
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }

        // 플레이어를 인식하기 위한 범위를 벗어난 경우
        else
        {
            animator.SetBool("IsFindPlayer", false);
            animator.SetBool("AttackAble", false);
            isAttacking = false;
        }
    }

    // 일정시간 동안 대기한 후 공격을 할 수 있게 해주는 메소드
    public IEnumerator AttackCoroutine()
    {
        while (true)
        {
            // AttackAble이 true이고 현재 애니메이션이 AttackWait가 아닌 경우
            if (animator.GetBool("AttackAble") && !animator.GetCurrentAnimatorStateInfo(0).IsName("AttackWait"))
            {
                // waitMinTime초에서 waitMaxTime초 사이의 대기 시간 설정
                float waitTime = Random.Range(waitMinTime, waitMaxTime);

                // 0부터 공격 개수 - 1까지의 랜덤한 AttackType 설정
                int randomAttackType = Random.Range(0, maxAttackType + 1);
                animator.SetInteger("AttackType", randomAttackType);

                // 다음 공격을 위해서 일정 대기 공격 대기를 함
                animator.SetBool("IsEndWait", false);
                yield return new WaitForSeconds(waitTime);
                animator.SetBool("IsEndWait", true);
            }
            yield return null;
        }
    }

    // Idle 상태에서 플레이어를 찾을 수 있도록 회전을 주는 메소드
    public IEnumerator IdleRotationCoroutine(bool isHit = false)
    {
        while (true)
        {
            // IsEndIdle이 true일 경우 IdleTurnAngle 값을 랜덤하게 설정
            if (animator.GetBool("IsEndIdle"))
            {
                float randomIdleTurnAngle = Random.Range(idleTurnAngleMin, idleTurnAngleMax);
                animator.SetFloat("IdleTurnAngle", randomIdleTurnAngle / 105 * 2);

                // Idle 상태에서 회전하기 위한 대기 시간 설정
                float idleTurnWaitTime = Random.Range(idleTurnWaitTimeMin, idleTurnWaitTimeMax);

                // 지속적으로 회전하는 것을 방지하기 위해서 대기 시간을 설정
                animator.SetBool("IsEndIdle", false);
                yield return new WaitForSeconds(idleTurnWaitTime);
                animator.SetBool("IsEndIdle", true);
                yield return null;
            }

            else if (isHit)
            {
                // 플레이어가 있는 방향을 구합니다.
                Vector3 targetDirection = player.transform.position - transform.position;
                targetDirection.y = 0f; // 회전을 Y 축으로만 하려면 이 부분을 조절하세요.

                if (targetDirection != Vector3.zero)
                {
                    // 부드럽게 회전하기 위해 Lerp 함수를 사용합니다.
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 2 * Time.deltaTime);
                }

                Vector3 forwardDirection = transform.forward;

                // 두 벡터 사이의 각도 계산 (acos 함수는 라디안 단위를 반환하므로 각도로 변환)
                float angle = Mathf.Acos(Vector3.Dot(targetDirection.normalized, forwardDirection)) * Mathf.Rad2Deg;

                if (angle <= 5.0f)
                    isHit = false;
            }

            yield return null;
        }
    }
}