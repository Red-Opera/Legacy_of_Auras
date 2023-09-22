using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float defaultSpeed;          // 기본 이동 속도
    public float runMulti;              // 달렸을 때 기본 이동 속도의 배수
    public float jumpPower = 10f;       // 플레이어 점프력

    private float toSpeed;              // 목표 이동속도
    private float currentSpeed;         // 현재 이동속도

    private Rigidbody rigid;
    private Animator animator;

    private bool isJumpAnimation = false;
    private bool isGrounded = true;

    public void Start()
    {
        DontDestroyOnLoad(gameObject.transform.parent);

        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Debug.Assert(rigid != null, "Error (Null Reference) : 해당 객체에 RigidBody가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 Animator가 존재하지 않습니다.");
    }

    public void Update()
    {
        // 현재 점프 중인지 확인
        isJumpAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping");

        // 만약 점프 키를 눌렸을 때 NPC와 대화 중인 경우 점프를 하지 않았을 경우
        if (ChatNPC.isEnd && Input.GetKeyDown(KeyCode.Space) && !isJumpAnimation && isGrounded && (animator.GetFloat("IdleMode") < 1))
            Jump();

        // 이동
        Move();
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");                                    // 점프 트리거 발생
        rigid.AddForce(transform.up * jumpPower, ForceMode.Impulse);    // 위로 점프력만큼 위로 올림
        isGrounded = false;                                             // 현재 캐릭터가 땅에 있지 않음을 표시
    }

    private void Move()
    {
        // NPC와 대화 중인 경우 이동하지 않음
        if (!ChatNPC.isEnd)
        {
            animator.SetFloat("PlayerFront", 0.0f);
            animator.SetFloat("PlayerLeft", 0.0f);

            animator.SetBool("isWalk", false);
            return;
        }

        // 이동 입력 키를 입력 받음
        float x = Input.GetAxis("Horizontal"), y = Input.GetAxis("Vertical");

        // 달리는 키를 눌렸을 때 목표 속도를 지정함
        toSpeed = defaultSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && (animator.GetFloat("IdleMode") != 1.0f))
            toSpeed *= runMulti;

        // 걷는 속도와 달리는 속도에서 순간적인 부분을 부드럽게 수정
        currentSpeed = Mathf.Lerp(currentSpeed, toSpeed, Time.deltaTime * 3.0f);

        // 만약 점프를 하지 않고 이동 키를 눌렸을 경우
        if (!isJumpAnimation && new Vector2(x, y).magnitude > 0)
        {
            // 애니메이션이 작동하지 않을 경우에만 애니메이션 실행 
            if (!animator.GetBool("isWalk"))
                animator.SetBool("isWalk", true);

            // 해당 방향에 맞는 애니메이션이 실행하도록 변수 조정
            animator.SetFloat("PlayerFront", (x * currentSpeed) / defaultSpeed);
            animator.SetFloat("PlayerLeft", (y * currentSpeed) / defaultSpeed);

            // 키보드 입력 방향으로 움직임
            Vector3 moveDirection = new Vector3(x, 0, y);
            moveDirection = transform.TransformDirection(moveDirection); // 캐릭터 로컬 좌표계 기준으로 이동 방향 변환
            moveDirection *= currentSpeed;
            moveDirection.y = 0;                // 공중으로 이동 금지

            // 해당 방향으로 움직임
            rigid.velocity = moveDirection;
            rigid.AddForce(Physics.gravity);    // 중력은 따로 처리
        }

        // 만약 입력 키를 누르지 않았다면 Idle 상태로 돌아감
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