using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerArrow : MonoBehaviour
{
    public float speed;                     // ȭ���� �̵��ӵ�
    public int addDamage;                   // ȭ���� �߰� ���ݷ�

    public GameObject bloodEffect;          // �ǰ� ����Ʈ

    public AudioClip launchSound;           // ȭ�� �߻� �Ҹ�
    public AudioClip hitSound;              // ȭ���� �´� �Ҹ�

    private bool isStart = false;
    private Vector3 front = new Vector3();

    private LineRenderer trajectory;        // LineRenderer
    private List<Vector3> moveCourse;       // �Ѿ��� �̵��� ��ġ�� ���� ����

    private Rigidbody rigid;

    private bool isDestoryed = false;

    public void Start()
    {
        trajectory = GetComponent<LineRenderer>();
        Debug.Assert(GetComponent<LineRenderer>() != null, "LineRenderer ���۳�Ʈ�� �������� ����. LineRenderer ���۳�Ʈ�� �߰� �ʿ�");

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

        // �θ� ��ü ����
        front = transform.parent.forward;
        transform.localPosition = new Vector3(2.5f, 2.0f, -5.0f);
        transform.parent = null;
        transform.localRotation = Quaternion.identity;

        rigid.AddForce(front * speed * 60.0f);

        moveCourse = new List<Vector3>() { transform.position };

        // ȭ�� �߻� �����ϰ� 10�� �� ȭ�� ������Ʈ ����
        isStart = true;
        Destroy(gameObject, 10.0f);

        // �Ҹ� ���
        GetComponent<AudioSource>().PlayOneShot(launchSound);
    }

    // ȭ���� ���ư��� ���� ǥ�� �� ȭ���� ���ư��� ���� �ùٸ� �������� ���ư� �� �ְ� ����
    public void SetRightDir()
    {
        moveCourse.Add(transform.position);                 // �Ѿ��� �̵��� ��ġ�� ��� ������

        // �̵� ��� ������ 2�� �̻��� ���
        if (moveCourse.Count >= 2)
        {
            // ������ �̵� ������ ������ ȭ���� �������� ����
            Vector3 dir = moveCourse[moveCourse.Count - 1] - moveCourse[moveCourse.Count - 2];

            if (dir !=  Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.down);

                // ȸ���� x���� 90�� ���� ���� ����
                targetRotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);

                transform.rotation = targetRotation;

                trajectory.positionCount = moveCourse.Count;        // ��ġ�� ����� ������ ������
                trajectory.SetPositions(moveCourse.ToArray());      // ���� ���� ��ġ�� ������
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDestoryed)
            return;

        // ȭ���� ���Ϳ��� ���� ���
        if (collision.gameObject.CompareTag("Monster"))
        {
            // �浹�� ��ü�� Monster �±׸� ���� ���
            MonsterHPBar monsterHPBar = collision.gameObject.GetComponent<MonsterHPBar>();

            if (monsterHPBar == null)
                monsterHPBar = collision.gameObject.transform.parent.GetComponent<MonsterHPBar>();

            // MonsterHPBar ������Ʈ�� �ִٸ� SetDamage�� ����
            if (monsterHPBar != null)
                monsterHPBar.SetDamage(addDamage);

            GetComponent<AudioSource>().PlayOneShot(hitSound);
            GetComponent<MeshCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            // �ǰ� ����Ʈ
            GameObject effect = Instantiate(bloodEffect, transform);
            effect.transform.parent = null;

            effect.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
            effect.transform.localRotation = Quaternion.identity;

            effect.transform.GetChild(0).GetComponent<VisualEffect>().Play();
            Destroy(effect, 60f);

            // ȭ�� ������Ʈ ����
            Destroy(gameObject, 0.4f);
            isDestoryed = true;
        }
    }
}