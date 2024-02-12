using UnityEngine;

public class MonsterGravity : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    public float gravity = 9.8f; // �߷��� ���� ������ ���� ����

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Animator���� ��Ʈ ����� �����ͼ� Rigidbody�� ����
        ApplyRootMotion();
    }

    void ApplyRootMotion()
    {
        // Animator�� ��Ʈ ����� �����ͼ� Rigidbody�� ����
        Vector3 rootMotionVelocity = animator.deltaPosition / Time.fixedDeltaTime;
        rootMotionVelocity.y = 0f;  // ���� ������ �̵��� ���� (��Ʈ ��ǿ� �߷��� �߰��� ���̹Ƿ�)
        rb.velocity = rootMotionVelocity + Vector3.down * gravity * Time.fixedDeltaTime;

        transform.rotation = animator.rootRotation;
    }
}
