using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float defaultSpeed;          // �⺻ �̵� �ӵ�
    public float runMulti;              // �޷��� �� �⺻ �̵� �ӵ��� ���
    public float jumpPower = 10f;       // �÷��̾� ������

    private float toSpeed;              // ��ǥ �̵��ӵ�
    private float currentSpeed;         // ���� �̵��ӵ�

    private CharacterController characterController;    
    private Rigidbody rigid;
    private Animator animator;
    private Vector3 moveDirection;

    private bool isJumpAnimation = false;
    private bool isGrounded = true;

    public void Start()
    {
        DontDestroyOnLoad(gameObject.transform.parent);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        Debug.Assert(characterController != null, "Error (Null Reference) : �ش� ��ü�� characterController�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� Animator�� �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
        characterController.Move(moveDirection * Time.deltaTime);

        if (TypeStory.hasActivatedCanvas || ItemShopOpenClose.isShopOpen || PlayerGetAurasArrow.isGetting)
        {
            animator.SetBool("isWalk", false);
            moveDirection = Vector3.zero;
            return;
        }

        // ���� ���� ������ Ȯ��
        isJumpAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping");

        CheckIsGround();

        // ���� ���� Ű�� ������ �� NPC�� ��ȭ ���� ��� ������ ���� �ʾ��� ���
        if (ChatNPC.isEnd && Input.GetKeyDown(KeyCode.Space) && !isJumpAnimation && isGrounded && (animator.GetFloat("IdleMode") < 1))
            Jump();

        // �̵� ó��
        Move();

        SetGravity();
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");                // ���� Ʈ���� �߻�
        moveDirection.y += jumpPower;               // ���� �����¸�ŭ ���� �ø�
        isGrounded = false;                         // ���� ĳ���Ͱ� ���� ���� ������ ǥ��
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
        float idleMode = animator.GetFloat("IdleMode");

        
        // �޸��� Ű�� ������ �� ��ǥ �ӵ��� ������
        toSpeed = defaultSpeed;

        if (SceneManager.GetActiveScene().name == "Forest")
            toSpeed *= 0.3f;

        if (Input.GetKey(KeyCode.LeftShift) && (idleMode != 1.0f) && !(idleMode > 2.9f && idleMode < 3.1f))
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
            if (SceneManager.GetActiveScene().name != "Forest")
            {
                animator.SetFloat("PlayerFront", (x * currentSpeed) / defaultSpeed);
                animator.SetFloat("PlayerLeft", (y * currentSpeed) / defaultSpeed);
            }

            else
            {
                animator.SetFloat("PlayerFront", (x * currentSpeed) / (defaultSpeed * 0.3f));
                animator.SetFloat("PlayerLeft", (y * currentSpeed) / (defaultSpeed * 0.3f));
            }

            // Ű���� �Է� �������� ������
            moveDirection = new Vector3(x, 0, y);
            moveDirection = transform.TransformDirection(moveDirection); // ĳ���� ���� ��ǥ�� �������� �̵� ���� ��ȯ
            moveDirection *= currentSpeed;
            moveDirection.y = Physics.gravity.y;        // �������� �̵� ����

            if (SceneManager.GetActiveScene().name == "Forest")
                moveDirection.y /= 10;
        }

        // ���� �Է� Ű�� ������ �ʾҴٸ� Idle ���·� ���ư�
        else
        {
            if (animator.GetBool("isWalk"))
            {
                animator.SetBool("isWalk", false);
                characterController.Move(Vector3.zero);
            }

            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 2.0f); // ���������� �ӵ� ����
        }
    }

    private void SetGravity()
    {
        if (characterController.isGrounded)
            moveDirection.y = 0;

        else
            moveDirection.y += Physics.gravity.y * Time.deltaTime;
    }

    private void CheckIsGround()
    {
        isGrounded = characterController.isGrounded;
    }
}