using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PhilosopherState : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // 스킨

    public int maxAttackType = 3;           // 공격 타입의 개수
    public float waitMinTime = 1.0f;        // 공격 후 최소 대기시간
    public float waitMaxTime = 5.0f;        // 공격 후 최대 대기시간
    public float nextDelay = 5.0f;          // 다음 공격 대기 시간
    public float amplitudePersent = 15.0f;  // 다음 공격 대기 시간 진폭

    private MonsterHPBar hpBar;             // HP 바 스크립트
    private Animator animator;
    private Rigidbody rigidbody;            
    private AnimatorStateInfo beforeState;  // 이전 애니메이션 상태

    private bool isAction = false;          // 해당 상태의 활동을 했는지 여부
    private bool isIdle = false;            // 현재 Idle 상태인 여부
    private bool isDeath = false;           // 현재 죽은 판정이 된 여부
    private bool isCollision = false;       // 어떠한 것과 충돌했는지 여부
    private bool isPlayerCollision = false; // 플레이어와 충돌했는지 여부

    public GameObject zombie;               // 좀비 오브젝트
    public GameObject bloodEffect;          // 좀비 생성 피 이펙트

    public int createMonsterCount = 3;      // 생성되는 몬스터 수
    public float spawnRadius = 10f;         // 좀비 스폰 범위

    public float healPersent = 10f;         // 체력 회복 퍼센트

    public GameObject arrow;                // 화살 오브젝트
    public int createArrowCount = 8;        // 생성되는 화살 개수
    public float spawnDelay = 0.25f;        // 스폰 딜레이

    public ShieldEffect shieldEffect;       // 쉴드 이펙트

    public GameObject player;               // 플레이어 오브젝트

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 애니메이터가 존재하지 않습니다.");
        hpBar = GetComponent<MonsterHPBar>();
        Debug.Assert(hpBar != null, "Error (Null Reference) : 체력 바 컴포넌트가 존재하지 않습니다.");

        Debug.Assert(zombie != null, "Error (Null Reference) : 생성할 좀비 오브젝트가 존재하지 않습니다.");
        Debug.Assert(bloodEffect != null, "Error (Null Reference) : 생성시 좀비 피 이펙트가 존재하지 않습니다.");

        player = GameObject.Find("MonsterTarget");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어 오브젝트가 존재하지 않습니다.");

        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null, "Error (Null Reference) : 물리 컴포넌트가 존재하지 않습니다.");

        StartCoroutine(ChangeAttackType());
    }

    void Update()
    {
        if (!isPlayerCollision)
            transform.LookAt(player.transform);

        if (transform.position.y < 0)
            transform.position += new Vector3(0, Time.deltaTime, 0);

        AnimatorStateInfo nowState = animator.GetCurrentAnimatorStateInfo(0);

        if (nowState.IsName("Death") && isDeath)
            return;

        if (nowState.fullPathHash != beforeState.fullPathHash)
        {
            beforeState = nowState;
            isAction = false;
        }

        if (isAction)
            return;

        if (!isIdle)
            StartCoroutine(NextAttack());

        // 좀비 생성 이벤트일 경우
        if (nowState.IsName("CreateZombie"))
            CreateZombie();

        // 화살 생성 이벤트일 경우
        else if (nowState.IsName("CreateAurasArrow"))
            StartCoroutine(CreateArrow());

        // 힐 이벤트일 경우
        else if (nowState.IsName("Heal"))
            Heal();

        // 죽은 이벤트일 경우
        else if (nowState.IsName("Death"))
            StartCoroutine(DeathTrigger(nowState));

        if (nowState.IsName("MoveAttack"))
            MoveAttack();

        else
        {
            if (isCollision)
                transform.position += new Vector3(0, Time.deltaTime, 0);
        }
    }

    // 일정시간 동안 대기한 후 공격을 할 수 있게 해주는 메소드
    private IEnumerator ChangeAttackType()
    {
        bool isChange = false;

        while (true)
        {
            AnimatorStateInfo nowState = animator.GetCurrentAnimatorStateInfo(0);

            if (nowState.normalizedTime >= 0.8f && !isChange)
            {
                // 0부터 공격 개수 - 1까지의 랜덤한 AttackType 설정
                int randomAttackType = Random.Range(0, maxAttackType);
                while (randomAttackType == animator.GetInteger("AttackType"))
                    randomAttackType = Random.Range(0, maxAttackType);

                animator.SetInteger("AttackType", randomAttackType);

                isChange = true;
            }

            if (animator.IsInTransition(0) && nowState.IsName("Idle"))
                isChange = false;

            yield return null;
        }
    }

    // 좀비를 생성하는 메소드
    private void CreateZombie()
    {
        isAction = true;

        for (int i = 0; i < 3; i++)
        {
            float angle = Random.Range(0f, 2f * Mathf.PI);
            float rangeX = Random.Range(5.0f, spawnRadius);
            float rangeZ = Random.Range(5.0f, spawnRadius);

            float x = transform.position.x + rangeX * Mathf.Cos(angle);
            float z = transform.position.z + rangeZ * Mathf.Sin(angle);

            Vector3 spawnPosition = new Vector3(x, 1000f, z);

            NavMeshHit hit;
            GameObject newZombie = null;

            if (NavMesh.SamplePosition(spawnPosition, out hit, 1000f, NavMesh.AllAreas))
                newZombie = Instantiate(zombie, hit.position, Quaternion.identity);
            
            else
            {
                i--;
                continue;
            }

            Ray ray = new Ray(newZombie.transform.position, Vector3.down);
            RaycastHit hit2;

            int failCount = 0;
            while (true)
            {
                if (failCount >= 100)
                    break;

                if (Physics.Raycast(ray, out hit2, Mathf.Infinity))
                {
                    if (hit2.collider.CompareTag("Terrain"))
                    {
                        newZombie.transform.position = hit2.point;
                        Destroy(Instantiate(bloodEffect, hit2.point, Quaternion.identity), 5.0f);
                        break;
                    }
                }

                angle = Random.Range(0f, 2f * Mathf.PI);
                rangeX = Random.Range(5.0f, spawnRadius);
                rangeZ = Random.Range(5.0f, spawnRadius);

                x = transform.position.x + rangeX * Mathf.Cos(angle);
                z = transform.position.z + rangeZ * Mathf.Sin(angle);

                spawnPosition = new Vector3(x, 1000f, z);
                newZombie.transform.position = spawnPosition;

                ray = new Ray(newZombie.transform.position, Vector3.down);
                failCount++;
            }
        }
    }

    // 화살 생성 메소드
    private IEnumerator CreateArrow()
    {
        yield break;

        isAction = true;

        // 화살 개수만큼 반복
        for (int i = 0; i < createArrowCount; i++) 
        {
            float randomX = 0, randomY = 0, randomZ = 0;

            // 생성할 위치를 지정함
            Vector3 createPosition = new Vector3();
            while (true)
            {
                randomX = Random.Range(0, spawnRadius);
                randomY = Random.Range(-1, spawnRadius);
                randomZ = Random.Range(-spawnRadius, spawnRadius);

                createPosition = new Vector3(
                transform.position.x + randomX,
                transform.position.y + randomY,
                transform.position.z + randomZ);

                if ((createPosition - transform.position).magnitude > 5)
                    break;
            }

            // 화살 생성
            GameObject newArrow = Instantiate(arrow, createPosition, Quaternion.identity);
            newArrow.GetComponent<PlayerAurasArrow>().enabled = false;

            // 다음 화살을 생성할 때까지 대기
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // 체력 증가 메소드
    private void Heal()
    {
        isAction = true;

        // 피해를 입은 값을 가져옴
        int damaged = hpBar.maxHP - hpBar.currentHP;

        // 체력 감소한 값에 비례하여 체력을 상승시킴
        hpBar.Heal((int)(damaged * (healPersent / 100)));

        shieldEffect.gameObject.SetActive(true);

        shieldEffect.FillShieldHp();
        shieldEffect.renderer.material.SetFloat("_CurrentHP", 1);
        if (shieldEffect.renderer.material.GetFloat("_Disolve") >= 0.9)
            shieldEffect.OpenCloseShield();
    }

    // 이동하면서 공격하는 메소드
    private void MoveAttack()
    {
        Vector3 newForward = transform.forward;
        if (isCollision)
            newForward.y = 0;

        transform.position += newForward * Time.deltaTime * 20.0f;
    }

    // 다음 공격을 위해 대기하는 메소드
    private IEnumerator NextAttack()
    {
        AnimatorStateInfo nowState = animator.GetCurrentAnimatorStateInfo(0);

        if (!nowState.IsName("Idle"))
            yield break;

        isIdle = true;

        int nextState = animator.GetInteger("AttackType");
        animator.SetInteger("AttackType", 99);

        float waitTime = nextDelay + Random.Range(-(amplitudePersent / 100) * nextDelay, (amplitudePersent / 100) * nextDelay);
        if (nowState.IsName("MoveAttack"))
            waitTime += 5.0f;
        yield return new WaitForSeconds(waitTime);

        animator.SetInteger("AttackType", nextState);

        isIdle = false;
    }

    // 죽었을 때 발생하는 메소드
    private IEnumerator DeathTrigger(AnimatorStateInfo info)
    {
        isDeath = true;
        Destroy(gameObject, info.length + 0.5f);

        while (true)
        {
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                if (skinnedMeshRenderer.materials[i].HasFloat("_Persent"))
                {
                    float currentPersent = skinnedMeshRenderer.materials[i].GetFloat("_Persent");

                    if (currentPersent <= 0)
                        yield break;

                    skinnedMeshRenderer.materials[i].SetFloat("_Persent", currentPersent - 0.02f);
                }
            }

            yield return new WaitForSeconds(info.length / 50.0f);
        }
    }

    // 이동 공격후 플레이어와 충돌했을 경우 
    private IEnumerator ChagePlayerCollision()
    {
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0f;
        while (elapsedTime < 2.0f)
        {
            Vector3 direction = player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / 2.0f);

            yield return null;
        }

        isPlayerCollision = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {
            isCollision = true;
            transform.position += new Vector3(0f, Time.deltaTime, 0f);
        }

        if (other.tag == "Player")
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("MoveAttack"))
            {
                isPlayerCollision = true;
                StartCoroutine(ChagePlayerCollision());
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Terrain")
        {
            isCollision = true;
            transform.position += new Vector3(0f, Time.deltaTime, 0f);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain")
        {
            isCollision = false;
        }
    }
}