using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerAurasArrow : MonoBehaviour
{
    public int addDamage = 20;            // 화살의 추가 공격력

    [SerializeField] private float range = 30.0f;
    [SerializeField] private float rotationTime = 10.0f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float maxSpeedTime = 2.0f;
    [SerializeField] private GameObject bloodEffect;        // 피격 이펙트

    private GameObject player;          // 플레이어 오브젝트
    private Transform targetTransform;  // 몬스터 타켓 위치
    private MonsterHPBar monsterHpBar;  // 몬스터 체력바

    private bool isAddArrow = false;
    private float currentSpeed = 0.0f;

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Final" || sceneName == "Desert")
        {
            transform.localScale *= 3;
            maxSpeed *= 3;
        }
    }

    private void FixedUpdate()
    {
        if (!gameObject.IsDestroyed())
            transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (monsterHpBar == null || monsterHpBar.currentHP <= 0)
            SetTarget();
    }

    public void SetTarget()
    {
        // 화살이 중력에 의해 떨어지도록 설정

        if (!isAddArrow)
        {
            Rigidbody rigid = gameObject.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigid.drag = 2f;
            rigid.angularDrag = 0.0f;

            isAddArrow = true;
        }
        
        player = GameObject.Find("Model");

        Collider[] collider = Physics.OverlapSphere(player.transform.position, range);

        if (collider.Length <= 0)
            return;

        float minDistance = -1;
        int minIndex = 0;
        for (int i = 0; i < collider.Length; i++)
        {
            string tagName = collider[i].gameObject.tag;

            // 몬스터 또는 쉴드가 아닌 경우 중지
            if (!collider[i].gameObject.activeSelf || (tagName != "Monster" && tagName != "Sheld"))
                continue;

            // 화살이 날라갈 대상이 몬스터인 경우
            if (tagName == "Monster")
            {
                Transform target = collider[i].transform;
                monsterHpBar = target.GetComponent<MonsterHPBar>();

                // 부모로 계속 이동하면서 MonsterHPBar가 있는지 확인
                while (monsterHpBar == null && target.parent != null)
                {
                    target = target.parent;
                    monsterHpBar = target.GetComponent<MonsterHPBar>();
                }

                if (monsterHpBar != null)
                {
                    monsterHpBar = target.GetComponent<MonsterHPBar>();

                    // 몬스터 체력이 0인 경우 해당 몬스터로 이동하지 않음
                    if (monsterHpBar.currentHP <= 0)
                        continue;
                }
                    
                else if (monsterHpBar == null && target.GetComponent<LastBossHpBar>() != null)
                {
                    LastBossHpBar hpBar = target.GetComponent<LastBossHpBar>();

                    if (hpBar.currentHP <= 0)
                        continue;
                }
            }

            // 해당 몬스터와 거리가 얼마인지 확인
            float currentDistance = (collider[i].transform.position - player.transform.position).magnitude;

            // 해당 몬스터가 가장 가까운 경우 최소 거리로 설정
            if (currentDistance < minDistance || minDistance < 0)
            {
                minDistance = currentDistance;
                minIndex = i;
            }
        }

        if (minDistance < 0)
        {
            Destroy(gameObject);
            return;
        }

        // 가장 가까운 몬스터로 설정
        targetTransform = collider[minIndex].transform.Find("ArrowTarget").transform;

        StartCoroutine(LookAtMonster());
        StartCoroutine(AddSpeed());
    }

    private IEnumerator LookAtMonster()
    {
        while (true)
        {
            if (targetTransform.IsDestroyed())
                yield break;

            Vector3 lookAtTransform = targetTransform.position - transform.position;
            Quaternion lookAt = Quaternion.LookRotation(lookAtTransform);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookAt, rotationTime * Time.deltaTime);

            yield return null;
        }
    }

    private IEnumerator AddSpeed()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, maxSpeedTime * Time.deltaTime);
            yield return null;
        }
    }

    private void SetDamage(GameObject collisionTarget)
    {
        // 화살이 몬스터에게 맞은 경우
        if (collisionTarget.gameObject.CompareTag("Monster"))
        {
            if (collisionTarget.gameObject.transform.parent != null)
            {
                for (int i = 0; i < collisionTarget.gameObject.transform.parent.childCount; i++)
                {
                    Transform child = collisionTarget.gameObject.transform.parent.GetChild(i);
                    if (child.name == "Sheld" && child.gameObject.activeSelf)
                        return;
                }
            }

            // 충돌한 객체가 Monster 태그를 가진 경우
            Transform target = collisionTarget.transform;
            MonsterHPBar monsterHpBar = target.GetComponent<MonsterHPBar>();

            // 부모로 계속 이동하면서 MonsterHPBar가 있는지 확인
            while (monsterHpBar == null && target.parent != null)
            {
                target = target.parent;
                monsterHpBar = target.GetComponent<MonsterHPBar>();
            }

            if (monsterHpBar != null)
            {
                monsterHpBar = target.GetComponent<MonsterHPBar>();

                // MonsterHPBar 컴포넌트가 있다면 SetDamage을 실행
                monsterHpBar.SetDamage(addDamage);
            }

            else if (monsterHpBar == null && target.GetComponent<LastBossHpBar>() != null)
            {
                LastBossHpBar hpBar = target.GetComponent<LastBossHpBar>();

                hpBar.SetDamage(addDamage);
            }

            // 피격 이펙트
            GameObject effect = Instantiate(bloodEffect, transform);
            effect.transform.parent = null;

            effect.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            effect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            effect.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
            effect.transform.localRotation = Quaternion.identity;

            effect.transform.GetChild(0).GetComponent<VisualEffect>().Play();
            Destroy(effect, 60f);

            StopAllCoroutines();

            // 화살 오브젝트 제거
            Destroy(gameObject, 0.4f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        SetDamage(collision.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        SetDamage(collision.gameObject);
    }
}
