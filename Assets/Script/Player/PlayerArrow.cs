using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerArrow : MonoBehaviour
{
    public float speed;                     // 화살의 이동속도
    public int addDamage;                   // 화살의 추가 공격력

    public GameObject bloodEffect;          // 피격 이펙트

    public AudioClip launchSound;           // 화살 발사 소리
    public AudioClip hitSound;              // 화살이 맞는 소리

    private bool isStart = false;
    private Vector3 front = new Vector3();

    private LineRenderer trajectory;        // LineRenderer
    private List<Vector3> moveCourse;       // 총알이 이동한 위치를 담을 변수

    private Rigidbody rigid;

    private bool isDestoryed = false;

    public void Start()
    {
        trajectory = GetComponent<LineRenderer>();
        Debug.Assert(GetComponent<LineRenderer>() != null, "LineRenderer 컴퍼넌트가 존재하지 않음. LineRenderer 컴퍼넌트를 추가 필요");

        // 이동 경로 표시 활성화
        trajectory.enabled = true;
    }

    public void Update()
    {
        // 마우스를 땔 경우 화살이 발사됨
        if (Input.GetMouseButtonUp(0) && !isStart)
            Shoot();

        // 화살이 발사할 경우
        if (isStart)
            SetRightDir();
    }

    public void Shoot()
    {
        // 화살이 중력에 의해 떨어지도록 설정
        rigid = gameObject.AddComponent<Rigidbody>();
        rigid.useGravity = true;
        rigid.drag = 2f;
        rigid.angularDrag = 0.0f;

        // 부모 객체 해제
        front = transform.parent.forward;
        transform.localPosition = new Vector3(2.5f, 2.0f, -5.0f);
        transform.parent = null;
        transform.localRotation = Quaternion.identity;

        rigid.AddForce(front * speed * 60.0f);

        moveCourse = new List<Vector3>() { transform.position };

        // 화살 발사 시작하고 10초 뒤 화살 오브젝트 제거
        isStart = true;
        Destroy(gameObject, 10.0f);

        // 소리 재생
        GetComponent<AudioSource>().PlayOneShot(launchSound);
    }

    // 화살이 나아가는 방향 표시 및 화살이 나아가는 동안 올바른 방향으로 나아갈 수 있게 설정
    public void SetRightDir()
    {
        moveCourse.Add(transform.position);                 // 총알이 이동한 위치를 모두 저장함

        // 이동 경로 개수가 2개 이상일 경우
        if (moveCourse.Count >= 2)
        {
            // 마지막 이동 동선의 방향을 화살의 방향으로 설정
            Vector3 dir = moveCourse[moveCourse.Count - 1] - moveCourse[moveCourse.Count - 2];

            if (dir !=  Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.down);

                // 회전의 x값에 90도 더한 값을 설정
                targetRotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);

                transform.rotation = targetRotation;

                trajectory.positionCount = moveCourse.Count;        // 위치를 기록한 개수를 가져옴
                trajectory.SetPositions(moveCourse.ToArray());      // 선을 이을 위치를 대입함
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDestoryed)
            return;

        // 화살이 몬스터에게 맞은 경우
        if (collision.gameObject.CompareTag("Monster"))
        {
            // 충돌한 객체가 Monster 태그를 가진 경우
            MonsterHPBar monsterHPBar = collision.gameObject.GetComponent<MonsterHPBar>();

            if (monsterHPBar == null)
                monsterHPBar = collision.gameObject.transform.parent.GetComponent<MonsterHPBar>();

            // MonsterHPBar 컴포넌트가 있다면 SetDamage을 실행
            if (monsterHPBar != null)
                monsterHPBar.SetDamage(addDamage);

            GetComponent<AudioSource>().PlayOneShot(hitSound);
            GetComponent<MeshCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            // 피격 이펙트
            GameObject effect = Instantiate(bloodEffect, transform);
            effect.transform.parent = null;

            effect.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
            effect.transform.localRotation = Quaternion.identity;

            effect.transform.GetChild(0).GetComponent<VisualEffect>().Play();
            Destroy(effect, 60f);

            // 화살 오브젝트 제거
            Destroy(gameObject, 0.4f);
            isDestoryed = true;
        }
    }
}