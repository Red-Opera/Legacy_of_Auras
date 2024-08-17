using UnityEngine;

public class LastBossWingPosition : MonoBehaviour
{
    [HideInInspector] public bool isFlying = false;             // ���� ���� �ִ� ����

    [SerializeField] private GameObject lastBoss;               // ���� ���� ������Ʈ
    [SerializeField] private AnimationCurve animationCurve;     // �������� �ִϸ��̼� �ð��� ���� ��ġ�ؾ��� ����
    [SerializeField] private AudioClip bossWindSound;           // ���� ���� �Ҹ�

    private Animator animator;          // �ִϸ�����
    private AudioSource audioSource;    // ����� �ҽ�
    private Vector3 defaultPosition;    // ���� ��ġ
    private Vector3 addPosition;        // ���� ���� ��ġ���� �߰��� �̵��� ��ġ

    private bool isWing = false;

    void Start()
    {
        Debug.Assert(lastBoss != null, "Error (Null Reference) : ���� ���� ������Ʈ�� �������� �ʽ��ϴ�.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "Error (Null Reference) : ����� �ҽ��� �������� �ʽ��ϴ�.");

        // �ʱ⿡�� ���� ��ġ�� �������� ����
        addPosition = Vector3.zero;
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (defaultPosition.y < 0)
            defaultPosition += Vector3.up;

        // ���� ���� ���� ��� ��ġ�� �ʱ� ��ġ�� ������Ʈ ��
        if (!isFlying)
            defaultPosition = lastBoss.transform.position;

        // ���� �������� �ϰ� �ִ� �ִϸ��̼��� �ϰ� ���� ���
        else if (state.IsName("LassBossFlying") && animator.enabled)
        {
            isFlying = true;

            // �ִϸ��̼� ���൵�� ���� �߰� ��ġ�� ����
            addPosition = new Vector3(0.0f, animationCurve.Evaluate(state.normalizedTime % 1), 0.0f) * 3.0f;
            lastBoss.transform.position = defaultPosition + addPosition;

            // �������� �� �Ҹ� ���
            if (state.normalizedTime % 1 <= 0.1)
                isWing = false;

            else if (!isWing && state.normalizedTime % 1 >= 0.27)
            {
                isWing = true;
                audioSource.PlayOneShot(bossWindSound);
            }
        }

        else
            isFlying = false;
    }
}
