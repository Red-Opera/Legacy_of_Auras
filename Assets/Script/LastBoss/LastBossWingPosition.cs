using UnityEngine;

public class LastBossWingPosition : MonoBehaviour
{
    [HideInInspector] public bool isFlying = false;             // 현재 날고 있는 여부

    [SerializeField] private GameObject lastBoss;               // 최종 보스 오브잭트
    [SerializeField] private AnimationCurve animationCurve;     // 원점에서 애니메이션 시간에 따라 위치해야할 정보

    private Animator animator;          // 애니메이터
    private Vector3 defaultPosition;    // 원래 워치
    private Vector3 addPosition;        // 현재 원래 위치에서 추가로 이동한 위치

    void Start()
    {
        Debug.Assert(lastBoss != null, "Error (Null Reference) : 최종 보스 오브젝트가 존재하지 않습니다.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이터가 존재하지 않습니다.");

        // 초기에는 현재 위치를 원점으로 설정
        addPosition = Vector3.zero;
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (defaultPosition.y < 0)
            defaultPosition += Vector3.up;

        // 날고 있지 않을 경우 위치를 초기 위치를 업데이트 함
        if (!isFlying)
            defaultPosition = lastBoss.transform.position;

        // 현재 날개짓을 하고 있는 애니메이션을 하고 있을 경우
        else if (state.IsName("LassBossFlying") && animator.enabled)
        {
            isFlying = true;

            // 애니메이션 진행도에 따라 추가 위치도 설정
            addPosition = new Vector3(0.0f, animationCurve.Evaluate(state.normalizedTime % 1), 0.0f) * 3.0f;
            lastBoss.transform.position = defaultPosition + addPosition;
        }

        else
            isFlying = false;
    }
}
