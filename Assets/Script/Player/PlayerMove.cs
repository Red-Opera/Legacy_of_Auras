using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 1f;        // 플레이어 이동속도
    public float jumpPower = 10f;   // 플레이어 점프력

    private Rigidbody rigid;
    private Animator animator;

    public void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Debug.Assert(rigid != null, "Error (Null Reference) : 해당 객체에 RigidBody가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 Animator가 존재하지 않습니다.");
    }

    public void Update()
    {
        // 이동 입력 키를 입력 받음
        float x = Input.GetAxis("Horizontal"), y = Input.GetAxis("Vertical");

        // 만약 점프 키를 눌렸다면
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");                    // 점프 트리거 발생
            rigid.AddForce(transform.up * jumpPower);   // 위로 점프력만큼 위로 올림
        }

        // 만약 점프를 하지 않고 이동 키를 눌렸을 경우
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping") && new Vector2(x, y).magnitude > 0)
        {
            // 애니메이션이 작동하지 않을 경우에만 애니메이션 실행 
            if (!animator.GetBool("isWalk"))
                animator.SetBool("isWalk", true);

            // 해당 방향에 맞는 애니메이션이 실행하도록 변수 조정
            animator.SetFloat("PlayerFront", x);
            animator.SetFloat("PlayerLeft", -y);
            
            // 해당 방향으로 움직임
            rigid.MovePosition(transform.position + new Vector3(x, 0, y) * speed);
        }

        // 만약 입력 키를 누르지 않았다면 Idle 상태로 돌아감
        else
            animator.SetBool("isWalk", false);
    }
}
