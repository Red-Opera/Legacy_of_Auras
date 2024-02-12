using UnityEngine;
using UnityEngine.AI;

public class ZombieMove : MonoBehaviour
{
    public AnimationCurve moveSpeed;    // 애니메이션 진행에 따른 좀비 속도
    public float speed = 1.7f;          // 추가 이동속도
    public float waitDisace = 1.1f;     // 공격 사거리

    private Animator animator;          // 애니메이션 컴포넌트
    private Rigidbody rigidbody;
    private GameObject player;          // 플레이어 오브젝트
    private NavMeshAgent agent;         // 이동 AI 컴포넌트

    private bool isStop = false;        // 이동을 멈춘 경우

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.Find("MonsterTarget");

        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이터가 존재하지 않습니다.");
        Debug.Assert(rigidbody != null, "Error (Null Reference) : 중력 컴포넌트가 존재하지 않습니다.");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");

        agent.enabled = false;
    }

    void Update()
    {
        if (!agent.enabled)
            agent.enabled = true;

        // 좀비가 죽은 경우
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            agent.isStopped = true;
            agent.enabled = false;
            return;
        }

        // 항상 플레이어를 바라보도록 설정
        transform.LookAt(player.transform.position);

        CloseWithPlayer();

        if (!isStop)
            Move();
    }

    // 좀비가 자연스러운 이동을 위한 메소드
    private void Move()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        agent.speed = moveSpeed.Evaluate(info.normalizedTime % 1) * speed;

        agent.SetDestination(player.transform.position);
    }

    // 공격 가능한 상태일 경우 실행되는 메소드
    private void CloseWithPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // 플레이어와 가까워진 경우 플레이어를 공격하도록 설정
        if (distance < waitDisace)
        {
            agent.isStopped = true;
            isStop = true;

            animator.SetBool("Attackable", true);
        }

        // 플레이어와 멀어진 경우 플레이어를 쫒을 수 있도록 설정
        else
        {
            agent.isStopped = false;
            isStop = false;

            animator.SetBool("Attackable", false);
        }
    }
}
