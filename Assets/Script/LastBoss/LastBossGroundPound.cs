using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LastBossGroundPound : MonoBehaviour
{
    [HideInInspector] public bool isGroundPound = false;

    [SerializeField] private DecalProjector attackRange;        // 공격 범위를 나타내는 컴포넌트
    [SerializeField] private AnimationCurve upSpeed;            // 올라가는 속도
    [SerializeField] private Animator wing;                     // 날개를 관리하는 애니메이터
    [SerializeField] private GameObject explosion;              // 폭파 효과 오브젝트
    [SerializeField] private float attackRangeSize = 35.0f;     // 공격 범위 원 최대 사이즈
    [SerializeField] private float rotateTime = 2.0f;           // 아래로 내려가기전 회전 속도 (초)

    private Animator animator;
    private LastBossAction lastBossAction;  // 최종 보스의 행동을 처리하는 컴포넌트

    public void Start()
    {
        Debug.Assert(wing != null, "Error (Null Reference) : 날개를 관리하는 애니메이터가 존재하지 않습니다.");
        
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 애니메이터가 존재하지 않습니다.");

        lastBossAction = GetComponent<LastBossAction>();
        Debug.Assert(animator != null, "Error (Null Reference) : 최종 보스 액션 씬 컴포넌트가 존재하지 않습니다.");
    }

    public IEnumerator StartAction()
    {
        isGroundPound = true;
        wing.enabled = false;
        lastBossAction.isAction = true;

        // 지면 공격 포즈를 완벽하게 지을때가지 대기
        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f)
            {
                animator.enabled = false;
                break;
            }
            
            yield return null;
        }

        // 천천히 위로 올라감
        float elapsedTime = 0f, maxSpeed = 0;
        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (transform.position.y >= 50)
                break;

            maxSpeed = Mathf.Max(upSpeed.Evaluate(elapsedTime), maxSpeed);
            transform.position += Vector3.up * maxSpeed;


            yield return null;
        }

        float startTime = Time.time;
        Vector3 defaultRotation = transform.rotation.eulerAngles;

        // 천천히 아래 방향으로 회전
        while (true)
        {
            elapsedTime = (Time.time - startTime) / rotateTime;

            if (elapsedTime < rotateTime)
            {
                transform.rotation = Quaternion.Lerp(
                    Quaternion.Euler(new Vector3(0, defaultRotation.y, defaultRotation.z)),
                    Quaternion.Euler(new Vector3(180, defaultRotation.y, defaultRotation.z)),
                    upSpeed.Evaluate(elapsedTime));
            }

            else
                break;

            yield return null;
        }

        startTime = Time.time;
        while (true)
        {
            elapsedTime = (Time.time - startTime) / rotateTime;

            if (elapsedTime < rotateTime)
            {
                // 공격 범위 점점 커지도록 설정
                float size = Mathf.Lerp(0, attackRangeSize, elapsedTime);

                attackRange.size = new Vector3(size, size, attackRange.size.z);
            }

            else
                break;

            yield return null;
        }

        // 빠르게 내려감
        elapsedTime = 0f; maxSpeed = 0;
        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (transform.position.y < -1.0f)
                break;

            maxSpeed = Mathf.Max(upSpeed.Evaluate(elapsedTime), maxSpeed);
            transform.position += Vector3.down * maxSpeed;

            yield return null;
        }

        // 다시 원래 상태로 돌아감
        startTime = Time.time;
        attackRange.size = new Vector3(0, 0, attackRange.size.z);

        GameObject newExplosion = Instantiate(explosion, transform);
        newExplosion.transform.parent = null;
        Destroy(newExplosion, 3f);

        while (true)
        {
            elapsedTime = (Time.time - startTime) / rotateTime;

            if (elapsedTime < rotateTime)
            {
                transform.rotation = Quaternion.Lerp(
                    Quaternion.Euler(new Vector3(180, defaultRotation.y, defaultRotation.z)),
                    Quaternion.Euler(new Vector3(0, defaultRotation.y, defaultRotation.z)),
                    upSpeed.Evaluate(elapsedTime) / 7.5f);

                transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, 10, upSpeed.Evaluate(elapsedTime) / 7.5f), transform.position.z);
            }

            else
                break;

            yield return null;
        }

        animator.enabled = true;
        wing.enabled = true;
        isGroundPound = false;
    }
}