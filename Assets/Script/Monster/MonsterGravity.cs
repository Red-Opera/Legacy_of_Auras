using UnityEngine;

public class MonsterGravity : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    public float gravity = 9.8f; // 중력의 강도 조절을 위한 변수

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Animator에서 루트 모션을 가져와서 Rigidbody에 적용
        ApplyRootMotion();
    }

    void ApplyRootMotion()
    {
        // Animator의 루트 모션을 가져와서 Rigidbody에 적용
        Vector3 rootMotionVelocity = animator.deltaPosition / Time.fixedDeltaTime;
        rootMotionVelocity.y = 0f;  // 수직 방향의 이동은 무시 (루트 모션에 중력을 추가할 것이므로)
        rb.velocity = rootMotionVelocity + Vector3.down * gravity * Time.fixedDeltaTime;

        transform.rotation = animator.rootRotation;
    }
}
