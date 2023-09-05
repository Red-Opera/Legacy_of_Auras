using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public float speed;

    private bool isStart = false;
    private Vector3 front = new Vector3();

    private LineRenderer trajectory;    // LineRenderer
    private List<Vector3> moveCourse;   // 총알이 이동한 위치를 담을 변수

    private Rigidbody rigid;

    public void Start()
    {
        trajectory = GetComponent<LineRenderer>();
        Debug.Assert(GetComponent<LineRenderer>() != null, "LineRenderer 컴퍼넌트가 존재하지 않습니다. LineRenderer 컴퍼넌트를 추가하세요");

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

        // 화살이 충돌할 수 있게 설정
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        // 앞으로 나아가는 방향을 설정함
        front = -transform.up;

        // 부모 객체 해제
        transform.parent = null;
        rigid.AddForce(front * speed * 60.0f);

        moveCourse = new List<Vector3>() { transform.position };

        // 화살 발사 시작하고 10초 뒤 화살 오브젝트 제거
        isStart = true;
        Destroy(gameObject, 10.0f);
    }

    // 화살이 나아가는 방향 표시 및 화살이 나아가는 동안 올바른 방향으로 나아갈 수 있게 설정
    public void SetRightDir()
    {
        moveCourse.Add(transform.position);                 // 총알이 이동한 위치를 모두 저장함

        trajectory.positionCount = moveCourse.Count;        // 위치를 기록한 개수를 가져옴
        trajectory.SetPositions(moveCourse.ToArray());      // 선을 이을 위치를 대입함

        // 이동 경로 개수가 2개 이상일 경우
        if (moveCourse.Count >= 2)
        {
            // 마지막 이동 동선의 방향을 화살의 방향으로 설정
            Vector3 dir = moveCourse[moveCourse.Count - 1] - moveCourse[moveCourse.Count - 2];
            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.down);

            // 회전의 x값에 90도 더한 값을 설정
            targetRotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);

            transform.rotation = targetRotation;
        }
    }
}
