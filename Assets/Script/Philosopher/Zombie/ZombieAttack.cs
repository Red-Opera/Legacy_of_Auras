using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public GameObject zombie;               // 좀비 오브젝트

    private Animator animator;              // 좀비 애니메이터
    private LineRenderer lineRenderer;      // 공격 라인 렌더러

    private List<Vector3> positionLog;      // 지난 경로를 저장할 배열

    void Start()
    {
        animator = zombie.GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();

        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이터가 존재하지 않습니다.");
        Debug.Assert(lineRenderer != null, "Error (Null Reference) : 라인랜더러가 존재하지 않습니다.");

        positionLog = new List<Vector3>();
    }

    void Update()
    {
        // 현재 애니메이션의 정보를 가져옴
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        // 공격하는 애니메이션을 하고 있을 경우
        if (info.IsName("ZombieAttack") && info.normalizedTime % 1 >= 0.355 && info.normalizedTime % 1 <= 0.519)
        {
            // 선이 그려질 수 있을 경우
            if (positionLog.Count > 1)
            {
                lineRenderer.positionCount = positionLog.Count;
                lineRenderer.SetPositions(positionLog.ToArray());
            }
            
            // 현재 위치를 저장함
            positionLog.Add(transform.position);
        }

        // 새로운 공격을 하거나 현재 공격하고 있지 않을 경우
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
