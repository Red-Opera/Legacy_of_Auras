using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class LastBossAction : MonoBehaviour
{
    [HideInInspector] public bool isAction = false;          // 해당 상태의 활동을 했는지 여부

    public SkinnedMeshRenderer skinnedMeshRenderer; // 스킨

    public int maxAttackType = 8;           // 공격 타입의 개수
    public float waitMinTime = 1.0f;        // 공격 후 최소 대기시간
    public float waitMaxTime = 5.0f;        // 공격 후 최대 대기시간
    public float nextDelay = 5.0f;          // 다음 공격 대기 시간
    public float amplitudePersent = 15.0f;  // 다음 공격 대기 시간 진폭

    private LassBossHpBar hpBar;            // HP 바 스크립트
    private Animator animator;
    private Rigidbody rigidbody;
    private AnimatorStateInfo beforeState;  // 이전 애니메이션 상태

    private bool isIdle = false;            // 현재 Idle 상태인 여부
    private bool isDeath = false;           // 현재 죽은 판정이 된 여부
    private bool isCollision = false;       // 어떠한 것과 충돌했는지 여부
    private bool isMoveAttackLook = false;  // 한번 플레이어를 본적이 있는 여부
    private bool isCreateAXE = false;       // 한번 도끼를 생성한 적이 있는 여부

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

    [SerializeField] private LastBossWingPosition wing;     // 날개를 관리하는 스크립트
    [SerializeField] private GameObject fireAXE;            // 불타는 도끼 오브젝트
    private LastBossGroundPound groundPound;                // 지면 강타할 수 있도록하는 스크립트
    private LastBossPixelate pixelate;                      // 픽셀화하는 스크립트
    private MeshCollider collider;                          // 콜라이더 컴포넌트

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 애니메이터가 존재하지 않습니다.");
        hpBar = GetComponent<LassBossHpBar>();
        Debug.Assert(hpBar != null, "Error (Null Reference) : 체력 바 컴포넌트가 존재하지 않습니다.");

        Debug.Assert(zombie != null, "Error (Null Reference) : 생성할 좀비 오브젝트가 존재하지 않습니다.");
        Debug.Assert(bloodEffect != null, "Error (Null Reference) : 생성시 좀비 피 이펙트가 존재하지 않습니다.");

        player = GameObject.Find("MonsterTarget");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어 오브젝트가 존재하지 않습니다.");

        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null, "Error (Null Reference) : 물리 컴포넌트가 존재하지 않습니다.");

        groundPound = GetComponent<LastBossGroundPound>();
        Debug.Assert(groundPound != null, "Error (Null Reference) : 지면 강타 컴포넌트가 존재하지 않습니다.");

        pixelate = GetComponent<LastBossPixelate>();
        Debug.Assert(pixelate != null, "Error (Null Reference) : 픽셀화 컴포넌트가 존재하지 않습니다.");

        Debug.Assert(wing != null, "Error (Null Reference) : 날개 스크립트가 존재하지 않습니다.");

        collider = GetComponent<MeshCollider>();
        Debug.Assert(collider != null, "Error (Null Reference) : 충돌 스크립트가 존재하지 않습니다.");

        StartCoroutine(ChangeAttackType());
    }

    void Update()
    {
        // 아직 보스 씬이 끝나지 않은 경우 어떠한 행동을 하지 않음
        if (!BossSceneFilm.isFilmEnd || groundPound.isGroundPound)
            return;

        // 너무 아래로 내려갔을 경우 위로 올라가도록 설정
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

        if (nowState.IsName("Idle"))
            LookAtPlayer();

        if (!isIdle)
            StartCoroutine(NextAttack());

        // 불타는 도끼 생성 이벤트일 경우
        if (nowState.IsName("AXECreate"))
            CreateAXEEffect();

        else
            isCreateAXE = false;

        // 좀비 생성 이벤트일 경우
        if (nowState.IsName("CreateZombie"))
            CreateZombie();

        // 화살 생성 이벤트일 경우
        else if (nowState.IsName("CreateAurasArrow"))
            StartCoroutine(CreateArrow());

        // 힐 이벤트일 경우
        else if (nowState.IsName("Heal"))
            Heal();

        // 픽셀화 시키는 이벤트일 경우
        else if (nowState.IsName("Pixelate"))
            StartCoroutine(pixelate.PixelToggle());

        // 지면 강타 공격일 경우
        else if (nowState.IsName("GroundPound"))
            StartCoroutine(groundPound.StartAction());

        // 플레이어에게 다가가 공격하는 공격일 경우
        else if (nowState.IsName("BurningFlameSlash"))
            StartCoroutine(BurningFlameSlash());

        // 죽은 이벤트일 경우
        else if (nowState.IsName("Death"))
            StartCoroutine(DeathTrigger(nowState));

        if (nowState.IsName("MoveAttack"))
            MoveAttack();

        else
            isMoveAttackLook = false;
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

            Vector3 spawnPosition = new(x, 15f, z);

            GameObject newZombie = Instantiate(zombie, spawnPosition, Quaternion.identity);

            if (SceneManager.GetActiveScene().name == "Final")
                newZombie.transform.localScale *= 3;

            Ray ray = new(newZombie.transform.position, Vector3.down);

            int failCount = 0;
            while (true)
            {
                if (failCount >= 100)
                    break;

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("Terrain"))
                    {
                        newZombie.transform.position = hit.point;
                        Destroy(Instantiate(bloodEffect, hit.point, Quaternion.identity), 5.0f);
                        break;
                    }
                }

                angle = Random.Range(0f, 2f * Mathf.PI);
                rangeX = Random.Range(5.0f, spawnRadius);
                rangeZ = Random.Range(5.0f, spawnRadius);

                x = transform.position.x + rangeX * Mathf.Cos(angle);
                z = transform.position.z + rangeZ * Mathf.Sin(angle);

                spawnPosition = new Vector3(x, 15f, z);
                newZombie.transform.position = spawnPosition;

                ray = new Ray(newZombie.transform.position, Vector3.down);
                failCount++;
            }
        }
    }

    // 화살 생성 메소드
    private IEnumerator CreateArrow()
    {
        isAction = true;

        // 화살 개수만큼 반복
        for (int i = 0; i < createArrowCount; i++)
        {
            float randomX, randomY, randomZ;

            // 생성할 위치를 지정함
            Vector3 createPosition;
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
        collider.enabled = false;
        wing.isFlying = false;

        if (!isMoveAttackLook)
        {
            transform.LookAt(player.transform);
            isMoveAttackLook = true;
        }

        Vector3 newForward = transform.forward;
        if (isCollision)
            newForward.y = 0;

        transform.position += newForward * Time.deltaTime * 35f;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.normalizedTime >= 0.95f || !state.IsName("MoveAttack"))
        {
            wing.isFlying = true;
            collider.enabled = true;
        }
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
    }

    // 플레이어를 천천히 바라보도록 하는 메소드
    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
    }

    // 도끼를 생성하는 메소드
    private void CreateAXEEffect()
    {
        if (isCreateAXE)
            return;

        isCreateAXE = true;

        List<GameObject> newAxe = new List<GameObject>();

        for (int i = 0; i < 3; i++)
            newAxe.Add(Instantiate(fireAXE, transform));

        newAxe[0].transform.localPosition = new Vector3(0.6f, 0.5f, 0);
        newAxe[1].transform.localPosition = new Vector3(0, 1.0f, 0.0f);
        newAxe[2].transform.localPosition = new Vector3(-0.6f, 0.5f, 0.0f);

        for (int i = 0; i < 3; i++)
        {
            newAxe[i].transform.localScale = new Vector3(15.0f, 15.0f, 15.0f);
            newAxe[i].transform.SetParent(null);
            newAxe[i].transform.LookAt(player.transform);
        }
    }

    // 플레이어에게 다가가 공격하는 메소드
    private IEnumerator BurningFlameSlash()
    {
        isAction = true;

        // 플레이어 쪽으로 이동하고 이동하는동안 애니메이터를 몀춤
        Vector3 lookAtPlayer = player.transform.position - transform.position;
        animator.speed = 0;

        while (true)
        {
            float distance = (transform.position - player.transform.position).magnitude;

            if (distance < 5)
                break;

            transform.position += lookAtPlayer * Time.deltaTime * 2;

            yield return null;
        }
        animator.speed = 1;
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
                StartCoroutine(ChagePlayerCollision());
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
