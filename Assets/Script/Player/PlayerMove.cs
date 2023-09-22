using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float defaultSpeed;          // �⺻ �̵� �ӵ�
    public float runMulti;              // �޷��� �� �⺻ �̵� �ӵ��� ���
    public float jumpPower = 10f;       // �÷��̾� ������

    private float toSpeed;              // ��ǥ �̵��ӵ�
    private float currentSpeed;         // ���� �̵��ӵ�

    private Rigidbody rigid;
    private Animator animator;

    private bool isJumpAnimation = false;
    private bool isGrounded = true;

    public void Start()
    {
        DontDestroyOnLoad(gameObject.transform.parent);

        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Debug.Assert(rigid != null, "Error (Null Reference) : �ش� ��ü�� RigidBody�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� Animator�� �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
        // ���� ���� ������ Ȯ��
        isJumpAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping");

        // ���� ���� Ű�� ������ �� NPC�� ��ȭ ���� ��� ������ ���� �ʾ��� ���
        if (ChatNPC.isEnd && Input.GetKeyDown(KeyCode.Space) && !isJumpAnimation && isGrounded && (animator.GetFloat("IdleMode") < 1))
            Jump();

        // �̵�
        Move();
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");                                    // ���� Ʈ���� �߻�
        rigid.AddForce(transform.up * jumpPower, ForceMode.Impulse);    // ���� �����¸�ŭ ���� �ø�
        isGrounded = false;                                             // ���� ĳ���Ͱ� ���� ���� ������ ǥ��
    }

    private void Move()
    {
        // NPC�� ��ȭ ���� ��� �̵����� ����
        if (!ChatNPC.isEnd)
        {
            animator.SetFloat("PlayerFront", 0.0f);
            animator.SetFloat("PlayerLeft", 0.0f);

            animator.SetBool("isWalk", false);
            return;
        }

        // �̵� �Է� Ű�� �Է� ����
        float x = Input.GetAxis("Horizontal"), y = Input.GetAxis("Vertical");

        // �޸��� Ű�� ������ �� ��ǥ �ӵ��� ������
        toSpeed = defaultSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && (animator.GetFloat("IdleMode") != 1.0f))
            toSpeed *= runMulti;

        // �ȴ� �ӵ��� �޸��� �ӵ����� �������� �κ��� �ε巴�� ����
        currentSpeed = Mathf.Lerp(currentSpeed, toSpeed, Time.deltaTime * 3.0f);

        // ���� ������ ���� �ʰ� �̵� Ű�� ������ ���
        if (!isJumpAnimation && new Vector2(x, y).magnitude > 0)
        {
            // �ִϸ��̼��� �۵����� ���� ��쿡�� �ִϸ��̼� ���� 
            if (!animator.GetBool("isWalk"))
                animator.SetBool("isWalk", true);

            // �ش� ���⿡ �´� �ִϸ��̼��� �����ϵ��� ���� ����
            animator.SetFloat("PlayerFront", (x * currentSpeed) / defaultSpeed);
            animator.SetFloat("PlayerLeft", (y * currentSpeed) / defaultSpeed);

            // Ű���� �Է� �������� ������
            Vector3 moveDirection = new Vector3(x, 0, y);
            moveDirection = transform.TransformDirection(moveDirection); // ĳ���� ���� ��ǥ�� �������� �̵� ���� ��ȯ
            moveDirection *= currentSpeed;
            moveDirection.y = 0;                // �������� �̵� ����

            // �ش� �������� ������
            rigid.velocity = moveDirection;
            rigid.AddForce(Physics.gravity);    // �߷��� ���� ó��
        }

        // ���� �Է� Ű�� ������ �ʾҴٸ� Idle ���·� ���ư�
        else
            if (animator.GetBool("isWalk"))
                animator.SetBool("isWalk", false);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
            isGrounded = true;
    }
}