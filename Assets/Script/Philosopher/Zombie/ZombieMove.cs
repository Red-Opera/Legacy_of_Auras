using UnityEngine;
using UnityEngine.AI;

public class ZombieMove : MonoBehaviour
{
    public AnimationCurve moveSpeed;    // �ִϸ��̼� ���࿡ ���� ���� �ӵ�
    public float speed = 1.7f;          // �߰� �̵��ӵ�
    public float waitDisace = 1.1f;     // ���� ��Ÿ�

    private Animator animator;          // �ִϸ��̼� ������Ʈ
    private Rigidbody rigidbody;
    private GameObject player;          // �÷��̾� ������Ʈ
    private NavMeshAgent agent;         // �̵� AI ������Ʈ

    private bool isStop = false;        // �̵��� ���� ���

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.Find("MonsterTarget");

        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
        Debug.Assert(rigidbody != null, "Error (Null Reference) : �߷� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");

        agent.enabled = false;
    }

    void Update()
    {
        if (!agent.enabled)
            agent.enabled = true;

        // ���� ���� ���
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            agent.isStopped = true;
            agent.enabled = false;
            return;
        }

        // �׻� �÷��̾ �ٶ󺸵��� ����
        transform.LookAt(player.transform.position);

        CloseWithPlayer();

        if (!isStop)
            Move();
    }

    // ���� �ڿ������� �̵��� ���� �޼ҵ�
    private void Move()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        agent.speed = moveSpeed.Evaluate(info.normalizedTime % 1) * speed;

        agent.SetDestination(player.transform.position);
    }

    // ���� ������ ������ ��� ����Ǵ� �޼ҵ�
    private void CloseWithPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // �÷��̾�� ������� ��� �÷��̾ �����ϵ��� ����
        if (distance < waitDisace)
        {
            agent.isStopped = true;
            isStop = true;

            animator.SetBool("Attackable", true);
        }

        // �÷��̾�� �־��� ��� �÷��̾ �i�� �� �ֵ��� ����
        else
        {
            agent.isStopped = false;
            isStop = false;

            animator.SetBool("Attackable", false);
        }
    }
}
