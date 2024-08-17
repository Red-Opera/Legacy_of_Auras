using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerAurasArrow : MonoBehaviour
{
    public int addDamage = 20;            // ȭ���� �߰� ���ݷ�

    [SerializeField] private float range = 30.0f;
    [SerializeField] private float rotationTime = 10.0f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float maxSpeedTime = 2.0f;
    [SerializeField] private GameObject bloodEffect;        // �ǰ� ����Ʈ

    private GameObject player;          // �÷��̾� ������Ʈ
    private Transform targetTransform;  // ���� Ÿ�� ��ġ
    private MonsterHPBar monsterHpBar;  // ���� ü�¹�

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
        // ȭ���� �߷¿� ���� ���������� ����

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

            // ���� �Ǵ� ���尡 �ƴ� ��� ����
            if (!collider[i].gameObject.activeSelf || (tagName != "Monster" && tagName != "Sheld"))
                continue;

            // ȭ���� ���� ����� ������ ���
            if (tagName == "Monster")
            {
                Transform target = collider[i].transform;
                monsterHpBar = target.GetComponent<MonsterHPBar>();

                // �θ�� ��� �̵��ϸ鼭 MonsterHPBar�� �ִ��� Ȯ��
                while (monsterHpBar == null && target.parent != null)
                {
                    target = target.parent;
                    monsterHpBar = target.GetComponent<MonsterHPBar>();
                }

                if (monsterHpBar != null)
                {
                    monsterHpBar = target.GetComponent<MonsterHPBar>();

                    // ���� ü���� 0�� ��� �ش� ���ͷ� �̵����� ����
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

            // �ش� ���Ϳ� �Ÿ��� ������ Ȯ��
            float currentDistance = (collider[i].transform.position - player.transform.position).magnitude;

            // �ش� ���Ͱ� ���� ����� ��� �ּ� �Ÿ��� ����
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

        // ���� ����� ���ͷ� ����
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
        // ȭ���� ���Ϳ��� ���� ���
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

            // �浹�� ��ü�� Monster �±׸� ���� ���
            Transform target = collisionTarget.transform;
            MonsterHPBar monsterHpBar = target.GetComponent<MonsterHPBar>();

            // �θ�� ��� �̵��ϸ鼭 MonsterHPBar�� �ִ��� Ȯ��
            while (monsterHpBar == null && target.parent != null)
            {
                target = target.parent;
                monsterHpBar = target.GetComponent<MonsterHPBar>();
            }

            if (monsterHpBar != null)
            {
                monsterHpBar = target.GetComponent<MonsterHPBar>();

                // MonsterHPBar ������Ʈ�� �ִٸ� SetDamage�� ����
                monsterHpBar.SetDamage(addDamage);
            }

            else if (monsterHpBar == null && target.GetComponent<LastBossHpBar>() != null)
            {
                LastBossHpBar hpBar = target.GetComponent<LastBossHpBar>();

                hpBar.SetDamage(addDamage);
            }

            // �ǰ� ����Ʈ
            GameObject effect = Instantiate(bloodEffect, transform);
            effect.transform.parent = null;

            effect.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            effect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            effect.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
            effect.transform.localRotation = Quaternion.identity;

            effect.transform.GetChild(0).GetComponent<VisualEffect>().Play();
            Destroy(effect, 60f);

            StopAllCoroutines();

            // ȭ�� ������Ʈ ����
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
