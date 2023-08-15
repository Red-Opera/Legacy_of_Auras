using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 1f;        // �÷��̾� �̵��ӵ�
    public float jumpPower = 10f;   // �÷��̾� ������

    private Rigidbody rigid;
    private Animator animator;

    public void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Debug.Assert(rigid != null, "Error (Null Reference) : �ش� ��ü�� RigidBody�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� Animator�� �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
        // �̵� �Է� Ű�� �Է� ����
        float x = Input.GetAxis("Horizontal"), y = Input.GetAxis("Vertical");

        // ���� ���� Ű�� ���ȴٸ�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");                    // ���� Ʈ���� �߻�
            rigid.AddForce(transform.up * jumpPower);   // ���� �����¸�ŭ ���� �ø�
        }

        // ���� ������ ���� �ʰ� �̵� Ű�� ������ ���
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping") && new Vector2(x, y).magnitude > 0)
        {
            // �ִϸ��̼��� �۵����� ���� ��쿡�� �ִϸ��̼� ���� 
            if (!animator.GetBool("isWalk"))
                animator.SetBool("isWalk", true);

            // �ش� ���⿡ �´� �ִϸ��̼��� �����ϵ��� ���� ����
            animator.SetFloat("PlayerFront", x);
            animator.SetFloat("PlayerLeft", -y);
            
            // �ش� �������� ������
            rigid.MovePosition(transform.position + new Vector3(x, 0, y) * speed);
        }

        // ���� �Է� Ű�� ������ �ʾҴٸ� Idle ���·� ���ư�
        else
            animator.SetBool("isWalk", false);
    }
}
