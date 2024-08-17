using System.Collections;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    public MeshRenderer renderer;

    [SerializeField] private AnimationCurve displacementCurve;
    [SerializeField] private float displacementMagitude;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float disolveSpeed;

    [SerializeField] private float createShieldHPPersent;   // 쉴드 체력 비율

    private GameObject playerCam;
    private MonsterHPBar hpBar;             // 체력을 관리하는 스크립트
    private LastBossHpBar lastBossHpBar;    // 체력을 관리하는 스크립트

    private int shieldMaxHp;                // 최대 쉴드 체력
    private int currentShieldHP;            // 현재 쉴드 체력
    private Coroutine disolveCoroutine;

    private void Start()
    {
        playerCam = GameObject.Find("Camera");
        renderer = GetComponent<MeshRenderer>();

        hpBar = transform.parent.GetComponent<MonsterHPBar>();

        if (hpBar == null)
            lastBossHpBar = transform.parent.GetComponent<LastBossHpBar>();

        if (hpBar != null)
            shieldMaxHp = (int)(hpBar.maxHP * createShieldHPPersent);

        else
            shieldMaxHp = (int)(lastBossHpBar.maxHP * createShieldHPPersent);

        currentShieldHP = shieldMaxHp;

        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        transform.forward = playerCam.transform.position - transform.position;
    }

    public void HitShield(Vector3 hitPos)
    {
        if (disolveCoroutine != null)
            return;

        renderer.material.SetVector("_HitPos", hitPos);
        StopAllCoroutines();
        StartCoroutine(HitDisplacement());
    }

    public void OpenCloseShield()
    {
        float target = 1;

        if (renderer.material.GetFloat("_Disolve") >= 0.95 && renderer.material.GetFloat("_CurrentHP") == 1)
        {
            FillShieldHp();
            target = 0;
        }

        bool isStart = false;
        if (disolveCoroutine != null && target == 1)
        {
            StopCoroutine(disolveCoroutine);
            isStart = true;
        }

        if (disolveCoroutine == null)
            isStart = true;

        if (isStart)
            disolveCoroutine = StartCoroutine(DisolveShield(target));

        else
        {
            FillShieldHp();
            renderer.material.SetFloat("_CurrentHP", 1);
        }
    }

    private IEnumerator HitDisplacement()
    {
        float lerp = 0;

        while (lerp < 1)
        {
            renderer.material.SetFloat("_DisplacementStrength", displacementCurve.Evaluate(lerp) * displacementMagitude);
            lerp += Time.deltaTime * lerpSpeed;

            yield return null;
        }
    }

    private IEnumerator DisolveShield(float target)
    {
        float start = renderer.material.GetFloat("_Disolve");
        float lerp = 0;

        while (lerp < 1)
        {
            renderer.material.SetFloat("_Disolve", Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * disolveSpeed;

            yield return null;
        }

        renderer.material.SetFloat("_Disolve", target);

        if (target == 1)
        {
            disolveCoroutine = null;
            gameObject.SetActive(false);
        }
    }

    private void IsPlayerBullet(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            if (renderer.material.GetFloat("_Disolve") >= 0.01f)
                return;

            HitShield(other.transform.position);

            int addDamage = 0;

            if (other.name.StartsWith("Arrow"))
            {
                if (other.GetComponent<PlayerArrow>() != null)
                    addDamage = other.GetComponent<PlayerArrow>().addDamage;

                else if (other.GetComponent<PlayerAurasArrow>() != null)
                    addDamage = other.GetComponent<PlayerAurasArrow>().addDamage;
            }

            else if (other.name.StartsWith("bullet"))
                addDamage = other.GetComponent<PlayerBullet>().addDamage;

            int changeHP = currentShieldHP - (GameManager.info.playerState.attack + addDamage);

            currentShieldHP = changeHP;

            if (changeHP < 0)
            {
                OpenCloseShield();
                currentShieldHP = 0;
            }

            renderer.material.SetFloat("_CurrentHP", currentShieldHP / (float)shieldMaxHp);

            Destroy(other.gameObject);
        }
    }

    public void FillShieldHp()
    {
        if (hpBar != null)
            shieldMaxHp = (int)(hpBar.maxHP * createShieldHPPersent);

        else
            shieldMaxHp = (int)(lastBossHpBar.maxHP * createShieldHPPersent);

        currentShieldHP = shieldMaxHp;
    }

    public void OnTriggerEnter(Collider other)
    {
        IsPlayerBullet(other);
    }

    public void OnTriggerStay(Collider other)
    {
        IsPlayerBullet(other);
    }

    private void FindPlayerAttack()
    {
        Collider[] coliiders = Physics.OverlapSphere(transform.position, 7.0f);

        foreach (Collider collider in coliiders)
        {
            if (collider.CompareTag("PlayerAttack"))
            {
                if (renderer.material.GetFloat("_Disolve") >= 0.01f)
                    return;

                HitShield(collider.transform.position);

                int addDamage = 0;

                if (collider.name.StartsWith("Arrow"))
                    addDamage = collider.GetComponent<PlayerArrow>().addDamage;

                else if (collider.name.StartsWith("bullet"))
                    addDamage = collider.GetComponent<PlayerBullet>().addDamage;

                int changeHP = currentShieldHP - (GameManager.info.playerState.attack + addDamage);

                currentShieldHP = changeHP;

                if (changeHP < 0)
                {
                    OpenCloseShield();
                    currentShieldHP = 0;
                }

                renderer.material.SetFloat("_CurrentHP", currentShieldHP / (float)shieldMaxHp);

                Destroy(collider.gameObject);
            }

        }
    }
}
