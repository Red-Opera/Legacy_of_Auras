using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    public static Transform targetMonster;                      // 현재 조준하고 있는 몬스터

    [SerializeField] private Camera camera;                     // 플레이어 카메라
    [SerializeField] private GameObject bow;                    // 플레이어 활
    [SerializeField] private GameObject arrow;                  // 화살 오브젝트
    [SerializeField] private GameObject gun;                    // 플레이어 총
    [SerializeField] private GameObject bullet;                 // 총알 오브젝트
    [SerializeField] private GameObject aurasArrow;             // 오라의 화살
    [SerializeField] private GameObject emptyCartridge;         // 탄피 배출 이펙트
    [SerializeField] private PlayerGunShotUI gunUI;             // 총 UI을 관리하는 오브젝트
    [SerializeField] private TextMeshProUGUI currentGunText;    // 탄창에 남은 총알의 개수
    [SerializeField] private AudioClip gunSoundClip;            // 총 소리 클립
    [SerializeField] private AudioClip noRemainBullet;          // 총알이 없을때 나는 소리
    [SerializeField] private Transform camaraToMove;            // 카메라가 이동할 위치
    [SerializeField] private Transform defCameraTrans;          // 기본 카메라 위치

    [SerializeField] private float cmrMoveTime;                 // 카메라가 목표지점까지 이동하는데 걸리는 시간
    [SerializeField] private float spawnRadius = 3f;            // 스폰 범위
    [SerializeField] private float spawnDelay = 0.1f;           // 스폰 딜레이
    [SerializeField] private float detectionRadius = 50.0f;     // 몬스터 탐색 반경
    [SerializeField] private float fieldOfView = 60.0f;         // 시야각

    public WeaponType weaponType;               // 현재 소유하고 있는 무기
    private Animator animator;                  // 플레이어의 애니메이터
    private ParticleSystem gunFlash;            // 총 쏠때 번쩍임
    private PlayerGunReLoad reLoad;             // 총 로드 스크립트

    private float cameraMoveStartTime;          // 카메라 이동 시작 시간
    private bool isCameraMoving = false;        // 카메라 이동 중인지 여부
    private bool isReady = false;               // 현재 화살을 준비하는지 여부
    private bool isAurasAttack = false;         // 현재 오라의 공격하고 있는지 여부

    public void Start()
    {
        animator = GetComponent<Animator>();

        Debug.Assert(GetComponent<PlayerWeaponChanger>() != null, "Error (Null Reference) : 플레이어 무기 변화 컴포넌트가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이션 컴포넌트가 존재하지 않습니다.");
        Debug.Assert(bow != null, "Error (Null Reference) : 활이 존재하지 않습니다.");
        Debug.Assert(gun != null, "Error (Null Reference) : 총이 존재하지 않습니다.");
        Debug.Assert(arrow != null, "Error (Null Reference) : 화살이 존재하지 않습니다.");
        Debug.Assert(gunUI != null, "Error (Null Reference) : 총알 수를 관리하는 컴포넌트가 존재하지 않습니다.");

        gunFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();

        Debug.Assert(gunFlash != null, "Error (Null Reference) : 총 번쩍 이펙트가 존재하지 않습니다.");
        Debug.Assert(emptyCartridge != null, "Error (Null Reference) : 탄피 이펙트가 존재하지 않습니다.");

        reLoad = GetComponent<PlayerGunReLoad>();
        Debug.Assert(reLoad != null, "Error (Null Reference) : 플레이어의 총 로드 컴포넌트 존재하지 않습니다.");
    }

    public void Update()
    {
        // 현재 아직 최종보스 영화가 끝나지 않았거나 플레이어가 죽은 경우
        if (!BossSceneFilm.isFilmEnd || PlayerHPBar.isPlayerDeath)
            return;

        // 지속적으로 어떠한 무기를 갖고 있는지 확인
        weaponType = GetComponent<PlayerWeaponChanger>().weaponType;

        // 활을 장착하고 있고 마우스를 누르고 있을동안 화살을 쏠 준비를 함 (단, 플레이어가 아직 NPC와 대화 중인 경우 공격할 수 없음)
        if (ChatNPC.isEnd && Input.GetMouseButton(0) && weaponType == WeaponType.BOW)
        {
            if (!(isCameraMoving || animator.GetBool("ArrowReady")))     // 카메라가 이동 중이거나 화살이 준비되었을 때 중지
                StartCoroutine(BowAttack());
        }

        else if (weaponType == WeaponType.BOW)
            AttackCancel();

        else if (Input.GetMouseButtonDown(0) && weaponType == WeaponType.GUN)
            GunAttack();

        else if (Input.GetMouseButtonDown(0) && weaponType == WeaponType.AURAS && !isAurasAttack)
            StartCoroutine(AurasAttack());

        // 카메라 이동할 수 있는지 확인하여 이동할 수 있을 경우 이동
        if (isCameraMoving)
            CameraHasMove(isReady);
    }

    // 화살 공격 메소드
    private IEnumerator BowAttack()
    {
        // 화살을 쏠 준비하는 애니메이션 실행
        animator.SetBool("ArrowReady", true);
        isReady = true;

        // 화살쏘는데 잘보이도록 카메라 이동
        CameraMove();
        StartCoroutine(FindClosestMonster());

        yield return new WaitForSeconds(1.0f);

        if (!Input.GetMouseButton(0))
            yield break;

        GameObject newBow = Instantiate(arrow, bow.transform.GetChild(0).transform);
        newBow.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        newBow.transform.localPosition = new Vector3(-6.5f, 0.5f, -0.5f);

        // 소리 재생
        AudioSource bowAudioSource = bow.GetComponent<AudioSource>();
        AudioClip clip = bowAudioSource.clip;
        bowAudioSource.volume = GameManager.info.soundVolume;
        bowAudioSource.PlayOneShot(clip);
    }

    // 총 공격 메소드
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

    // 오라의 화살 공격
    private IEnumerator AurasAttack()
    {
        isAurasAttack = true;
        animator.SetTrigger("CreateAuras");

        // 화살 개수만큼 반복
        for (int i = 0; i < 3; i++)
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

                if ((createPosition - transform.position).magnitude > 1)
                    break;
            }

            // 화살 생성
            GameObject newArrow = Instantiate(aurasArrow, createPosition, Quaternion.identity);
            Destroy(newArrow.GetComponent<ArrowTrace>());
            newArrow.GetComponent<PlayerAurasArrow>().SetTarget();
            newArrow.tag = "PlayerAttack";

            newArrow.transform.localScale *= 0.5f;

            Destroy(newArrow, 20.0f);

            // 다음 화살을 생성할 때까지 대기
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

    // 카메라 이동 설정 메소드
    private void CameraMove()
    {
        // 카메라 이동 시작 설정
        cameraMoveStartTime = Time.time;
        isCameraMoving = true;
    }

    // 카메라 이동 메소드
    private void CameraHasMove(bool ready)
    {
        // 카메라가 이동한 시간, 이동완료한 퍼센트
        float elapsedTime = Time.time - cameraMoveStartTime;
        float t = elapsedTime / cmrMoveTime;

        // 화살을 준비하는 경우
        if (ready)
        {
            // 시간에 따라서 자연스럽게 카메라 이동
            camera.transform.localPosition = Vector3.Lerp(defCameraTrans.localPosition, camaraToMove.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(defCameraTrans.localRotation, camaraToMove.localRotation, t * 2);
        }
        
        else
        {
            camera.transform.localPosition = Vector3.Lerp(camaraToMove.localPosition, defCameraTrans.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(camaraToMove.localRotation, defCameraTrans.localRotation, t * 2);
        }

        // 카메라가 이동완료한 경우 카메라 이동 중지
        if (t >= cmrMoveTime)
            isCameraMoving = false;
    }

    private IEnumerator FindClosestMonster()
    {
        // 탐색 반경 내의 모든 객체를 가져옴
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestMonster = null;        // 가장 가까운 몬스터의 Transform

        // 시야각의 절반 값을 계산
        float halfFieldOfView = fieldOfView / 2.0f;

        targetMonster = null;

        // 체력 낮은 순, 거리 가까운 순으로 반한하는 우선 순위 큐
        PriorityQueue<IntFloatChar32> findCloseMonster = PriorityQueue<IntFloatChar32>.Create();
        Dictionary<string, Transform> monsterToName = new Dictionary<string, Transform>();

        // 탐색 반경 내의 각 객체를 검사
        foreach (Collider obj in objectsInRange)
        {
            if (obj.CompareTag("Monster"))
            {
                // 객체와의 방향 벡터를 계산하고 플레이어 앞 방향과 객체와의 각도를 계산
                Vector3 directionToMonster = (obj.transform.position - transform.position).normalized;
                float angleToMonster = Vector3.Angle(transform.forward, directionToMonster);

                // 객체가 시야각 내에 있는지 확인
                if (angleToMonster <= halfFieldOfView)
                {
                    // 객체와의 거리를 계산
                    float distanceToMonster = Vector3.Distance(transform.position, obj.transform.position);

                    MonsterHPBar hp = obj.GetComponent<MonsterHPBar>();

                    if (hp == null && obj.transform.parent != null)
                        hp = obj.transform.parent.GetComponent<MonsterHPBar>();

                    if (hp != null)
                    {
                        if (hp.currentHP <= 0)
                            continue;

                        string objName = (obj.name.Length >= 5 ? obj.name[..5] : obj.name) + "("+ obj.GetInstanceID() + ")";

                        // 체력, 거리 순 큐로 입력
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

        // 체력, 거리 순으로 작은 몬스터 정보를 가져옴
        IntFloatChar32 clostMonsterInfo = findCloseMonster.top();

        if (clostMonsterInfo.c == "")
            yield break;

        // 해당 이름의 몬스터를 반환함
        closestMonster = monsterToName[clostMonsterInfo.c];

        // 가장 가까운 몬스터의 crossHair를 선택
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