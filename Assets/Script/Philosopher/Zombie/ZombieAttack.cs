using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public GameObject zombie;               // ���� ������Ʈ

    private Animator animator;              // ���� �ִϸ�����
    private LineRenderer lineRenderer;      // ���� ���� ������

    private List<Vector3> positionLog;      // ���� ��θ� ������ �迭

    void Start()
    {
        animator = zombie.GetComponent<Animator>();
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
        if (info.IsName("ZombieAttack") && info.normalizedTime % 1 >= 0.355 && info.normalizedTime % 1 <= 0.519)
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
        if ((info.IsName("ZombieAttack") && info.normalizedTime % 1 <= 0.03) || !info.IsName("ZombieAttack"))
        {
            lineRenderer.positionCount = 0;

            positionLog.Clear();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }
}
