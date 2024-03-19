using System.Collections.Generic;
using UnityEngine;

public class LastBossBurningFlameSlash : MonoBehaviour
{
    [SerializeField] private GameObject boss;   // ���� ������Ʈ

    private Animator animator;                  // ���� �ִϸ�����
    private LineRenderer lineRenderer;          // ���� ���� ������

    private List<Vector3> positionLog;          // ���� ��θ� ������ �迭

    void Start()
    {
        animator = boss.GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();

        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
        Debug.Assert(lineRenderer != null, "Error (Null Reference) : ���η������� �������� �ʽ��ϴ�.");

        positionLog = new List<Vector3>();
    }

    void Update()
    {
        // ���� �ִϸ��̼��� ������ ������
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        // �����ϴ� �ִϸ��̼��� �ϰ� ���� ���
        if (info.IsName("BurningFlameSlash") && info.normalizedTime % 1 >= 0.26 && info.normalizedTime % 1 <= 0.4)
        {
            // ���� �׷��� �� ���� ���
            if (positionLog.Count > 1)
            {
                lineRenderer.positionCount = positionLog.Count;
                lineRenderer.SetPositions(positionLog.ToArray());
            }

            // ���� ��ġ�� ������
            positionLog.Add(transform.position);
        }

        // ���ο� ������ �ϰų� ���� �����ϰ� ���� ���� ���
        if ((info.IsName("BurningFlameSlash") && info.normalizedTime % 1 <= 0.03) || !info.IsName("BurningFlameSlash"))
        {
            lineRenderer.positionCount = 0;

            positionLog.Clear();
        }
    }
}
