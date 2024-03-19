using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class LastBossAction : MonoBehaviour
{
    [HideInInspector] public bool isAction = false;          // �ش� ������ Ȱ���� �ߴ��� ����

    public SkinnedMeshRenderer skinnedMeshRenderer; // ��Ų

    public int maxAttackType = 8;           // ���� Ÿ���� ����
    public float waitMinTime = 1.0f;        // ���� �� �ּ� ���ð�
    public float waitMaxTime = 5.0f;        // ���� �� �ִ� ���ð�
    public float nextDelay = 5.0f;          // ���� ���� ��� �ð�
    public float amplitudePersent = 15.0f;  // ���� ���� ��� �ð� ����

    private LassBossHpBar hpBar;            // HP �� ��ũ��Ʈ
    private Animator animator;
    private Rigidbody rigidbody;
    private AnimatorStateInfo beforeState;  // ���� �ִϸ��̼� ����

    private bool isIdle = false;            // ���� Idle ������ ����
    private bool isDeath = false;           // ���� ���� ������ �� ����
    private bool isCollision = false;       // ��� �Ͱ� �浹�ߴ��� ����
    private bool isMoveAttackLook = false;  // �ѹ� �÷��̾ ������ �ִ� ����
    private bool isCreateAXE = false;       // �ѹ� ������ ������ ���� �ִ� ����

    public GameObject zombie;               // ���� ������Ʈ
    public GameObject bloodEffect;          // ���� ���� �� ����Ʈ

    public int createMonsterCount = 3;      // �����Ǵ� ���� ��
    public float spawnRadius = 10f;         // ���� ���� ����

    public float healPersent = 10f;         // ü�� ȸ�� �ۼ�Ʈ

    public GameObject arrow;                // ȭ�� ������Ʈ
    public int createArrowCount = 8;        // �����Ǵ� ȭ�� ����
    public float spawnDelay = 0.25f;        // ���� ������

    public ShieldEffect shieldEffect;       // ���� ����Ʈ

    public GameObject player;               // �÷��̾� ������Ʈ

    [SerializeField] private LastBossWingPosition wing;     // ������ �����ϴ� ��ũ��Ʈ
    [SerializeField] private GameObject fireAXE;            // ��Ÿ�� ���� ������Ʈ
    private LastBossGroundPound groundPound;                // ���� ��Ÿ�� �� �ֵ����ϴ� ��ũ��Ʈ
    private LastBossPixelate pixelate;                      // �ȼ�ȭ�ϴ� ��ũ��Ʈ
    private MeshCollider collider;                          // �ݶ��̴� ������Ʈ

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
        hpBar = GetComponent<LassBossHpBar>();
        Debug.Assert(hpBar != null, "Error (Null Reference) : ü�� �� ������Ʈ�� �������� �ʽ��ϴ�.");

        Debug.Assert(zombie != null, "Error (Null Reference) : ������ ���� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(bloodEffect != null, "Error (Null Reference) : ������ ���� �� ����Ʈ�� �������� �ʽ��ϴ�.");

        player = GameObject.Find("MonsterTarget");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾� ������Ʈ�� �������� �ʽ��ϴ�.");

        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null, "Error (Null Reference) : ���� ������Ʈ�� �������� �ʽ��ϴ�.");

        groundPound = GetComponent<LastBossGroundPound>();
        Debug.Assert(groundPound != null, "Error (Null Reference) : ���� ��Ÿ ������Ʈ�� �������� �ʽ��ϴ�.");

        pixelate = GetComponent<LastBossPixelate>();
        Debug.Assert(pixelate != null, "Error (Null Reference) : �ȼ�ȭ ������Ʈ�� �������� �ʽ��ϴ�.");

        Debug.Assert(wing != null, "Error (Null Reference) : ���� ��ũ��Ʈ�� �������� �ʽ��ϴ�.");

        collider = GetComponent<MeshCollider>();
        Debug.Assert(collider != null, "Error (Null Reference) : �浹 ��ũ��Ʈ�� �������� �ʽ��ϴ�.");

        StartCoroutine(ChangeAttackType());
    }

    void Update()
    {
        // ���� ���� ���� ������ ���� ��� ��� �ൿ�� ���� ����
        if (!BossSceneFilm.isFilmEnd || groundPound.isGroundPound)
            return;

        // �ʹ� �Ʒ��� �������� ��� ���� �ö󰡵��� ����
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

        // ��Ÿ�� ���� ���� �̺�Ʈ�� ���
        if (nowState.IsName("AXECreate"))
            CreateAXEEffect();

        else
            isCreateAXE = false;

        // ���� ���� �̺�Ʈ�� ���
        if (nowState.IsName("CreateZombie"))
            CreateZombie();

        // ȭ�� ���� �̺�Ʈ�� ���
        else if (nowState.IsName("CreateAurasArrow"))
            StartCoroutine(CreateArrow());

        // �� �̺�Ʈ�� ���
        else if (nowState.IsName("Heal"))
            Heal();

        // �ȼ�ȭ ��Ű�� �̺�Ʈ�� ���
        else if (nowState.IsName("Pixelate"))
            StartCoroutine(pixelate.PixelToggle());

        // ���� ��Ÿ ������ ���
        else if (nowState.IsName("GroundPound"))
            StartCoroutine(groundPound.StartAction());

        // �÷��̾�� �ٰ��� �����ϴ� ������ ���
        else if (nowState.IsName("BurningFlameSlash"))
            StartCoroutine(BurningFlameSlash());

        // ���� �̺�Ʈ�� ���
        else if (nowState.IsName("Death"))
            StartCoroutine(DeathTrigger(nowState));

        if (nowState.IsName("MoveAttack"))
            MoveAttack();

        else
            isMoveAttackLook = false;
    }

    // �����ð� ���� ����� �� ������ �� �� �ְ� ���ִ� �޼ҵ�
    private IEnumerator ChangeAttackType()
    {
        bool isChange = false;

        while (true)
        {
            AnimatorStateInfo nowState = animator.GetCurrentAnimatorStateInfo(0);

            if (nowState.normalizedTime >= 0.8f && !isChange)
            {
                // 0���� ���� ���� - 1������ ������ AttackType ����
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

    // ���� �����ϴ� �޼ҵ�
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

    // ȭ�� ���� �޼ҵ�
    private IEnumerator CreateArrow()
    {
        isAction = true;

        // ȭ�� ������ŭ �ݺ�
        for (int i = 0; i < createArrowCount; i++)
        {
            float randomX, randomY, randomZ;

            // ������ ��ġ�� ������
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

            // ȭ�� ����
            GameObject newArrow = Instantiate(arrow, createPosition, Quaternion.identity);
            newArrow.GetComponent<PlayerAurasArrow>().enabled = false;

            // ���� ȭ���� ������ ������ ���
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // ü�� ���� �޼ҵ�
    private void Heal()
    {
        isAction = true;

        // ���ظ� ���� ���� ������
        int damaged = hpBar.maxHP - hpBar.currentHP;

        // ü�� ������ ���� ����Ͽ� ü���� ��½�Ŵ
        hpBar.Heal((int)(damaged * (healPersent / 100)));

        shieldEffect.gameObject.SetActive(true);

        shieldEffect.FillShieldHp();
        shieldEffect.renderer.material.SetFloat("_CurrentHP", 1);
        if (shieldEffect.renderer.material.GetFloat("_Disolve") >= 0.9)
            shieldEffect.OpenCloseShield();
    }

    // �̵��ϸ鼭 �����ϴ� �޼ҵ�
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

    // ���� ������ ���� ����ϴ� �޼ҵ�
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

    // �׾��� �� �߻��ϴ� �޼ҵ�
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

    // �̵� ������ �÷��̾�� �浹���� ��� 
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

    // �÷��̾ õõ�� �ٶ󺸵��� �ϴ� �޼ҵ�
    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
    }

    // ������ �����ϴ� �޼ҵ�
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

    // �÷��̾�� �ٰ��� �����ϴ� �޼ҵ�
    private IEnumerator BurningFlameSlash()
    {
        isAction = true;

        // �÷��̾� ������ �̵��ϰ� �̵��ϴµ��� �ִϸ����͸� �m��
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
