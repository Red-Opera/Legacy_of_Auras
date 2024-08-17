using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    public static Transform targetMonster;                      // ���� �����ϰ� �ִ� ����

    [SerializeField] private Camera camera;                     // �÷��̾� ī�޶�
    [SerializeField] private GameObject bow;                    // �÷��̾� Ȱ
    [SerializeField] private GameObject arrow;                  // ȭ�� ������Ʈ
    [SerializeField] private GameObject gun;                    // �÷��̾� ��
    [SerializeField] private GameObject bullet;                 // �Ѿ� ������Ʈ
    [SerializeField] private GameObject aurasArrow;             // ������ ȭ��
    [SerializeField] private GameObject emptyCartridge;         // ź�� ���� ����Ʈ
    [SerializeField] private PlayerGunShotUI gunUI;             // �� UI�� �����ϴ� ������Ʈ
    [SerializeField] private TextMeshProUGUI currentGunText;    // źâ�� ���� �Ѿ��� ����
    [SerializeField] private AudioClip gunSoundClip;            // �� �Ҹ� Ŭ��
    [SerializeField] private AudioClip noRemainBullet;          // �Ѿ��� ������ ���� �Ҹ�
    [SerializeField] private Transform camaraToMove;            // ī�޶� �̵��� ��ġ
    [SerializeField] private Transform defCameraTrans;          // �⺻ ī�޶� ��ġ

    [SerializeField] private float cmrMoveTime;                 // ī�޶� ��ǥ�������� �̵��ϴµ� �ɸ��� �ð�
    [SerializeField] private float spawnRadius = 3f;            // ���� ����
    [SerializeField] private float spawnDelay = 0.1f;           // ���� ������
    [SerializeField] private float detectionRadius = 50.0f;     // ���� Ž�� �ݰ�
    [SerializeField] private float fieldOfView = 60.0f;         // �þ߰�

    public WeaponType weaponType;               // ���� �����ϰ� �ִ� ����
    private Animator animator;                  // �÷��̾��� �ִϸ�����
    private ParticleSystem gunFlash;            // �� �� ��½��
    private PlayerGunReLoad reLoad;             // �� �ε� ��ũ��Ʈ

    private float cameraMoveStartTime;          // ī�޶� �̵� ���� �ð�
    private bool isCameraMoving = false;        // ī�޶� �̵� ������ ����
    private bool isReady = false;               // ���� ȭ���� �غ��ϴ��� ����
    private bool isAurasAttack = false;         // ���� ������ �����ϰ� �ִ��� ����

    public void Start()
    {
        animator = GetComponent<Animator>();

        Debug.Assert(GetComponent<PlayerWeaponChanger>() != null, "Error (Null Reference) : �÷��̾� ���� ��ȭ ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ��̼� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(bow != null, "Error (Null Reference) : Ȱ�� �������� �ʽ��ϴ�.");
        Debug.Assert(gun != null, "Error (Null Reference) : ���� �������� �ʽ��ϴ�.");
        Debug.Assert(arrow != null, "Error (Null Reference) : ȭ���� �������� �ʽ��ϴ�.");
        Debug.Assert(gunUI != null, "Error (Null Reference) : �Ѿ� ���� �����ϴ� ������Ʈ�� �������� �ʽ��ϴ�.");

        gunFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();

        Debug.Assert(gunFlash != null, "Error (Null Reference) : �� ��½ ����Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(emptyCartridge != null, "Error (Null Reference) : ź�� ����Ʈ�� �������� �ʽ��ϴ�.");

        reLoad = GetComponent<PlayerGunReLoad>();
        Debug.Assert(reLoad != null, "Error (Null Reference) : �÷��̾��� �� �ε� ������Ʈ �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
        // ���� ���� �������� ��ȭ�� ������ �ʾҰų� �÷��̾ ���� ���
        if (!BossSceneFilm.isFilmEnd || PlayerHPBar.isPlayerDeath)
            return;

        // ���������� ��� ���⸦ ���� �ִ��� Ȯ��
        weaponType = GetComponent<PlayerWeaponChanger>().weaponType;

        // Ȱ�� �����ϰ� �ְ� ���콺�� ������ �������� ȭ���� �� �غ� �� (��, �÷��̾ ���� NPC�� ��ȭ ���� ��� ������ �� ����)
        if (ChatNPC.isEnd && Input.GetMouseButton(0) && weaponType == WeaponType.BOW)
        {
            if (!(isCameraMoving || animator.GetBool("ArrowReady")))     // ī�޶� �̵� ���̰ų� ȭ���� �غ�Ǿ��� �� ����
                StartCoroutine(BowAttack());
        }

        else if (weaponType == WeaponType.BOW)
            AttackCancel();

        else if (Input.GetMouseButtonDown(0) && weaponType == WeaponType.GUN)
            GunAttack();

        else if (Input.GetMouseButtonDown(0) && weaponType == WeaponType.AURAS && !isAurasAttack)
            StartCoroutine(AurasAttack());

        // ī�޶� �̵��� �� �ִ��� Ȯ���Ͽ� �̵��� �� ���� ��� �̵�
        if (isCameraMoving)
            CameraHasMove(isReady);
    }

    // ȭ�� ���� �޼ҵ�
    private IEnumerator BowAttack()
    {
        // ȭ���� �� �غ��ϴ� �ִϸ��̼� ����
        animator.SetBool("ArrowReady", true);
        isReady = true;

        // ȭ���µ� �ߺ��̵��� ī�޶� �̵�
        CameraMove();
        StartCoroutine(FindClosestMonster());

        yield return new WaitForSeconds(1.0f);

        if (!Input.GetMouseButton(0))
            yield break;

        GameObject newBow = Instantiate(arrow, bow.transform.GetChild(0).transform);
        newBow.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        newBow.transform.localPosition = new Vector3(-6.5f, 0.5f, -0.5f);

        // �Ҹ� ���
        AudioSource bowAudioSource = bow.GetComponent<AudioSource>();
        AudioClip clip = bowAudioSource.clip;
        bowAudioSource.volume = GameManager.info.soundVolume;
        bowAudioSource.PlayOneShot(clip);
    }

    // �� ���� �޼ҵ�
    private void GunAttack()
    {
        if (reLoad.isReLoad)
            return;

        AudioSource gunAudioSource = gun.GetComponent<AudioSource>();
        gunAudioSource.volume = GameManager.info.soundVolume * 0.4f;

        if (int.Parse(currentGunText.text.ToString()) <= 0)
        {
            gunAudioSource.PlayOneShot(noRemainBullet);
            return;
        }

        StartCoroutine(gunUI.ReduceBullet());
        animator.SetTrigger("FireGun");

        GameObject newBullet = Instantiate(bullet, gun.transform.GetChild(0).transform);
        newBullet.transform.localRotation = Quaternion.Euler(90.0f, 90.0f, 0.0f);
        newBullet.transform.parent = null;
        newBullet.transform.localScale = new Vector3(20.0f, 20.0f, 20.0f);

        gunAudioSource.PlayOneShot(gunSoundClip);

        if (gunFlash == null)
            gunFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();

        gunFlash.Play();

        GameObject newCartridge = Instantiate(emptyCartridge, gun.transform);
        newCartridge.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        newCartridge.transform.parent = null;

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Forest")
            newCartridge.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        else
            newCartridge.transform.localScale = Vector3.one;

        Destroy(newCartridge, 1.5f);
    }

    // ������ ȭ�� ����
    private IEnumerator AurasAttack()
    {
        isAurasAttack = true;
        animator.SetTrigger("CreateAuras");

        // ȭ�� ������ŭ �ݺ�
        for (int i = 0; i < 3; i++)
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

                if ((createPosition - transform.position).magnitude > 1)
                    break;
            }

            // ȭ�� ����
            GameObject newArrow = Instantiate(aurasArrow, createPosition, Quaternion.identity);
            Destroy(newArrow.GetComponent<ArrowTrace>());
            newArrow.GetComponent<PlayerAurasArrow>().SetTarget();
            newArrow.tag = "PlayerAttack";

            newArrow.transform.localScale *= 0.5f;

            Destroy(newArrow, 20.0f);

            // ���� ȭ���� ������ ������ ���
            yield return new WaitForSeconds(spawnDelay);
        }

        isAurasAttack = false;
    }

    private void AttackCancel()
    {
        if (isCameraMoving || !animator.GetBool("ArrowReady"))
            return;

        animator.SetBool("ArrowReady", false);
        isReady = false;

        CameraMove();
    }

    // ī�޶� �̵� ���� �޼ҵ�
    private void CameraMove()
    {
        // ī�޶� �̵� ���� ����
        cameraMoveStartTime = Time.time;
        isCameraMoving = true;
    }

    // ī�޶� �̵� �޼ҵ�
    private void CameraHasMove(bool ready)
    {
        // ī�޶� �̵��� �ð�, �̵��Ϸ��� �ۼ�Ʈ
        float elapsedTime = Time.time - cameraMoveStartTime;
        float t = elapsedTime / cmrMoveTime;

        // ȭ���� �غ��ϴ� ���
        if (ready)
        {
            // �ð��� ���� �ڿ������� ī�޶� �̵�
            camera.transform.localPosition = Vector3.Lerp(defCameraTrans.localPosition, camaraToMove.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(defCameraTrans.localRotation, camaraToMove.localRotation, t * 2);
        }
        
        else
        {
            camera.transform.localPosition = Vector3.Lerp(camaraToMove.localPosition, defCameraTrans.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(camaraToMove.localRotation, defCameraTrans.localRotation, t * 2);
        }

        // ī�޶� �̵��Ϸ��� ��� ī�޶� �̵� ����
        if (t >= cmrMoveTime)
            isCameraMoving = false;
    }

    private IEnumerator FindClosestMonster()
    {
        // Ž�� �ݰ� ���� ��� ��ü�� ������
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestMonster = null;        // ���� ����� ������ Transform

        // �þ߰��� ���� ���� ���
        float halfFieldOfView = fieldOfView / 2.0f;

        targetMonster = null;

        // ü�� ���� ��, �Ÿ� ����� ������ �����ϴ� �켱 ���� ť
        PriorityQueue<IntFloatChar32> findCloseMonster = PriorityQueue<IntFloatChar32>.Create();
        Dictionary<string, Transform> monsterToName = new Dictionary<string, Transform>();

        // Ž�� �ݰ� ���� �� ��ü�� �˻�
        foreach (Collider obj in objectsInRange)
        {
            if (obj.CompareTag("Monster"))
            {
                // ��ü���� ���� ���͸� ����ϰ� �÷��̾� �� ����� ��ü���� ������ ���
                Vector3 directionToMonster = (obj.transform.position - transform.position).normalized;
                float angleToMonster = Vector3.Angle(transform.forward, directionToMonster);

                // ��ü�� �þ߰� ���� �ִ��� Ȯ��
                if (angleToMonster <= halfFieldOfView)
                {
                    // ��ü���� �Ÿ��� ���
                    float distanceToMonster = Vector3.Distance(transform.position, obj.transform.position);

                    MonsterHPBar hp = obj.GetComponent<MonsterHPBar>();

                    if (hp == null && obj.transform.parent != null)
                        hp = obj.transform.parent.GetComponent<MonsterHPBar>();

                    if (hp != null)
                    {
                        if (hp.currentHP <= 0)
                            continue;

                        string objName = (obj.name.Length >= 5 ? obj.name[..5] : obj.name) + "("+ obj.GetInstanceID() + ")";

                        // ü��, �Ÿ� �� ť�� �Է�
                        findCloseMonster.push(new IntFloatChar32(-hp.currentHP, -distanceToMonster, objName));
                        monsterToName.Add(objName, obj.transform);
                    }

                    else
                    {
                        LastBossHpBar hpbar = obj.GetComponent<LastBossHpBar>();

                        if (hpbar != null)
                        {
                            if (hpbar.currentHP <= 0)
                                continue;

                            string objName = (obj.name.Length >= 5 ? obj.name[..5] : obj.name) + "(" + obj.GetInstanceID() + ")";

                            findCloseMonster.push(new IntFloatChar32(-hpbar.currentHP, -distanceToMonster, objName));
                            monsterToName.Add(objName, obj.transform);
                        }
                    }
                    
                }
            }
        }

        // ü��, �Ÿ� ������ ���� ���� ������ ������
        IntFloatChar32 clostMonsterInfo = findCloseMonster.top();

        if (clostMonsterInfo.c == "")
            yield break;

        // �ش� �̸��� ���͸� ��ȯ��
        closestMonster = monsterToName[clostMonsterInfo.c];

        // ���� ����� ������ crossHair�� ����
        Transform crossHair = null;
        
        if (closestMonster.name != "sk_ch_mp_GrimReaper_01")
            crossHair = closestMonster.Find("Crosshair").GetChild(0);

        else
            crossHair = closestMonster.parent.Find("Crosshair").GetChild(0);

        targetMonster = closestMonster;

        while (isReady)
        {
            if (crossHair == null)
                yield return null;

            if (!crossHair.gameObject.activeSelf)
                crossHair.gameObject.SetActive(true);

            yield return null;
        }

        crossHair.gameObject.SetActive(false);
    }
}