using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public float speed;

    private bool isStart = false;
    private Vector3 front = new Vector3();

    private LineRenderer trajectory;    // LineRenderer
    private List<Vector3> moveCourse;   // �Ѿ��� �̵��� ��ġ�� ���� ����

    private Rigidbody rigid;

    public void Start()
    {
        trajectory = GetComponent<LineRenderer>();
        Debug.Assert(GetComponent<LineRenderer>() != null, "LineRenderer ���۳�Ʈ�� �������� �ʽ��ϴ�. LineRenderer ���۳�Ʈ�� �߰��ϼ���");

        // �̵� ��� ǥ�� Ȱ��ȭ
        trajectory.enabled = true;
    }

    public void Update()
    {
        // ���콺�� �� ��� ȭ���� �߻��
        if (Input.GetMouseButtonUp(0) && !isStart)
            Shoot();

        // ȭ���� �߻��� ���
        if (isStart)
            SetRightDir();
    }

    public void Shoot()
    {
        // ȭ���� �߷¿� ���� ���������� ����
        rigid = gameObject.AddComponent<Rigidbody>();
        rigid.useGravity = true;
        rigid.drag = 2f;
        rigid.angularDrag = 0.0f;

        // ȭ���� �浹�� �� �ְ� ����
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        // ������ ���ư��� ������ ������
        front = -transform.up;

        // �θ� ��ü ����
        transform.parent = null;
        rigid.AddForce(front * speed * 60.0f);

        moveCourse = new List<Vector3>() { transform.position };

        // ȭ�� �߻� �����ϰ� 10�� �� ȭ�� ������Ʈ ����
        isStart = true;
        Destroy(gameObject, 10.0f);
    }

    // ȭ���� ���ư��� ���� ǥ�� �� ȭ���� ���ư��� ���� �ùٸ� �������� ���ư� �� �ְ� ����
    public void SetRightDir()
    {
        moveCourse.Add(transform.position);                 // �Ѿ��� �̵��� ��ġ�� ��� ������

        trajectory.positionCount = moveCourse.Count;        // ��ġ�� ����� ������ ������
        trajectory.SetPositions(moveCourse.ToArray());      // ���� ���� ��ġ�� ������

        // �̵� ��� ������ 2�� �̻��� ���
        if (moveCourse.Count >= 2)
        {
            // ������ �̵� ������ ������ ȭ���� �������� ����
            Vector3 dir = moveCourse[moveCourse.Count - 1] - moveCourse[moveCourse.Count - 2];
            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.down);

            // ȸ���� x���� 90�� ���� ���� ����
            targetRotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);

            transform.rotation = targetRotation;
        }
    }
}
